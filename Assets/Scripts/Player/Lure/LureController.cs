using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LureController : MonoBehaviour {

    [SerializeField]
    private float gravityScale = 1.0f;
    [SerializeField]
    private float gravityGlobal = 9.81f;
    [SerializeField]
    private float forceRebound = 2.0f;
    [SerializeField]
    private int health = 3;
    [SerializeField] private GameObject prefabSpawnEffect;

    private Vector3 gravity;

    Rigidbody body;

    private void Start() {
        

        gravity = -gravityGlobal * gravityScale * Vector3.up;
    }

    private void OnEnable() {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
    }

    private void FixedUpdate() {
        body.AddForce(gravity * (40 * Time.deltaTime), ForceMode.Acceleration);
    }

    private void BeingHit() {
        health--;
        
        if (health <= 0) {
            GameObject smokeSpawn = Instantiate(prefabSpawnEffect, transform.position, Quaternion.identity);
            smokeSpawn.transform.localScale = Vector3.one * 0.5f;
            Destroy(gameObject);
            GameObject.FindGameObjectWithTag("Player").GetComponent<KodaController>().ResetLeafLock();
        }
    }

    void OnCollisionEnter(Collision collider) {
        if (collider.gameObject.tag == "Yokai") {
            BeingHit();
        }
    }
}
