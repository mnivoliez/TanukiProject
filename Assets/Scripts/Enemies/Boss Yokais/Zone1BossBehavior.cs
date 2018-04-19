using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//================================================
//SOUNDCONTROLER
//================================================

public class Zone1BossBehavior : YokaiController {

    [SerializeField] private float jumpForce = 300f;
    [SerializeField] private GameObject spawnBoss;
    [SerializeField] private GameObject platform1;
    [SerializeField] private GameObject platform2;
    [SerializeField] private GameObject platform3;
    [SerializeField] private GameObject platform4;
    private GameObject[] AllPlatform;
    private float hpMax;
    private int phasePattern = 0;
    private int currentPlateform = 0;
    private bool changingPhase = false;
    [SerializeField] private GameObject arenaPhase1;
    [SerializeField] private GameObject arenaPhase2;
    [SerializeField] private GameObject arena1;
    [SerializeField] private GameObject arena2;
    [SerializeField] private GameObject arena3;
    [SerializeField] private GameObject arena4;

    [SerializeField] private PlayableDirector playableDirector;

    bool onMovement = false;
    Vector3 startPosition;
    Vector3 endPosition;
    Vector3 bending = Vector3.up;
    float timeToTravelPlatform = 3f;
    float timeToJump = 2f;
    float timeStamp = 0;

    [SerializeField] private float projectileDamage = 1f;
    [SerializeField] private float firerate = 5f;
    private float cooldownFire = 0;
    [SerializeField] private GameObject prefabProjectile;
    [SerializeField] private GameObject spawnProjectile;

    private float invincibleTime = 2f;
    private bool isInvincible = false;

    private bool followPlayer = false;
    private float cooldownPurchase = 0;
    [SerializeField] private float purchaseRate = 4f;
    private bool inAir;
    private DetectionRange detectArea;
    private Color initialColor = new Color(215f/255f, 127f / 255f, 240f / 255f);
    private Color hitColor = new Color(255f / 255f, 40f / 255f, 150f / 255f);
    private Color chargeColor = new Color(255f / 255f, 70f / 255f, 70f / 255f);

    void Start() {

        target = GameObject.FindGameObjectWithTag("Player");
        rendererMat = gameObject.GetComponent<Renderer>().material;
        hpMax = hp;
        AllPlatform = new GameObject[] { spawnBoss, platform1, platform2, platform3, platform4 };
        detectArea = GetComponentInChildren<DetectionRange>();
    }

