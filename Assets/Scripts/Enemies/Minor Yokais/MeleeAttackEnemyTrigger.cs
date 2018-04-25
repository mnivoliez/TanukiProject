using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackEnemyTrigger : MonoBehaviour {

    private float damage;

    /*void OnTriggerEnter(Collider collid) {

        //if (collid.gameObject.CompareTag("Yokai") && !collid.gameObject.GetComponent<YokaiController>().GetIsKnocked()) {
        //    collid.gameObject.GetComponent<YokaiController>().BeingHit();
        //    collid.gameObject.GetComponent<YokaiController>().LooseHp(damage);

        //}

    }

    void OnTriggerExit(Collider collid) {

        //if (collid.gameObject.CompareTag("Yokai") && !collid.gameObject.GetComponent<YokaiController>().GetIsKnocked()) {
        //    collid.gameObject.GetComponent<YokaiController>().EndHit();
        //}

    }*/

    void OnTriggerStay(Collider collid) {

        if (collid.gameObject.CompareTag("Player")) {
            collid.gameObject.GetComponent<PlayerHealth>().LooseHP(damage);
        }

    }

    public void SetDamage(float dmg) {
        damage = dmg;
    }

    public float GetDamage() {
        return damage;
    }
}
