﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone1BossBehavior : YokaiController {

    [SerializeField] private float jumpForce = 300f;
    public GameObject spawnBoss;
    public GameObject platform1;
    public GameObject platform2;
    public GameObject platform3;
    public GameObject platform4;
    private GameObject[] AllPlatform;
    private float hpMax;
    private int phasePattern = 0;
    private int currentPlateform = 0;
    private bool changingPhase = false;
    [SerializeField] private GameObject arenaPhase1;
    [SerializeField] private GameObject arenaPhase2;

    bool onMovement = false;
    Vector3 startPosition;
    Vector3 endPosition;
    Vector3 bending = Vector3.up;
    float timeToTravel = 3f;
    float timeStamp = 0;

    [SerializeField] private float projectileDamage = 1f;
    [SerializeField] private float firerate = 5f;
    private float cooldownFire = 0;
    [SerializeField] private GameObject prefabProjectile;
    [SerializeField] private GameObject spawnProjectile;

    private float invincibleTime = 2f;
    private bool isInvincible = false;

    private bool followPlayer = false;
    private float cooldownPurchase;
    private float purchaseRate;

    void Start() {
        target = GameObject.FindGameObjectWithTag("Player");
        rendererMat = gameObject.GetComponent<Renderer>().material;
        hpMax = hp;
        AllPlatform = new GameObject[] { spawnBoss, platform1, platform2, platform3, platform4 };
    }

    void Update() {
        if (!isKnocked) {
            transform.GetChild(0).transform.LookAt(target.transform);

            if (onMovement) {
                MoveToPosition();
            }
            else if (followPlayer) {
                if (cooldownPurchase > purchaseRate) {
                    PurchasePlayer();

                }
            }
            else {
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
        if (!isKnocked) {
            cooldownFire += Time.deltaTime;
            if (followPlayer) { cooldownPurchase += Time.deltaTime; }
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
           
            if (hp <= 0) {
                isInvincible = false;
                isKnocked = true;
                Instantiate(knockedParticle, transform.position, Quaternion.identity).transform.parent = transform;
                rendererMat.color = new Color(150f / 255f, 40f / 255f, 150f / 255f);
            }
            else {
                isInvincible = true;
                invincibleTime = 3f;
                rendererMat.EnableKeyword("_EMISSION");
            }

            if (hp <= hpMax / 2 && !isKnocked) {
                if (!changingPhase) {
                    GameObject smokeParticleTransition = Instantiate(knockedParticle, arenaPhase1.transform.position, Quaternion.identity);
                    smokeParticleTransition.transform.GetChild(0).localScale = new Vector3(200f, 200f, 200f);
                    Destroy(smokeParticleTransition, 3f);
                    arenaPhase1.SetActive(false);
                    arenaPhase2.SetActive(true);
                    changingPhase = true;
                }
                
                ChangePlatform(1);
            }

            if (hp > hpMax / 2) {
                ChangePlatform(0);
            }
        }
    }

    public override void BeingHit() {
        Invoke("EndHit", 0.5f);
        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
        rendererMat.color = new Color(150f / 255f, 40f / 255f, 150f / 255f);
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
        timeToTravel = 3f;
        onMovement = true;
        GetComponent<Collider>().enabled = false;
        transform.GetChild(0).GetComponent<Collider>().enabled = false;
        Destroy(Instantiate(knockedParticle, transform.position, Quaternion.identity), 2);
    }

    public void MoveToPosition() {

        if (0 < timeToTravel - timeStamp) {
            Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, (timeStamp) / timeToTravel);
            currentPos.y += 20 * Mathf.Sin(Mathf.Clamp01((timeStamp) / timeToTravel) * Mathf.PI);
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
        projectile.transform.LookAt(target.transform);
        projectile.GetComponent<ProjectileBehavior>().SetDamage(projectileDamage);
        Destroy(projectile, 10f);

    }

    public void PurchasePlayer() {

        if (0 < timeToTravel - timeStamp) {
            Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, (timeStamp) / timeToTravel);
            currentPos.y += 20 * Mathf.Sin(Mathf.Clamp01((timeStamp) / timeToTravel) * Mathf.PI);
            transform.position = currentPos;
            timeStamp += Time.deltaTime;
        }
        else {
            followPlayer = false;
            timeStamp = 0;
            timeToTravel = 3f;
            cooldownPurchase = 0;
            cooldownFire = 0f;
        }
    }

    public void SetFollowPlayer(bool isFollowing) {
        followPlayer = isFollowing;
        timeStamp = 0;
        timeToTravel = 3f;
        startPosition = transform.position;
        endPosition = target.transform.position + (Vector3.up * 7);
    }

    private void OpenEye() {
        transform.GetChild(0).transform.localScale = Vector3.one;
    }

}
