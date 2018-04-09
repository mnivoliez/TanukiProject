using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiGeneralBehavior : YokaiController {

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

    [Header("FLOCKING YOKAI")]
    [Space(8)]
    [SerializeField]
    private float neighbourDistance = 2.0f;

    private List<GameObject> yokaiList;

    void Start() {
        positionOrigin = transform.position;
        rotationOrigin = transform.rotation;
        target = GameObject.FindGameObjectWithTag("Player");
        body = gameObject.GetComponent<Rigidbody>();
        yokaiList = new List<GameObject>(GameObject.FindGameObjectsWithTag("Yokai"));
    }

    void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        if (isAbsorbed) {
            Die();
        }
        //yokaiList = GameObject.FindGameObjectsWithTag("Yokai");
    }

    private void FixedUpdate() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
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
                        Vector3 normalize = relativePos.normalized;
                        body.velocity = normalize * speed;
                    }
                    else {
                        body.velocity = Vector3.zero;
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
            }
            else {
                //comeback the origine position
                if ((int)transform.position.x != (int)positionOrigin.x || (int)transform.position.z != (int)positionOrigin.z) {
                    Vector3 relativePos = positionOrigin - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos);
                    rotation.x = transform.rotation.x;
                    rotation.z = transform.rotation.z;
                    transform.rotation = rotation;
                    transform.Translate(0, 0, speed * Time.deltaTime);
                }
                else {
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
            }
            else if (other.gameObject.tag == "Leaf" && other.gameObject.GetComponent<MeleeAttackTrigger>() != null) {
                damage = other.gameObject.GetComponent<MeleeAttackTrigger>().GetDamage();
            }

            SoundController.instance.PlayYokaiSingle(yokaiHurt);
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
            //target = GameObject.FindGameObjectWithTag("Player");
            SoundController.instance.PlayYokaiSingle(yokaiScream);
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
        SoundController.instance.PlayYokaiSingle(absorbed);
    }

    public override void Die() {
        if (Vector3.Distance(transform.position, target.transform.position) < 0.5) {
            target.GetComponent<Animator>().SetBool("isAbsorbing", false);
            Destroy(gameObject);
        }
        else {
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