using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//================================================
//SOUNDCONTROLER
//================================================

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
    private bool isActivatePlate = false;
    private GameObject interactPlate;

    Rigidbody body;

    private void Start() {
        gravity = -gravityGlobal * gravityScale * Vector3.up;
        //================================================
        SoundController.instance.SelectLEAF("LureVanish");
        //================================================
    }

    private void OnEnable() {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
    }

    private void FixedUpdate() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        body.AddForce(gravity * (40 * Time.deltaTime), ForceMode.Acceleration);
    }

    public void BeingHit() {
        health--;

        if (health <= 0) {
            if (isActivatePlate) {
                interactPlate.GetComponent<PressPlateSwitch>().UnpressPlate();
                isActivatePlate = false;
                interactPlate = null;
            }
            GameObject smokeSpawn = Instantiate(prefabSpawnEffect, transform.position, Quaternion.identity);
            smokeSpawn.transform.localScale = Vector3.one * 0.5f;
            Destroy(smokeSpawn, 2f);

            GameObject.FindGameObjectWithTag("Player").GetComponent<KodaController>().ResetLeafLock();
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collider) {
        /*if (collider.gameObject.tag == "Yokai")
        {
            BeingHit();
        }
        else */

        if(collider.gameObject.layer == LayerMask.NameToLayer("Ground") || collider.gameObject.layer == LayerMask.NameToLayer("Rock")) {
            //================================================
            SoundController.instance.SelectLEAF("LureGround");
            //================================================
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("Water")) {
            DestroyLure();
        }
        if (collider.gameObject.CompareTag("Plate")) {
            isActivatePlate = true;
            interactPlate = collider.gameObject;
        }
    }

    public void DestroyLure() {
        //================================================
        SoundController.instance.SelectLEAF("LureVanish");
        //================================================
        if (isActivatePlate) {
            interactPlate.GetComponent<PressPlateSwitch>().UnpressPlate();
            isActivatePlate = false;
            interactPlate = null;
        }

        GameObject.FindGameObjectWithTag("Player").GetComponent<KodaController>().ResetLeafLock();
        Destroy(gameObject);
    }
}
