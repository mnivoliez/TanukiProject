using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeCollision : MonoBehaviour {

    private Zone1BossBehavior bossBehavior;

	void Start () {
        bossBehavior = gameObject.GetComponentInParent<Zone1BossBehavior>();
    }
	

	void Update () {
        if (bossBehavior.GetIsKnocked()) {
            transform.Rotate(Vector3.right * 5);
        }
	}

    void OnTriggerEnter(Collider collid) {

        if (collid.gameObject.CompareTag("Leaf") && !bossBehavior.GetIsKnocked()) {
            float damage;
            if (collid.gameObject.GetComponent<MoveLeaf>() != null) {
                damage = collid.gameObject.GetComponent<MoveLeaf>().GetDamage();
            }
            else {
                damage = collid.gameObject.GetComponent<MeleeAttackTrigger>().GetDamage();
            }
            bossBehavior.BeingHit();
            bossBehavior.LooseHp(damage);
        }
    }
}
