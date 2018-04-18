using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiGeneralTong : MonoBehaviour {

    [SerializeField]
    private float damageTong = 1.0f;
    [SerializeField]
    private float rateTongAttack = 2.0f;
    private float nextAttack = 0.0f;

    private bool tongAttack = false;
    private GameObject target;

    private void FixedUpdate() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        if (tongAttack) {
            if(Time.time > nextAttack) {
                nextAttack = Time.time + rateTongAttack;
                target.GetComponent<PlayerHealth>().LooseHP(damageTong);
                Debug.Log("Tong attack");
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Player") {
            tongAttack = true;
            target = collision.gameObject;
        }
    }
    private void OnCollisionExit(Collision collision) {
        if(collision.gameObject.tag == "Player") {
            tongAttack = false;
            target = null;
        }
    }
}
