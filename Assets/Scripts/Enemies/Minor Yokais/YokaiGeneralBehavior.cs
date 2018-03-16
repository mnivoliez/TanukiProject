using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiGeneralBehavior : YokaiController {

    [SerializeField]
    private float distanceLimit = 20.0f;
    private Vector3 positionOrigin;
    private Quaternion rotationOrigin;

    [SerializeField]
    private float damageBody = 1.0f;
    [SerializeField]
    private float rangeAttack = 4.0f;

    [SerializeField]
    private float rateBodyAttack = 2.0f;
    private float nextBodyAttack = 0.0f;

    private bool bodyAttack = false;

    private Rigidbody body;

    [Header("FLOCKING FISH")]
    [Space(8)]
    [SerializeField]
    private float neighbourDistance = 2.0f;

    private GameObject[] yokaiList;

    void Start() {
        positionOrigin = transform.position;
        rotationOrigin = transform.rotation;

        body = gameObject.GetComponent<Rigidbody>();
    }

    void Update() {
        if (isAbsorbed) {
            Die();
        }
        yokaiList = GameObject.FindGameObjectsWithTag("Yokai");
    }

    private void FixedUpdate() {
        if (!isKnocked) {
            if (target != null) {
                //follow target
                bool rel = applyRules(); //flocking fish
                if (!rel) {
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
                        target = GameObject.Find("Player");
                        nextBodyAttack = nextBodyAttack + rateBodyAttack;
                        target.GetComponent<PlayerHealth>().LooseHP(damageBody);
                    }
                }
            } else {
                //comeback the origine position
                if ((int)transform.position.x != (int)positionOrigin.x || (int)transform.position.z != (int)positionOrigin.z) {
                    Vector3 relativePos = positionOrigin - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos);
                    rotation.x = transform.rotation.x;
                    rotation.z = transform.rotation.z;
                    transform.rotation = rotation;
                    transform.Translate(0, 0, speed * Time.deltaTime);
                } else {
                    transform.rotation = rotationOrigin;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            bodyAttack = true;
            Vector3 vectorToCollision = collision.gameObject.transform.position - transform.position;
        }

        
        if (collision.gameObject.tag == "Lure") {

            if (collision.gameObject.GetComponent<Rigidbody>().velocity.y < 0) {
                BeingHit();
                LooseHp(1);
                EndHit();
                Destroy(collision.gameObject);
                GameObject.FindGameObjectWithTag("Player").GetComponent<KodaController>().ResetLeafLock();
            }
        }


    }
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            bodyAttack = false;
        }
    }

    void OnTriggerEnter(Collider other) {
        if ((other.gameObject.tag == "Leaf" || other.gameObject.tag == "Lure") && !isKnocked) {
            float damage = 1;
            if (other.gameObject.tag == "Leaf" && other.gameObject.GetComponent<MoveLeaf>() != null) {
                damage = other.gameObject.GetComponent<MoveLeaf>().GetDamage();
            } else if (other.gameObject.tag == "Leaf" && other.gameObject.GetComponent<MeleeAttackTrigger>() != null) {
                damage = other.gameObject.GetComponent<MeleeAttackTrigger>().GetDamage();
            }

            BeingHit();
            LooseHp(damage);
            EndHit();
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

    public bool TooFarAway() {
        bool tooFarAway = false;
        if (Vector3.Distance(transform.position, positionOrigin) > distanceLimit) {
            tooFarAway = true;
        }
        return tooFarAway;
    }

    public bool TooFarAway(Vector3 position) {
        bool tooFarAway = false;
        if (Vector3.Distance(position, positionOrigin) > distanceLimit) {
            tooFarAway = true;
        }
        return tooFarAway;
    }

    private bool applyRules() {
        bool rel = false;

        Vector3 tarPos = target.transform.position;
        tarPos.y = transform.position.y;

        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;

        float distance;
        float groupSpeed = 0.1f;
        int groupSize = 0;

        foreach (GameObject yokai in yokaiList) {
            if (yokai != gameObject) {
                distance = Vector3.Distance(yokai.transform.position, transform.position);
                if (distance < neighbourDistance) {
                    vcentre = vcentre + yokai.transform.position;
                    groupSize++;
                    if (distance < 0.5f) {
                        vavoid = vavoid + (transform.position - yokai.transform.position);
                    }
                    YokaiGeneralBehavior anotherYokai = yokai.GetComponent<YokaiGeneralBehavior>();
                    groupSpeed = groupSpeed + anotherYokai.speed;
                }
            }
        }
        if (groupSize > 0) {
            rel = true;
            vcentre = vcentre / groupSize + (tarPos - transform.position);
            float speedPlus = groupSpeed / groupSize;
            Vector3 direction = transform.position - (vcentre + vavoid);
            direction.y = 0;
            if (direction != Vector3.zero) {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
                transform.Translate(0, 0, speedPlus * Time.deltaTime);
            }
        }
        return rel;
    }
}