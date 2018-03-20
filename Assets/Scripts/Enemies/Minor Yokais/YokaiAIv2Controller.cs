using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class YokaiAIv2Controller : YokaiController {

    [SerializeField]
    private float detectArea = 20.0f;

    private Vector3 positionOrigin;
    private Quaternion rotationOrigin;
    public bool comeBack;
    private NavMeshAgent agent;
    private float stoppingDistance;

    [SerializeField]
    private LayerMask ignoredLayerMash;

    [SerializeField]
    private float damageBody = 1.0f;
    [SerializeField]
    private float rateBodyAttack = 2.0f;
    private float nextBodyAttack = 0.0f;

    private bool bodyAttack = false;
    private Rigidbody body;

    private void Start() {
        positionOrigin = transform.position;
        rotationOrigin = transform.rotation;
        comeBack = false;
        agent = GetComponent<NavMeshAgent>();
        stoppingDistance = agent.stoppingDistance;

        body = GetComponent<Rigidbody>();
    }

    private void Update() {
        if (isAbsorbed) {
            Debug.Log("DIE !!");
            Die();
        }
    }

    private void FixedUpdate() {
        if (!isKnocked) {
            if (target != null) {
                agent.stoppingDistance = stoppingDistance;
                if (Vector3.Distance(transform.position, positionOrigin) > detectArea) {
                    comeBack = true;
                    target = null;
                } else {
                    Vector3 relativePos = target.transform.position - transform.position;
                    // all layers are = 0xFFFFFFFF => -1
                    int layerAll = -1;
                    RaycastHit hit;
                    if (Physics.Raycast(target.transform.position, -relativePos, out hit, detectArea, layerAll - ignoredLayerMash.value)) {
                        //Debug.Log(target.transform.position);
                        //Debug.Log(hit.transform.tag);
                        if (hit.transform.tag == "Yokai") {
                            Quaternion rotation = Quaternion.LookRotation(relativePos);
                            rotation.x = transform.rotation.x;
                            rotation.z = transform.rotation.z;
                            transform.rotation = rotation;

                            agent.SetDestination(target.transform.position);
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
            } else {
                if ((int)transform.position.x != (int)positionOrigin.x || (int)transform.position.z != (int)positionOrigin.z) {
                    agent.stoppingDistance = 0.0f;
                    agent.SetDestination(positionOrigin);
                } else {
                    comeBack = false;
                    agent.stoppingDistance = stoppingDistance;
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
            BeingHit();
            LooseHp(1);
            EndHit();

            Destroy(collision.gameObject);
        }


    }
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            bodyAttack = false;
        }
    }

    void OnTriggerEnter(Collider other) {
        if ((other.gameObject.tag == "Leaf" || other.gameObject.tag == "Lure") && !isKnocked) {
            if (comeBack) {
                comeBack = false;
            }
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
            Debug.Log("KNOCKED !!");
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
        Debug.Log("ABSORBE !!");
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
}