    void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        if (!isKnocked) {
            transform.GetChild(0).transform.LookAt(target.transform);

            if (onMovement) {
                MoveToPosition();
            }
            else if (followPlayer) {
                if (cooldownPurchase > purchaseRate) {
                    if (!inAir) {
                        startPosition = transform.position;
                        endPosition = target.transform.position + (Vector3.up * 7);
                        inAir = true;
                    }
                    PurchasePlayer();
                }
                else {
                    transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                    cooldownPurchase += Time.deltaTime;
                    if (cooldownPurchase > purchaseRate/2) {
                        rendererMat.color = chargeColor;
                        //rendererMat.SetColor("_Globalcolor", chargeColor);
                    }
                    
                }
            }
            else {
                cooldownFire += Time.deltaTime;
                transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
                if (cooldownFire > firerate) {
                    transform.GetChild(0).transform.localScale = new Vector3(1f, 0.1f, 0.5f);
                    Invoke("OpenEye", 0.2f);
                    AttackTarget();
                    cooldownFire = 0;
                }

            }
        }

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
            if (Random.Range(0, 100) == 5) {
                Behavior();
            }
            if (invincibleTime > 0) {
                invincibleTime -= Time.deltaTime;
            }
            else {
                rendererMat.DisableKeyword("_EMISSION");
                isInvincible = false;
            }
        }
        
    }

    public override void LooseHp(float damage) {

        if (!onMovement || !isInvincible) {
            hp -= damage;

            switch ((int)hp) {

                case 7:
                    arena1.SetActive(true);
                    break;

                case 6:
                    arena2.SetActive(true);
                    break;

                case 5:
                    arena3.SetActive(true);
                    break;

                case 4:
                    arena4.SetActive(true);
                    break;

            }

            if (hp <= 0) {
                isInvincible = false;
                isKnocked = true;
                Instantiate(knockedParticle, transform.position, Quaternion.identity).transform.parent = transform;
                rendererMat.color = hitColor;
                //================================================
                SoundController.instance.SelectYOKAI("KO");
                //================================================
                //rendererMat.SetColor("_Globalcolor", hitColor);
            }
            else {
                isInvincible = true;
                invincibleTime = 3f;
                //rendererMat.EnableKeyword("_EMISSION");
            }

            if (hp < 4 && !isKnocked) {
                if (!changingPhase) {
                    playableDirector.Play();
                    GameObject smokeParticleTransition = Instantiate(knockedParticle, arenaPhase1.transform.position, Quaternion.identity);
                    smokeParticleTransition.transform.GetChild(0).localScale = new Vector3(200f, 200f, 200f);
                    Destroy(smokeParticleTransition, 3f);
                    arenaPhase1.SetActive(false);
                    arenaPhase2.SetActive(true);
                    changingPhase = true;
                }
                
                ChangePlatform(1);
            }

            if (hp >= hpMax / 2) {
                ChangePlatform(0);
            }
        }
    }

    public override void BeingHit() {
        Invoke("EndHit", 0.5f);
        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
        rendererMat.color = hitColor;
        //rendererMat.SetColor("_Globalcolor", hitColor);
    }

    public override void EndHit() {
        //if (!isKnocked) rendererMat.SetColor("_Globalcolor", initialColor);
        if (!isKnocked) rendererMat.color = Color.white;
    }

    public override void Absorbed() {
        isAbsorbed = true;
        gameObject.GetComponent<Collider>().enabled = false;
        //================================================
        SoundController.instance.SelectYOKAI("Absorbed");
        //================================================
        Game.playerData.lightBoss1 = true;
        Game.PreSave_Game_and_Save();
    }

    public override void Die() {
        if (Mathf.Abs(Vector3.Magnitude(transform.position) - Vector3.Magnitude(target.transform.position)) < 0.2) {
            target.GetComponent<Animator>().SetBool("isAbsorbing", false);
            GameObject.Find("VictoryTrigger").GetComponent<VictorySwitch>().VictoryScreen();
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

    public override void Behavior() {
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void ChangePlatform(int numPhase) {

        startPosition = transform.position;

        if (numPhase == 0) {
            if (currentPlateform == 4) {
                currentPlateform = 0;
            }
            else {
                currentPlateform++;
            }
        }
        else {
            int nextPlatform = Random.Range(0, 5);

            while (nextPlatform == currentPlateform) {
                nextPlatform = Random.Range(0, 5);
            }

            currentPlateform = nextPlatform;
        }

        endPosition = AllPlatform[currentPlateform].transform.position;
        transform.LookAt(new Vector3(AllPlatform[currentPlateform].transform.position.x, transform.position.y, AllPlatform[currentPlateform].transform.position.z));
        timeStamp = 0;
        timeToTravelPlatform = 2f;
        onMovement = true;
        GetComponent<Collider>().enabled = false;
        transform.GetChild(0).GetComponent<Collider>().enabled = false;
        Destroy(Instantiate(knockedParticle, transform.position, Quaternion.identity), 2);
    }

    public void MoveToPosition() {

        if (0 < timeToTravelPlatform - timeStamp) {
            Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, (timeStamp) / timeToTravelPlatform);
            currentPos.y += 20 * Mathf.Sin(Mathf.Clamp01((timeStamp) / timeToTravelPlatform) * Mathf.PI);
            transform.position = currentPos;
            timeStamp += Time.deltaTime;
        }
        else {
            GetComponent<Collider>().enabled = true;
            transform.GetChild(0).GetComponent<Collider>().enabled = true;
            onMovement = false;
        }
    }

    public void AttackTarget() {

        GameObject projectile = Instantiate(prefabProjectile, spawnProjectile.transform.position, Quaternion.identity);
        Vector3 targetPos = new Vector3(target.transform.position.x, target.transform.position.y+1, target.transform.position.z);
        projectile.transform.LookAt(targetPos);
        projectile.GetComponent<ProjectileBehavior>().SetDamage(projectileDamage);
        Destroy(projectile, 10f);

    }

    public void PurchasePlayer() {

        if (0 < timeToJump - timeStamp) {
            Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, (timeStamp) / timeToJump);
            currentPos.y += 10 * Mathf.Sin(Mathf.Clamp01((timeStamp) / timeToJump) * Mathf.PI);
            transform.position = currentPos;
            timeStamp += Time.deltaTime;
        }
        else {
            followPlayer = false;
            timeStamp = 0;
            timeToJump = 2f;
            cooldownPurchase = 0;
            cooldownFire = 0f;
            inAir = false;
            rendererMat.color = Color.white;
            //rendererMat.SetColor("_Globalcolor", initialColor);
            detectArea.ActivateCollider();
        }
    }

    public void SetFollowPlayer(bool isFollowing) {
        followPlayer = isFollowing;
        timeStamp = 0;
        timeToJump = 2f;
        cooldownFire = 0f;
        startPosition = transform.position;
        endPosition = target.transform.position + (Vector3.up * 7);
    }

    private void OpenEye() {
        transform.GetChild(0).transform.localScale = Vector3.one;
    }

}
