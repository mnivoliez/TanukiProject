﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//================================================
//SOUNDCONTROLER
//================================================

public class BazekoriBehavior : YokaiController {

    [SerializeField] private float jumpForce = 300f;
    [SerializeField] private float attackRate = 0.2f;
    bool followPlayer = false;
    Vector3 startPosition;
    Vector3 endPosition;
    Vector3 bending = Vector3.up;
    float timeStamp = 0;
    [SerializeField] private GameObject hpCollectable;
    private Vector3 positionCollectable;
    //[SerializeField] private Renderer rendererBakezori;
    private Color initColor = new Color(202 / 255f, 54 / 255f, 255f / 255f, 0f);
    private Color hitColor = new Color(1f, 0, 0, 90f/255f);

    void Start() {
        target = GameObject.FindGameObjectWithTag("Player");
        rendererMat = GetComponent<Renderer>().material;
    }

    void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        transform.GetChild(0).transform.LookAt(target.transform);
        if (followPlayer) {
            MoveToPosition();
        }
        else {
            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
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
            if (!followPlayer) {
                if (Random.Range(0, 100) == 5) {
                    Behavior();
                }
            }
        }

    }

    public override void LooseHp(float damage) {
        hp = hp - damage;
        BeingHit();
        Invoke("EndHit", 0.3f);
        if (hp <= 0) {
            isKnocked = true;
            Vector3 posKnockedParticle = GetComponent<MeshRenderer>().bounds.max;
            posKnockedParticle.x = transform.position.x;
            posKnockedParticle.z = transform.position.z;
            Instantiate(knockedParticle, posKnockedParticle, Quaternion.identity).transform.parent = transform;
            //================================================
            SoundController.instance.SelectYOKAI("Scream");
            //================================================
        }

    }

    public override void BeingHit() {
        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
        //
        rendererMat.SetColor("_FirstDColor", hitColor);
        
    }

    public override void EndHit() {
        if (!isKnocked) rendererMat.SetColor("_FirstDColor", initColor);
        //rendererBakezori.material.SetColor("_FirstDColor", initColor);
    }

    public override void Absorbed() {
        isAbsorbed = true;
        gameObject.GetComponent<Collider>().enabled = false;
        positionCollectable = transform.position;
        positionCollectable.y = positionCollectable.y + 1;
        //================================================
        SoundController.instance.SelectYOKAI("Absorbed");
        //================================================
    }

    public override void Die() {
        if (Mathf.Abs(Vector3.Magnitude(transform.position) - Vector3.Magnitude(target.transform.position)) < 0.2) {
            if (Random.Range(0, 10) > 4.9f) {
                Instantiate(hpCollectable, positionCollectable, Quaternion.identity);
            }
            target.GetComponent<Animator>().SetBool("isAbsorbing", false);
            Destroy(gameObject);
        }
        else {
            if (transform.localScale.x < 0 && transform.localScale.y < 0 && transform.localScale.z < 0) {
                transform.localScale = Vector3.zero;
            }
            else {
                transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
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

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Leaf") && !isKnocked) {
            float damage;
            if (other.gameObject.GetComponent<MoveLeaf>() != null) {
                damage = other.gameObject.GetComponent<MoveLeaf>().GetDamage();
            }
            else {
                damage = other.gameObject.GetComponent<MeleeAttackTrigger>().GetDamage();
            }
            //================================================
            SoundController.instance.SelectYOKAI("Hurt");
            //================================================
            LooseHp(damage);
        }

    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Player") && !isKnocked) {
            col.gameObject.GetComponent<PlayerHealth>().LooseHP(damage);
            Destroy(Instantiate(hitParticle, col.gameObject.transform.position, Quaternion.identity), 1);
        }
    }


    public void MoveToPosition() {

        if (0 < attackRate - timeStamp) {
            Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, (timeStamp) / attackRate);
            currentPos.y += 1 * Mathf.Sin(Mathf.Clamp01((timeStamp) / attackRate) * Mathf.PI);
            transform.position = currentPos;
            timeStamp += Time.deltaTime;
        }
        else {
            followPlayer = false;
            timeStamp = 0;
        }
    }

    public void SetFollowPlayer(bool isFollowing) {
        followPlayer = isFollowing;
        timeStamp = 0;
        startPosition = transform.position;
        endPosition = target.transform.position + (Vector3.up * 2);
    }
}
