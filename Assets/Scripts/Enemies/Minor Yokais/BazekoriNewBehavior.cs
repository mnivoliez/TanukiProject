using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//================================================
//SOUNDCONTROLER
//================================================

public class BazekoriNewBehavior : YokaiController {

    [SerializeField] private float detectArea = 20.0f;
    private Quaternion rotationOrigin;
    private NavMeshAgent agent;
    private float stoppingDistance;

    [SerializeField] private LayerMask ignoredLayerMash;
    [SerializeField] private GameObject attackRange;
    private bool bodyAttack = false;
    private bool isAttacking = false;

    [SerializeField] private float zoneBehavior = 5f;
    [SerializeField] private float rateBehavior = 4f;
    private float nextRate = 0f;

    private Color initColor = new Color(202 / 255f, 54 / 255f, 255f / 255f, 0f);
    private Color hitColor = new Color(1f, 0, 0, 90f / 255f);

    [SerializeField] private GameObject hpCollectable;
    private Vector3 positionCollectable;
    [SerializeField] private Renderer renderBody;
    private Animator animBody;

    [SerializeField]
    private GameObject AttackPrefab;

    [SerializeField]
    private GameObject LandingPrefab;

    private void Start() {
        positionOrigin = transform.position;
        rotationOrigin = transform.rotation;
        comeBack = false;
        agent = GetComponent<NavMeshAgent>();
        stoppingDistance = agent.stoppingDistance;
        rendererMat = renderBody.material;
        attackRange.GetComponent<MeleeAttackEnemyTrigger>().SetDamage(damage);
        animBody = GetComponent<Animator>();
    }

