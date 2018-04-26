using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//================================================
//SOUNDCONTROLER
//================================================

public class Zone2BossBehavior : YokaiController {

    private GameObject corps;
    private List<GameObject> oreilles;
    
    private Rigidbody myRigidbody;
    private float nextThrow;
    private Transform spawnRock;
    private bool stop;
    private int phasePattern = 0;
    private bool interPhase;
    private List<GameObject> yokais;
    private List<GameObject> yokaisSpe;
    private float hpMax = 10;
    private bool doKnockBack;
    private bool doYell;
    private float timeStamp;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float currentRocksToThrow;
    private GameObject player;
    private SphereCollider myCollider;
    private bool canBump;
    private float delayToBump;
    private List<GameObject> lanternsInRange;
    private float throwRate;
    private GameObject[] lanterns;
    private GameObject[] respawnsLantern;
    private Color defaultColor;

    [SerializeField] private float timeToBump = 3;
    [SerializeField] private float timeToKnockBack;
    [SerializeField] private float throwRateP1 = 6;
    [SerializeField] private float throwRateP2 = 1;
    [SerializeField] private int nbRocksToThrow = 5;
    [SerializeField] private GameObject prefabRock;
    [SerializeField] private GameObject prefabSmoke;
    [SerializeField] private GameObject river;
    [SerializeField] private GameObject positionLanternPhase1;

	// Use this for initialization
	void Start () {
        lanternsInRange = new List<GameObject>();
        canBump = false;
        delayToBump = timeToBump;
        doKnockBack = false;
        phasePattern = 1;
        throwRate = throwRateP1;
        interPhase = false;
        nextThrow = 0;
        stop = false;
        currentRocksToThrow = nbRocksToThrow;
        spawnRock = transform.Find("SpawnRock");
        corps = transform.Find("corps").gameObject;
        

        SetTarget(GameObject.FindGameObjectWithTag("Player"));
        player = target;
        rendererMat = corps.GetComponent<Renderer>().material;
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<SphereCollider>();
        defaultColor = rendererMat.GetColor("_FirstLColor");
        foreach (Transform child in river.transform) {
			Collider colliderChild = child.GetComponent<Collider> ();
			if (colliderChild != null) {
				Physics.IgnoreCollision (myCollider, colliderChild, true);
			} else {
				Physics.IgnoreCollision (myCollider, child.GetComponentInChildren<Collider> (), true);
			}
        }
        myRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        hpMax = hp;
        
        yokais = new List<GameObject>();
        yokaisSpe = new List<GameObject>();
        oreilles = new List<GameObject>();

        GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];

