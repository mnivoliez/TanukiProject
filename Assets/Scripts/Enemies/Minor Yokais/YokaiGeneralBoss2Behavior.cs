using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//================================================
//SOUNDCONTROLER
//================================================

public class YokaiGeneralBoss2Behavior : YokaiController {
    
    private Quaternion rotationOrigin;
    
    [SerializeField]
    private float rangeAttack = 4.0f;

    [SerializeField]
    private float rateBodyAttack = 2.0f;
    private float nextBodyAttack = 0.0f;

    [SerializeField] private GameObject boss;
    private float rayon;

    private bool bodyAttack = false;

    private float currentSpeed;

    private Rigidbody body;

    void Start() {
        currentSpeed = speed;
        positionOrigin = transform.position;
        rotationOrigin = transform.rotation;
        Vector3 vectBossYokai = transform.position - boss.transform.position;
        rayon = vectBossYokai.magnitude;
        body = gameObject.GetComponent<Rigidbody>();
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
    }

    private void FixedUpdate() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        if (!isKnocked) {
            if (target != null) {

                Vector3 relativePos = new Vector3();
                Vector3 positionToGo = new Vector3();

                if (target.tag == "Lure") {
                    currentSpeed = speed / 2;
                    relativePos = target.transform.position - transform.position;
                    positionToGo = target.transform.position;

                } else if (target.tag == "Player") {
                    currentSpeed = speed;
                    Vector3 vectBossPlayer = target.transform.position - boss.transform.position;
                    if (vectBossPlayer.magnitude > rayon) {
                        positionToGo = boss.transform.position + rayon * vectBossPlayer.normalized;
                    } else {
                        positionToGo = target.transform.position + new Vector3(target.transform.rotation.x, target.transform.rotation.y, target.transform.rotation.z) * 2;
                    }
                }

                relativePos = positionToGo - transform.position;

                Vector3 lookAtTarget = target.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(lookAtTarget);
                rotation.x = transform.rotation.x;
                rotation.z = transform.rotation.z;
                transform.rotation = rotation;

                //go to target
                //if (Math.Round(positionToGo.x, 1) != Math.Round(transform.position.x, 1) || Math.Round(positionToGo.y, 1) != Math.Round(transform.position.y, 2) || Math.Round(positionToGo.z, 2) != Math.Round(transform.position.z, 2)) {
                if ((int)positionToGo.x != (int)transform.position.x || Math.Round(positionToGo.y, 1) != Math.Round(transform.position.y, 1) || (int)positionToGo.z != (int)transform.position.z) {
                    Vector3 normalize = relativePos.normalized;
                    body.velocity = normalize * currentSpeed;
                    currentSpeed = speed;
                }
                else {
                    body.velocity = Vector3.zero;
                    currentSpeed = speed;
                }

                
                if (bodyAttack) {
                    //attack target with rate
                    if (Time.time > nextBodyAttack) {
                        target = GameObject.Find("Player");
                        nextBodyAttack = nextBodyAttack + rateBodyAttack;
                        target.GetComponent<PlayerHealth>().LooseHP(damage);
                    }
                }
            }
            else {
                //comeback the origine position
                if ((int)transform.position.x != (int)positionOrigin.x || (int)transform.position.y != (int)positionOrigin.y || (int)transform.position.z != (int)positionOrigin.z) {
                    Vector3 relativePos = positionOrigin - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos);
                    rotation.x = transform.rotation.x;
                    rotation.z = transform.rotation.z;
                    transform.rotation = rotation;
                    Vector3 normalize = relativePos.normalized;
                    body.velocity = normalize * speed / 4;
                }
                else {
                    transform.rotation = rotationOrigin;
                    body.velocity = Vector3.zero;
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
                //================================================
                SoundController.instance.SelectYOKAI("Hurt");
                //================================================
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

    /*void OnTriggerEnter(Collider other) {
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
    }*/

    public override void LooseHp(float damage) {
        hp = hp - damage;
        if (hp <= 0) {
            isKnocked = true;
            Instantiate(knockedParticle, transform.position, Quaternion.identity).transform.parent = transform;
            target = GameObject.FindGameObjectWithTag("Player");
            //================================================
            SoundController.instance.SelectYOKAI("Scream");
            //================================================
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
        //================================================
        SoundController.instance.SelectYOKAI("Absorbed");
        //================================================
    }

    public override void Die() {
        if (Mathf.Abs(Vector3.Magnitude(transform.position) - Vector3.Magnitude(target.transform.position)) < 0.2) {
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
