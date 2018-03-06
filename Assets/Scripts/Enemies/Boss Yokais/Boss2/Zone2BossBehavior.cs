﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float hpMax = 10;
    private bool doKnockBack;
    private float timeStamp;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float currentRocksToThrow;
    private GameObject player;
    private Collider myCollider;
    private bool canBump;
    private float delayToBump;
    private int objectsWaterInContact;

    [SerializeField] private float timeToBump = 3;
    [SerializeField] private float timeToKnockBack;
    [SerializeField] private float throwRate = 1;
    [SerializeField] private float stopRate = 5;
    [SerializeField] private int nbRocksToThrow = 5;
    [SerializeField] private GameObject prefabRock;

	// Use this for initialization
	void Start () {
        objectsWaterInContact = 0;
        canBump = false;
        delayToBump = timeToBump;
        doKnockBack = false;
        phasePattern = 1;
        interPhase = false;
        nextThrow = 0;
        stop = false;
        currentRocksToThrow = nbRocksToThrow;
        spawnRock = transform.Find("SpawnRock");
        corps = transform.Find("corps").gameObject;

        SetTarget(GameObject.Find("Player"));
        player = target;
        rendererMat = corps.GetComponent<Renderer>().material;
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        myRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        hpMax = hp;

        yokais = new List<GameObject>();
        oreilles = new List<GameObject>();

        GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];

        for (var i = 0; i < gameObjects.Length; i++) {
            if (gameObjects[i].name.Contains("Yokai")) {
                yokais.Add(gameObjects[i]);
            } else if (gameObjects[i].name.Contains("Oreille")) {
                oreilles.Add(gameObjects[i]);
                gameObjects[i].SetActive(false);
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown("p")) {
            phasePattern = 2;
            throwRate = 0.6f;
            myRigidbody.constraints = RigidbodyConstraints.None;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!isKnocked) {
            
            if (interPhase) {
                if (yokais.Count > 0) {
                    GameObject objectToDestroy = yokais[yokais.Count - 1];
                    yokais.Remove(objectToDestroy);
                    Destroy(objectToDestroy);
                } else {
                    interPhase = false;
                    phasePattern = 2;
                }
            } else if (phasePattern == 1) {                                                     //PHASE 1
                transform.LookAt(target.transform);
                if (doKnockBack) {
                    KnockBack(startPosition, endPosition);
                } else {
                    if (Time.time >= nextThrow) {
                        ThrowRock();
                    }
                }
                
            } else if (phasePattern == 2) {                                                     //PHASE 2
                if (doKnockBack) {
                    KnockBack(startPosition, endPosition);
                }
                else if (canBump) {
                    if (delayToBump > 0) {
                        delayToBump -= Time.fixedDeltaTime;
                    } else {
                        BumpTarget();
                    }
                } else {
                    GameObject lure = GameObject.FindGameObjectWithTag("Lure");
                    if (target == null) {
                        target = player;
                    }

                    if (target.tag == "Player") {
                        if (lure != null) {
                            target = lure;
                        }
                    }
                    else if (target.tag == "Lure") {
                        if (lure == null) {
                            target = player;
                        }
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
	}

    void ThrowRock() {
        GameObject theRock = Instantiate(prefabRock);
        theRock.transform.position = spawnRock.position;
        theRock.transform.LookAt(target.transform);
        Vector3 direction = (target.transform.position - theRock.transform.position).normalized;
        theRock.GetComponent<Rigidbody>().AddForce(direction * 20, ForceMode.Impulse);
        nextThrow = Time.time + throwRate;
    }

    void Stop() {
        Physics.IgnoreCollision(target.GetComponent<Collider>(), myCollider, false);
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
            hp -= damage;

            if (phasePattern == 1 && hp <= hpMax / 2) {
                interPhase = true;
            }

            if (hp <= 0) {
                isKnocked = true;
                Instantiate(knockedParticle, transform.position, Quaternion.identity).transform.parent = transform;
                rendererMat.color = new Color(150f / 255f, 40f / 255f, 150f / 255f);
            }
        }
    }

    public override void BeingHit() {
        Invoke("EndHit", 0.5f);
        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
        rendererMat.color = new Color(150f / 255f, 40f / 255f, 150f / 255f);
        Vector3 positionTargetForDirection = new Vector3(target.transform.position.x, gameObject.transform.position.y, target.transform.position.z);
        Vector3 direction = (positionTargetForDirection - gameObject.transform.position).normalized;
        startPosition = target.transform.position;
        endPosition = startPosition + direction * 50;
        doKnockBack = true;
        timeStamp = 0;
    }

    public override void EndHit() {
        if (!isKnocked) rendererMat.color = Color.white;
    }

    public override void Absorbed() {
        isAbsorbed = true;
        gameObject.GetComponent<Collider>().enabled = false;
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
        if (collid.gameObject.CompareTag("Leaf")) {
            MoveLeaf ml = collid.gameObject.GetComponent<MoveLeaf>();
            if (!isKnocked && ml == null)
            {
                BeingHit();
                LooseHp(1);
            }
        } else if (collid.gameObject.layer == LayerMask.NameToLayer("Water")) {
            objectsWaterInContact++;
        }
    }

    void OnTriggerExit(Collider collid) {
        if (collid.gameObject.layer == LayerMask.NameToLayer("Water")) {
            objectsWaterInContact--;
        }
    }

    void OnTriggerStay(Collider collid) {
        if (collid.gameObject.tag == "lantern" && objectsWaterInContact > 0 && !isKnocked) {
            BeingHit();
            LooseHp(1);
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
        foreach (GameObject oreille in oreilles) {
            oreille.GetComponent<Renderer>().material.color = Color.white;
        }
        stop = true;
        delayToBump = timeToBump;
        canBump = false;
    }

    private void FollowTarget() {
        Physics.IgnoreCollision(target.GetComponent<Collider>(), myCollider, true);
        myRigidbody.constraints = RigidbodyConstraints.None;
        //follow target
        Vector3 relativePos = target.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        rotation.x = transform.rotation.x;
        rotation.z = transform.rotation.z;
        transform.rotation = rotation;

        float dis = Vector3.Distance(transform.position, target.transform.position);
        transform.Translate(0, 0, speed * Time.deltaTime);
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

    void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.tag == "Player") {
            startPosition = collision.gameObject.transform.position;
            Vector3 positionTargetForDirection = new Vector3(collision.gameObject.transform.position.x, 0, collision.gameObject.transform.position.z);
            Vector3 direction = (new Vector3(0, 0, 0) - positionTargetForDirection).normalized;
            endPosition = startPosition + direction * 40;
            doKnockBack = true;
            timeStamp = 0;
        } else if (collision.gameObject.tag == "Lure") {
            Destroy(target);
        }
    }

    
}