        for (var i = 0; i < gameObjects.Length; i++) {
            if (gameObjects[i].name.Contains("Yokai_Barrier")) {
                yokais.Add(gameObjects[i]);
            } else if (gameObjects[i].name.Contains("YokaiSpeLure")) {
                yokaisSpe.Add(gameObjects[i]);
            } else if (gameObjects[i].name.Contains("Oreille") || gameObjects[i].name == "Corps Caché") {
                oreilles.Add(gameObjects[i]);
                gameObjects[i].SetActive(false);
            } else if (gameObjects[i].name.Contains("Tori")) {
                Physics.IgnoreCollision(myCollider, gameObjects[i].GetComponent<Collider>(), true);
            }
        }
    }

    void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================

        RaycastHit hit;

        /*if (Physics.Raycast(transform.position, Vector3.down, out hit, LayerMask.NameToLayer("Ground"))) {
            float rayon = myCollider.radius * (transform.localScale.y);
            if (hit.distance < rayon && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                transform.position = new Vector3(transform.position.x, hit.point.y + rayon, transform.position.z);
            }
        }*/

        if (!isKnocked) {

            for (int i = 0; i < lanternsInRange.Count; i++) {
                if (lanternsInRange[i] == null) {
                    lanternsInRange.RemoveAt(i);
                }
            }

            if (interPhase) {
                if (yokais.Count > 0) {
                    GameObject objectToDestroy = yokais[yokais.Count - 1];
                    yokais.Remove(objectToDestroy);
                    Destroy(objectToDestroy);
                } else {
                    foreach (GameObject respawnLantern in respawnsLantern) {
                        respawnLantern.SetActive(true);
                    }

                    foreach (GameObject lantern in lanterns) {
                        lantern.GetComponent<LanternController>().Respawn();
                    }

                    interPhase = false;
                    phasePattern = 2;
                }
            } else if (phasePattern == 1) {                                                     //PHASE 1
                if (lanterns == null) {
                    lanterns = GameObject.FindGameObjectsWithTag("Lantern");
                    respawnsLantern = GameObject.FindGameObjectsWithTag("LoveHotel");
                    foreach (GameObject lantern in lanterns) {
                        lantern.transform.position = positionLanternPhase1.transform.position;
                        Physics.IgnoreCollision(lantern.GetComponent<BoxCollider>(), myCollider);
                    }

                    foreach (GameObject respawnLantern in respawnsLantern) {
                        respawnLantern.SetActive(false);
                        Physics.IgnoreCollision(respawnLantern.GetComponent<Collider>(), myCollider, true);
                    }
                }

                transform.LookAt(target.transform);
                if (doKnockBack) {
                    KnockBack(startPosition, endPosition);
                } else {
                    if (Time.time >= nextThrow) {
                        ThrowRock();
                    }
                }
                
            } else if (phasePattern == 2) {                                                     //PHASE 2

                GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
                yokaisSpe.Clear();
                for (var i = 0; i < gameObjects.Length; i++) {
                    if (gameObjects[i].name.Contains("YokaiSpeLure")) {
                        yokaisSpe.Add(gameObjects[i]);
                    }
                }

                if (doKnockBack) {
                    KnockBack(startPosition, endPosition);
                } else if (canBump) {
                    if (delayToBump > 0) {
                        oreilles[0].GetComponent<Renderer>().material.SetColor("_FirstLColor", Color.yellow);
                        myRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
                        if (target != null) {
                            Physics.IgnoreCollision(target.GetComponent<Collider>(), myCollider, true);
                        }
                        delayToBump -= Time.fixedDeltaTime;
                    } else {
                        BumpTarget();
                        oreilles[0].GetComponent<Renderer>().material.SetColor("_FirstLColor", defaultColor);
                    }
                } else {
                    if (target == null) {
                        target = player;
                    }

                    if (stop == true) {
                        if (corps.activeSelf == false) {
                            corps.SetActive(true);
                            foreach (GameObject oreille in oreilles) {
                                oreille.SetActive(false);
                            }
                        }
                        myRigidbody.constraints = RigidbodyConstraints.None;
                        Stop();
                    }
                    else {
                        if (IsUnderTarget()) {
                            canBump = true;
                        }
                        else {
                            transform.localRotation = Quaternion.identity;
                            if (corps.activeSelf == true) {
                                corps.SetActive(false);
                                foreach (GameObject oreille in oreilles) {
                                    oreille.SetActive(true);
                                }
                            }
                        }
                        

                        FollowTarget();
                        if (currentRocksToThrow == 0) {
                            currentRocksToThrow = nbRocksToThrow;
                        }
                    }
                }
            }
        }

        if (isAbsorbed) {
            Die();
        }
    }

    void ThrowRock() {
        Debug.Log("throw rock");
        GameObject theRock = Instantiate(prefabRock);
        theRock.transform.position = spawnRock.position;
        theRock.transform.LookAt(target.transform);
        Vector3 direction = (target.transform.position - theRock.transform.position).normalized;
        theRock.GetComponent<Rigidbody>().AddForce(direction * 20, ForceMode.Impulse);
        nextThrow = Time.time + throwRate;
    }

    void Stop() {
        Physics.IgnoreCollision(target.GetComponent<Collider>(), myCollider, false);
        foreach (GameObject yokaiSpe in yokaisSpe) {
            Physics.IgnoreCollision(yokaiSpe.GetComponent<Collider>(), myCollider, false);
        }
        transform.LookAt(target.transform);
        myRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        if (currentRocksToThrow > 0) {
            if (Time.time >= nextThrow) {
                ThrowRock();
                currentRocksToThrow--;
            }
        } else {
            stop = false;
        }
    }


    public override void LooseHp(float damage) {
        if (!isKnocked) {
            //================================================
            SoundController.instance.SelectYOKAI("Hurt");
            //================================================
            hp -= damage;

            if (phasePattern == 1 && hp <= hpMax / 2) {
                interPhase = true;
            }

            if (hp <= 0) {
                transform.localScale = new Vector3(5, 5, 5);
                Instantiate(prefabSmoke, transform.position, Quaternion.identity);
                isKnocked = true;
                Physics.IgnoreCollision(target.GetComponent<Collider>(), myCollider, false);
                Vector3 posKnockedParticle = corps.GetComponent<MeshRenderer>().bounds.max;
                posKnockedParticle.x = transform.position.x;
                posKnockedParticle.z = transform.position.z;
                Instantiate(knockedParticle, posKnockedParticle, Quaternion.identity).transform.parent = transform;
                rendererMat.SetColor("_FirstLColor", Color.red);
                //================================================
                SoundController.instance.SelectYOKAI("KO");
                //================================================
            }
        }
    }

    public override void BeingHit() {
        //================================================
        StartCoroutine(SoundController.instance.FadeOnExitTheme()); //Will launch an other theme automatically
        Game.playerData.Boss2KO = true;
        //================================================
        Invoke("EndHit", 0.5f);
        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
        rendererMat.SetColor("_FirstLColor", Color.red);
        Vector3 positionTargetForDirection = new Vector3(target.transform.position.x, gameObject.transform.position.y, target.transform.position.z);
        Vector3 direction = (positionTargetForDirection - gameObject.transform.position).normalized;
        startPosition = target.transform.position;
        endPosition = startPosition + direction * 30;
        if (phasePattern == 1) {
            doKnockBack = true;
        } else if (phasePattern == 2) {
            stop = false;
        }
        
        timeStamp = 0;
    }

    public override void EndHit() {
        if (!isKnocked) rendererMat.SetColor("_FirstLColor", defaultColor);
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
            if (transform.localScale.x < 0 && transform.localScale.y < 0 && transform.localScale.z < 0) {
                transform.localScale = Vector3.zero;
            }
            else {
                transform.localScale -= new Vector3(1f, 1f, 1f);
            }
            speed = speed + 0.2f;
            transform.position = Vector3.MoveTowards(transform.position, (target.transform.position + Vector3.up), speed * Time.deltaTime);
            transform.Rotate(Vector3.right, rotationSpeed);
            transform.Rotate(Vector3.up, rotationSpeed);
            rotationSpeed += 2;
        }
    }


    void OnTriggerEnter(Collider collid) {
        if (collid.gameObject.CompareTag("Leaf") && phasePattern == 1) {
            if (!isKnocked)
            {
                BeingHit();
                LooseHp(1);
            }
        } else if (collid.gameObject.tag == "WaterBoss2" && phasePattern == 2 && lanternsInRange.Count > 0) {
            BeingHit();
            LooseHp(1);
        } else if (collid.gameObject.tag == "Lure") {
            if (stop) {
                Physics.IgnoreCollision(myCollider, collid.gameObject.GetComponent<Collider>(), false);
            }
        }
    }

    void OnTriggerStay(Collider collid) {
        if (collid.gameObject.tag == "Lantern" && !lanternsInRange.Contains(collid.gameObject)) {
            lanternsInRange.Add(collid.gameObject);
        }
    }

    void OnTriggerExit(Collider collid) {
        if (collid.gameObject.tag == "Lantern" && lanternsInRange.Contains(collid.gameObject)) {
            lanternsInRange.Remove(collid.gameObject);
        }
    }

    private bool IsUnderTarget() {
        bool isUnderTarget = false;
        if ((int)transform.position.x == (int)target.transform.position.x && (int)transform.position.z == (int)target.transform.position.z) {
            isUnderTarget = true;
            foreach (GameObject oreille in oreilles) {
                oreille.GetComponent<Renderer>().material.color = new Color(255f / 255f, 40f / 255f, 40f / 255f);
            }
        }

        return isUnderTarget;
    }

    private void BumpTarget() {
        if (target != null) {
            Physics.IgnoreCollision(target.GetComponent<Collider>(), myCollider, false);
        }
        foreach (GameObject oreille in oreilles) {
            oreille.GetComponent<Renderer>().material.color = Color.white;
        }
        stop = true;
        delayToBump = timeToBump;
        canBump = false;
    }

    private void FollowTarget() {
        Physics.IgnoreCollision(target.GetComponent<Collider>(), myCollider, true);
        foreach (GameObject yokaiSpe in yokaisSpe) {
            Physics.IgnoreCollision(yokaiSpe.GetComponent<Collider>(), myCollider, true);
        }
        myRigidbody.constraints = RigidbodyConstraints.None;
        //follow target
        Vector3 relativePos = target.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        rotation.x = transform.rotation.x;
        rotation.z = transform.rotation.z;
        transform.rotation = rotation;

        //myRigidbody.velocity = Vector3.zero;
        float dis = Vector3.Distance(transform.position, target.transform.position);
        Vector3 vectDir = (target.transform.position - oreilles[0].transform.position).normalized;
        //transform.Translate(vectDir * speed * Time.deltaTime);
        vectDir.y = 0;
        myRigidbody.velocity = new Vector3(vectDir.x * speed, myRigidbody.velocity.y, vectDir.z * speed);
    }

    void KnockBack (Vector3 firstPosition, Vector3 secondPosition) {
        if (0 < timeToKnockBack - timeStamp) {
            Vector3 currentPos = Vector3.Lerp(firstPosition, secondPosition, (timeStamp) / timeToKnockBack);
            currentPos.y += 10 * Mathf.Sin(Mathf.Clamp01((timeStamp) / timeToKnockBack) * Mathf.PI);
            target.transform.position = currentPos;
            timeStamp += 0.1f;
        }
        else {
            stop = true;
            doKnockBack = false;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            startPosition = collision.gameObject.transform.position;
            Vector3 positionTargetForDirection = new Vector3(collision.gameObject.transform.position.x, 0, collision.gameObject.transform.position.z);
            Vector3 direction = (new Vector3(0, 0, 0) - positionTargetForDirection).normalized;
            endPosition = startPosition + direction * 30;
            endPosition = new Vector3(endPosition.x, endPosition.y + 10, endPosition.z);
            doKnockBack = true;
            timeStamp = 0;
        } else if (collision.gameObject.tag == "Lure") {
            if (stop) {
                Destroy(collision.gameObject);
            }
            else {
                Physics.IgnoreCollision(myCollider, collision.gameObject.GetComponent<Collider>(), true);
            }
        }
    }
}