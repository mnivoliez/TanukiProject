using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRange : MonoBehaviour {
    private Zone1BossBehavior bossBehavior;
    private Collider collidTrigger;

    void Start () {
        bossBehavior = gameObject.GetComponentInParent<Zone1BossBehavior>();
        collidTrigger = GetComponent<Collider>();
    }

    public void ActivateCollider() {
        collidTrigger.enabled = true;
    }

    void OnTriggerEnter(Collider collid) {

        if (collid.gameObject.CompareTag("Player") && !bossBehavior.GetIsKnocked()) {
            bossBehavior.SetFollowPlayer(true);
            collidTrigger.enabled = false;
        }
    }

}
