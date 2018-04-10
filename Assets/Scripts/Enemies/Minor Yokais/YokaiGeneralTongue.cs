using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiGeneralTongue : MonoBehaviour {

    [SerializeField]
    private float damageTongue = 1.0f;
    [SerializeField]
    private float rateTongueAttack = 2.0f;
    private float nextAttack = 0.0f;

    private bool tongueAttack = false;
    private GameObject target;

    private void FixedUpdate() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        if (tongueAttack) {
            if(Time.time > nextAttack) {
                nextAttack = Time.time + rateTongueAttack;
                target.GetComponent<PlayerHealth>().LooseHP(damageTongue);
                Debug.Log("tongue attack");
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Player") {
            tongueAttack = true;
            target = collision.gameObject;
        }
    }
    private void OnCollisionExit(Collision collision) {
        if(collision.gameObject.tag == "Player") {
            tongueAttack = false;
            target = null;
        }
    }
}
