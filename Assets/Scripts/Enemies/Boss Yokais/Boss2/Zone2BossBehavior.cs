using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone2BossBehavior : YokaiController {

    private Rigidbody myRigidbody;
    private float nextThrow;
    private Transform spawnRock;
    private int phasePattern = 0;
    private bool interPhase;
    private List<GameObject> yokais;
    private float hpMax = 10;
    private bool doKnockBack;
    private float timeStamp;
    private Vector3 startPosition;
    private Vector3 endPosition;

    [SerializeField] private float timeToKnockBack;
    [SerializeField] private float throwRate = 1;
    [SerializeField] private GameObject prefabRock;

	// Use this for initialization
	void Start () {
        doKnockBack = false;
        phasePattern = 1;
        interPhase = false;
        nextThrow = 0;
        spawnRock = transform.Find("SpawnRock");
        SetTarget(GameObject.Find("Player"));
        rendererMat = gameObject.GetComponent<Renderer>().material;
        myRigidbody = GetComponent<Rigidbody>();
        myRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        hpMax = hp;

        yokais = new List<GameObject>();
        GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];

        for (var i = 0; i < gameObjects.Length; i++) {
            if (gameObjects[i].name.Contains("Yokai")) {
                yokais.Add(gameObjects[i]);
            }
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
            } else if (phasePattern == 1) {
                if (doKnockBack) {
                    KnockBack();
                } else {
                    transform.LookAt(target.transform);
                    if (Time.time >= nextThrow) {
                        ThrowRock();
                    }
                }
                
            } else if (phasePattern == 2) {

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
            
        }
    }

    void KnockBack () {
        if (0 < timeToKnockBack - timeStamp) {
            Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, (timeStamp) / timeToKnockBack);
            currentPos.y += 10 * Mathf.Sin(Mathf.Clamp01((timeStamp) / timeToKnockBack) * Mathf.PI);
            target.transform.position = currentPos;
            timeStamp += 0.1f;
        }
        else {
            doKnockBack = false;
        }
    }
}
