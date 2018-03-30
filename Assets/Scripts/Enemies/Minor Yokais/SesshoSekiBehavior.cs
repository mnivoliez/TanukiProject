using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SesshoSekiBehavior : YokaiController {
    
    private Quaternion rotationOrigin;

    private bool bodyAttack = false;

    private Rigidbody body;

    private Vector3 positionTarget;
    private bool positionTargetSet;
    private Vector3 positionToGo;
    private bool positionToGoSet;
    private bool prepare;
    private bool search;
    private bool comeBack;
    private float nextAction;

    [SerializeField] private float durationOfPreparation = 2;
    [SerializeField] private float durationOfResearch = 2;


    void Start() {
        comeBack = true;
        prepare = true;
        positionOrigin = transform.position;
        rotationOrigin = transform.rotation;
        body = gameObject.GetComponent<Rigidbody>();
        nextAction = 0;
        positionTargetSet = false;
        rendererMat = GetComponent<Renderer>().material;
    }

    void Update() {
        if (isAbsorbed) {
            Die();
        }
    }

    private void FixedUpdate() {
        if (!isKnocked) {
            if (target != null) {
                comeBack = false;
            }

            if (comeBack) {
                //comeback the origine position
                prepare = true;
                search = false;
                positionTargetSet = false;
                positionToGoSet = false;
                rendererMat.color = Color.white;
                if ((int)transform.position.x != (int)positionOrigin.x || (int)transform.position.z != (int)positionOrigin.z) {
                    Vector3 relativePos = positionOrigin - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos);
                    rotation.x = transform.rotation.x;
                    rotation.z = transform.rotation.z;
                    transform.rotation = rotation;
                    Vector3 normalize = relativePos.normalized;
                    normalize.y = 0;
                    body.velocity = normalize * speed;
                }
                else {
                    transform.rotation = rotationOrigin;
                    body.velocity = Vector3.zero;
                }
            } else if (prepare) {
                body.velocity = Vector3.zero;
                if (Time.time < nextAction || !positionTargetSet) {
                    if (!positionTargetSet) {
                        body.velocity = Vector3.zero;
                        transform.rotation = Quaternion.identity;
                        positionTarget = target.transform.position;
                        transform.LookAt(target.transform);
                        rendererMat.color = new Color(150f / 255f, 40f / 255f, 150f / 255f);
                        positionTargetSet = true;
                        nextAction = Time.time + durationOfPreparation;
                    }
                }
                else if (Time.time >= nextAction) {
                    prepare = false;
                    positionTargetSet = false;
                }
            } else if (search) {
                body.velocity = Vector3.zero;
                body.velocity = Vector3.zero;
                transform.rotation = Quaternion.identity;
                if (Time.time < nextAction) {
                    rendererMat.color = new Color(40f / 255f, 150f / 255f, 150f / 255f);
                }
                else {
                    if (target == null) {
                        comeBack = true;
                    }
                    search = false;
                    prepare = true;
                    positionToGoSet = false;
                }

            }
            else {
                Vector3 relativePos = new Vector3();
                if (!positionToGoSet) {
                    relativePos = (positionTarget - transform.position) * 1.5f;
                    positionToGo = transform.position + relativePos;
                    rendererMat.color = new Color(150f / 255f, 150f / 255f, 40f / 255f);
                    positionToGoSet = true;
                }

                relativePos = positionToGo - transform.position;
                relativePos.y = 0;

                Vector3 lookAtTarget = positionToGo - transform.position;
                Quaternion rotation = Quaternion.LookRotation(lookAtTarget);
                rotation.x = transform.rotation.x;
                rotation.z = transform.rotation.z;
                transform.rotation = rotation;

                if ((int)positionToGo.x != (int)transform.position.x || (int)positionToGo.z != (int)transform.position.z) {
                    Vector3 normalize = relativePos.normalized;
                    body.velocity = normalize * speed;
                }
                else {
                    search = true;
                    nextAction = durationOfResearch + Time.time;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player" && !isKnocked) {
            collision.gameObject.GetComponent<PlayerHealth>().LooseHP(damage);
            Destroy(Instantiate(hitParticle, collision.gameObject.transform.position, Quaternion.identity), 1);
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

            BeingHit();
            LooseHp(damage);
            EndHit();
        }
    }

    public override void LooseHp(float damage) {
        hp = hp - damage;
        if (hp <= 0) {
            isKnocked = true;
            Vector3 posKnockedParticle = GetComponent<MeshRenderer>().bounds.max;
            posKnockedParticle.x = transform.position.x;
            posKnockedParticle.z = transform.position.z;
            Instantiate(knockedParticle, posKnockedParticle, Quaternion.identity).transform.parent = transform;
            rendererMat.SetColor("_Globalcolor", new Color(255f / 255f, 255f / 255f, 255f / 255f));
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
}
