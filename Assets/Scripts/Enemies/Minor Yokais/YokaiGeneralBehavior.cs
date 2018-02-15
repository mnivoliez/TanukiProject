using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiGeneralBehavior : YokaiController {

    [SerializeField]
    private float distanceLimit = 20.0f; //radius of trigger sphere collider
    private Vector3 positionOrigin;
    private Quaternion rotationOrigin;
    private bool comeBack = false;

    [SerializeField]
    private float damageBody = 1.0f;
    [SerializeField]
    private float rangeAttack = 4.0f;

    [SerializeField]
    private float rateBodyAttack = 2.0f;
    private float nextBodyAttack = 0.0f;

    private bool bodyAttack = false;

    private Rigidbody body;

    void Start() {
        positionOrigin = transform.position;
        rotationOrigin = transform.rotation;

        body = gameObject.GetComponent<Rigidbody>();
    }

    void Update() {
        if (isAbsorbed) {
            Die();
        }
    }

    private void FixedUpdate() {
        if (!isKnocked) {

            if (target != null) {
                float distance = Vector3.Distance(transform.position, positionOrigin);
                //come back if Yokai go too far
                if (distance > distanceLimit) {
                    target = null;
                    comeBack = true;
                }
                else {
                    //follow target
                    Vector3 relativePos = target.transform.position - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos);
                    rotation.x = transform.rotation.x;
                    rotation.z = transform.rotation.z;
                    transform.rotation = rotation;

                    float dis = Vector3.Distance(transform.position, target.transform.position);
                    //go to target
                    if (dis > rangeAttack) {
                        transform.Translate(0, 0, speed * Time.deltaTime);
                    }
                }

                if (bodyAttack) {
                    //attack target with rate
                    if (Time.time > nextBodyAttack) {
                        nextBodyAttack = nextBodyAttack + rateBodyAttack;
                        target.GetComponent<PlayerHealth>().LooseHP(damageBody);
                    }
                }
            }
            else {
                //comeback the origine position
                if ((int)transform.position.x != (int)positionOrigin.x && (int)transform.position.z != (int)positionOrigin.z && comeBack) {
                    Vector3 relativePos = positionOrigin - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos);
                    rotation.x = transform.rotation.x;
                    rotation.z = transform.rotation.z;
                    transform.rotation = rotation;

                    transform.Translate(0, 0, speed * Time.deltaTime);
                }
                else {
                    transform.rotation = rotationOrigin;
                    comeBack = false;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            bodyAttack = true;
            Vector3 vectorToCollision = collision.gameObject.transform.position - transform.position;
        }
    }
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            bodyAttack = false;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if ((other.gameObject.tag == "Leaf" || other.gameObject.tag == "Lure") && !isKnocked) {
            float damage = 1;
            if (other.gameObject.tag == "Leaf" && other.gameObject.GetComponent<MoveLeaf>() != null) {
                damage = other.gameObject.GetComponent<MoveLeaf>().GetDamage();
            }
            else if (other.gameObject.tag == "Leaf" && other.gameObject.GetComponent<MeleeAttackTrigger>() != null) {
                damage = other.gameObject.GetComponent<MeleeAttackTrigger>().GetDamage();
            }
            else if (other.gameObject.tag == "Lure") {
                damage = 1;
            }

            BeingHit();
            LooseHp(damage);
            EndHit();
        }

        if (other.gameObject.tag == "Lure") {
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player" && !comeBack) {
            target = other.gameObject;
        }
    }

    public override void LooseHp(float damage) {
        hp = hp - damage;
        if (hp <= 0) {
            isKnocked = true;
            Instantiate(knockedParticle, transform.position, Quaternion.identity).transform.parent = transform;
            target = GameObject.Find("Player");
            //rendererMat.color = new Color(150f / 255f, 40f / 255f, 150f / 255f);
        }
    }

    public override void BeingHit() {
        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
        //rendererMat.color = new Color(150f / 255f, 40f / 255f, 150f / 255f);
    }

    public override void EndHit() {
        //rendererMat.color = Color.white;
    }

    public override void Absorbed() {
        isAbsorbed = true;
        gameObject.GetComponent<Collider>().enabled = false;
    }

    public override void Die() {
        if (transform.position == target.transform.position) {
            target.GetComponent<Animator>().SetBool("isAbsorbing", false);
            Destroy(gameObject);
        } else {
            if (transform.localScale.x > 0 && transform.localScale.y > 0 && transform.localScale.z > 0) {
                Vector3 scale = transform.localScale;
                scale -= new Vector3(20f, 20f, 20f);
                if (scale.x < 0 && scale.y < 0 && scale.z < 0) {
                    scale = Vector3.zero;
                }
                transform.localScale = scale;
            }
            speed = speed + 0.2f;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            transform.Rotate(Vector3.right, rotationSpeed);
            transform.Rotate(Vector3.up, rotationSpeed);
            rotationSpeed += 2;
        }
    }

    public override void Behavior() {

    }
}