    private void Update() {
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
        if (!isKnocked && !isAbsorbed) {
            animBody.SetFloat("Speed", agent.velocity.magnitude);
            if (target != null) {
                agent.stoppingDistance = stoppingDistance;
                if (Vector3.Distance(transform.position, positionOrigin) > detectArea) {
                    comeBack = true;
                    target = null;
                }
                else {
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

                    if(Mathf.Abs(Vector3.Magnitude(transform.position) - Vector3.Magnitude(target.transform.position)) < 0.5 && !isAttacking) {
                        bodyAttack = true;
                    }

                    if (bodyAttack) {
                        if (!isAttacking) {
                            isAttacking = true;
                            animBody.SetTrigger("Attack");
                            Invoke("ActivateAttackTrigger", 1f);
                        }

                        ////attack target with rate
                        //if (Time.time > nextBodyAttack) {
                        //    nextBodyAttack = Time.time + rateBodyAttack;
                        //    if (target.tag == "Player") {
                        //        target.GetComponent<PlayerHealth>().LooseHP(damageBody);
                        //    }
                        //    else if (target.tag == "Lure") {
                        //        target.GetComponent<LureController>().BeingHit();
                        //    }
                        //}
                    }
                }
            }
            else {
                bodyAttack = false;
                if (comeBack) {
                    if ((int)transform.position.x != (int)positionOrigin.x || (int)transform.position.z != (int)positionOrigin.z) {
                        agent.stoppingDistance = 0.0f;
                        agent.SetDestination(positionOrigin);
                    } else {
                        comeBack = false;
                        agent.stoppingDistance = stoppingDistance;
                    }
                } else {
                    Behavior();
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.tag == "Lure") {

            if (Mathf.Abs(collision.gameObject.GetComponent<Rigidbody>().velocity.y) > 0.5f) {
                //================================================
                SoundController.instance.SelectYOKAI("Hurt");
                //================================================
                LooseHp(1);
                Destroy(collision.gameObject);
                GameObject.FindGameObjectWithTag("Player").GetComponent<KodaController>().ResetLeafLock();
            }
            bodyAttack = true;
        }
    }

    //private void OnCollisionExit(Collision collision) {
    //    if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Lure") {
    //        bodyAttack = false;
    //    }
    //}

    void OnTriggerEnter(Collider other) {
        if ((other.gameObject.tag == "Leaf" || (other.gameObject.tag == "Lure" && Mathf.Abs(other.gameObject.GetComponent<Rigidbody>().velocity.y) > 0.5f)) && !isKnocked) {
            if (comeBack) {
                comeBack = false;
            }
            float damage = 1;
            if (other.gameObject.tag == "Leaf" && other.gameObject.GetComponent<MoveLeaf>() != null) {
                damage = other.gameObject.GetComponent<MoveLeaf>().GetDamage();
            }
            else if (other.gameObject.tag == "Leaf" && other.gameObject.GetComponent<MeleeAttackTrigger>() != null) {
                damage = other.gameObject.GetComponent<MeleeAttackTrigger>().GetDamage();
            }

            //================================================
            SoundController.instance.SelectYOKAI("Hurt");
            //================================================
            LooseHp(damage);

        }
    }

    private void ActivateAttackTrigger() {
        attackRange.SetActive(true);
        Invoke("DeactivateAttackTrigger", 1f);
        //================================================
        SoundController.instance.SelectYOKAIBAZEKORI("Attack");
        //================================================
    }

    private void DeactivateAttackTrigger() {
        attackRange.SetActive(false);
        bodyAttack = false;
        isAttacking = false;
    }

    public override void LooseHp(float damage) {
        hp -= damage;
        BeingHit();
        Invoke("EndHit", 0.3f);
        if (hp <= 0) {
            isKnocked = true;
            animBody.SetFloat("Speed", 0);
            Vector3 posKnockedParticle = renderBody.gameObject.GetComponent<SkinnedMeshRenderer>().bounds.max;
            posKnockedParticle.x = transform.position.x;
            posKnockedParticle.z = transform.position.z;
            Instantiate(knockedParticle, posKnockedParticle, Quaternion.identity).transform.parent = transform;
            rendererMat.SetColor("_Color", hitColor);
            //================================================
            SoundController.instance.SelectYOKAI("KO");
            //================================================
            target = GameObject.FindGameObjectWithTag("Player");
            agent.radius = 0.05f;
            //GetComponent<Collider>().enabled = false;
            comeBack = false;
        }
    }

    public override void BeingHit() {
        rendererMat.SetColor("_FirstDColor", hitColor);
        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
    }

    public override void EndHit() {
        if (!isKnocked) rendererMat.SetColor("_FirstDColor", initColor);
    }

    public override void Absorbed() {
        isAbsorbed = true;
        positionCollectable = transform.position;
        positionCollectable.y = positionCollectable.y + 1;
        gameObject.GetComponent<Collider>().enabled = false;
        //================================================
        SoundController.instance.SelectYOKAI("Absorbed");
        //================================================
    }

    public override void Die() {
        if (Mathf.Abs(Vector3.Magnitude(transform.position) - Vector3.Magnitude(target.transform.position)) < 0.5) {
            if (Random.Range(0, 10) > 5.9f) {
                Instantiate(hpCollectable, positionCollectable, Quaternion.identity);
                //================================================
                SoundController.instance.SelectENVQuick("FruitLoot");
                //================================================
            }
            target.GetComponent<Animator>().SetBool("IsAbsorbing", false);
            Destroy(gameObject);
        }
        else {
            if (transform.localScale.x < 0 && transform.localScale.y < 0 && transform.localScale.z < 0) {
                transform.localScale = Vector3.zero;
            }
            else {
                transform.localScale -= Vector3.one * 0.2f;
            }
            speed = speed + 0.2f;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            transform.Rotate(Vector3.right, rotationSpeed);
            transform.Rotate(Vector3.up, rotationSpeed);
            rotationSpeed += 2;
        }

    }

    public override void Behavior() {

        if (Random.Range(0, 100) < 3) {
            if (Time.time > nextRate) {
                nextRate = Time.time + rateBehavior;
                Vector3 direction = new Vector3(Random.Range(-zoneBehavior, zoneBehavior), Random.Range(-zoneBehavior, zoneBehavior), Random.Range(-zoneBehavior, zoneBehavior));
                Vector3 positionBehavior = positionOrigin + direction;
                agent.SetDestination(positionBehavior);
            }
        }
    }

    public void AttackFX() {
        Instantiate(AttackPrefab, transform.position + transform.forward*2.5f, transform.rotation);
    }

    public void LandingFX() {
        GameObject go = Instantiate(LandingPrefab, new Vector3(transform.position.x, transform.position.y+.5f, transform.position.z), transform.rotation);
        go.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
    }
}
