using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRange : MonoBehaviour {
    private Zone1BossBehavior bossBehavior;
    private Collider collidTrigger;

    void Start () {
        bossBehavior = gameObject.GetComponentInParent<Zone1BossBehavior>();
        collidTrigger = GetComponent<Collider>();
        InvokeRepeating("ActivateCollider",1f,4f);
    }
	

	void Update () {
		
	}

    void ActivateCollider() {
        collidTrigger.enabled = true;
        Invoke("DesactivateCollider",0.2f);
    }
    void DesactivateCollider() {
        collidTrigger.enabled = false;
    }

    void OnTriggerEnter(Collider collid) {

        if (collid.gameObject.CompareTag("Player") && !bossBehavior.GetIsKnocked()) {
            bossBehavior.SetFollowPlayer(true);
        }
    }

    

}
