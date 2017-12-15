using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackTrigger : MonoBehaviour {

    private float damage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collid) {

        if (collid.gameObject.CompareTag("Yokai") && !collid.gameObject.GetComponent<YokaiController>().GetIsKnocked()) {
            collid.gameObject.GetComponent<YokaiController>().BeingHit();
            collid.gameObject.GetComponent<YokaiController>().LooseHp(damage);
            
        }

    }

    void OnTriggerExit(Collider collid) {

        if (collid.gameObject.CompareTag("Yokai") && !collid.gameObject.GetComponent<YokaiController>().GetIsKnocked()) {
            collid.gameObject.GetComponent<YokaiController>().EndHit();
        }

    }

    public void SetDamage(float dmg) {
        damage = dmg;
    }

    public float GetDamage() {
        return damage;
    }
}
