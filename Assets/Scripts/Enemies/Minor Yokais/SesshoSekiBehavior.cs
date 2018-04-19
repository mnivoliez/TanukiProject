using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//================================================
//SOUNDCONTROLER
//================================================

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
    private float nextAction;
    private Renderer myRenderer;
    private float offsetRaycast = 1.0f;
    private Vector3 positionWithOffset;
    private Vector3 targetPositionWithOffset;

    [SerializeField] private float durationOfPreparation = 2;
    [SerializeField] private float durationOfResearch = 2;

    private Color initColor = new Color(97f / 255f, 88f / 255f, 99f / 255f, 84f / 255f);
    private Color chargeColor = new Color(210f / 255f, 210f / 255f, 70f / 255f, 80f / 255f);
    private Color hitColor = new Color(180f / 255f, 70f / 255f, 80f / 255f, 80f/ 255f);


    void Start() {
        comeBack = true;
        prepare = true;
        positionOrigin = transform.position;
        rotationOrigin = transform.rotation;
        body = gameObject.GetComponent<Rigidbody>();
        nextAction = 0;
        positionTargetSet = false;
        rendererMat = GetComponent<Renderer>().material;
        myRenderer = GetComponent<Renderer>();
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

        positionWithOffset = new Vector3(transform.position.x, transform.position.y + offsetRaycast, transform.position.z);
        if (target != null) {
            targetPositionWithOffset = new Vector3(target.transform.position.x, target.transform.position.y + offsetRaycast, target.transform.position.z);
        }
    }

    private void FixedUpdate() {
        
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================

        positionWithOffset = new Vector3(transform.position.x, transform.position.y + offsetRaycast, transform.position.z);

        if (!isKnocked) {
            RaycastHit myHit;
            if (target != null && Physics.Raycast(positionWithOffset, targetPositionWithOffset - positionWithOffset, out myHit, (targetPositionWithOffset - positionWithOffset).magnitude + 1f, 1 << LayerMask.NameToLayer("Player"))) {
                comeBack = false;
            }

            if (comeBack) {
                //comeback the origine position
                prepare = true;
                search = false;
                positionTargetSet = false;
                positionToGoSet = false;
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
                    body.velocity = new Vector3(0, body.velocity.y, 0);
                }
            } else if (prepare) {
                body.constraints = RigidbodyConstraints.None;
                body.velocity = new Vector3(0, body.velocity.y, 0);
                if (Time.time < nextAction || !positionTargetSet) {
                    if (!positionTargetSet) {
                        body.velocity = new Vector3(0, body.velocity.y, 0);
                        transform.rotation = Quaternion.identity;
                        positionTarget = target.transform.position;
                        transform.LookAt(target.transform);
                        rendererMat.SetColor("_FirstLColor", chargeColor);
                        positionTargetSet = true;
                        nextAction = Time.time + durationOfPreparation;
                    }

                    body.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                }
                else if (Time.time >= nextAction) {
                    body.constraints = RigidbodyConstraints.None;
                    prepare = false;
                    positionTargetSet = false;
                }
            } else if (search) {
                body.velocity = new Vector3(0, body.velocity.y, 0);
                transform.rotation = Quaternion.identity;
                if (Time.time < nextAction) {
                    //rendererMat.color = new Color(40f / 255f, 150f / 255f, 150f / 255f);
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
                body.constraints = RigidbodyConstraints.FreezeRotation;

                RaycastHit hit;

                Vector3 relativePos = new Vector3();
                if (!positionToGoSet) {
                    relativePos = (positionTarget - transform.position) * 1.5f;
                    positionToGo = transform.position + relativePos;
                    rendererMat.SetColor("_FirstLColor", initColor);
                    positionToGoSet = true;
                }

                relativePos = positionToGo - transform.position;
                relativePos.y = 0;

                Vector3 lookAtTarget = positionToGo - transform.position;

                int layerMask = ~(1 << LayerMask.NameToLayer("Player"));
                if (Physics.Raycast(positionWithOffset, targetPositionWithOffset - positionWithOffset, out hit, (targetPositionWithOffset - positionWithOffset).magnitude, layerMask)) {
                    positionToGo = hit.point - (relativePos.normalized * myRenderer.bounds.size.x * transform.localScale.x / 4);
                    relativePos = positionToGo - transform.position;
                    relativePos.y = 0;
                }

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
        if (!isKnocked) {

            if (collision.gameObject.tag == "Player") {
                collision.gameObject.GetComponent<PlayerHealth>().LooseHP(damage);
                Destroy(Instantiate(hitParticle, collision.gameObject.transform.position, Quaternion.identity), 1);

            } else if (collision.gameObject.tag == "Lure") {

                if (Math.Abs(collision.gameObject.GetComponent<Rigidbody>().velocity.y) > 0.5f) {
                //================================================
                SoundController.instance.SelectYOKAI("Hurt");
                //================================================
                    LooseHp(1);
                    Destroy(collision.gameObject);
                    GameObject.FindGameObjectWithTag("Player").GetComponent<KodaController>().ResetLeafLock();
                }
            } else if (collision.gameObject.tag != "Ground") {
                ContactPoint[] contacts = collision.contacts;
                foreach (ContactPoint contact in contacts) {
                    float coefInclination = Vector3.Angle(contact.normal, Vector3.up);
                    if (coefInclination > 10f) {
                        search = true;
                        nextAction = durationOfResearch + Time.time;
                        return;
                    }
                }
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

            LooseHp(damage);
        }
    }

    public override void LooseHp(float damage) {
        BeingHit();
        hp -= damage;
        Invoke("EndHit", 0.3f);
        //================================================
        SoundController.instance.SelectYOKAI("Hurt");
        //================================================
        if (hp <= 0) {
            isKnocked = true;
            Vector3 posKnockedParticle = GetComponent<MeshRenderer>().bounds.max;
            posKnockedParticle.x = transform.position.x;
            posKnockedParticle.z = transform.position.z;
            Instantiate(knockedParticle, posKnockedParticle, Quaternion.identity).transform.parent = transform;
            rendererMat.SetColor("_FirstLColor", hitColor);
            target = GameObject.FindGameObjectWithTag("Player");
            //================================================
            SoundController.instance.SelectYOKAI("KO");
            //================================================
        }
    }

    public override void BeingHit() {
        rendererMat.SetColor("_FirstLColor", hitColor);
        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
    }

    public override void EndHit() {
        if (!isKnocked) rendererMat.SetColor("_FirstLColor", initColor);
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
