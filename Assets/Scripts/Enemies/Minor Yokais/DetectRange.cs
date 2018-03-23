using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectRange : MonoBehaviour {
    private BazekoriBehavior bazekoriBehavior;
    private Collider collidTrigger;
    [SerializeField] private float attackRate = 3f;

    void Start() {
        bazekoriBehavior = gameObject.GetComponentInParent<BazekoriBehavior>();
        collidTrigger = GetComponent<Collider>();
        InvokeRepeating("ActivateCollider", 1f, attackRate);
    }
    
    void Update() {
        if (bazekoriBehavior.GetIsKnocked()){
            CancelInvoke();
            DesactivateCollider();
        }
    }

    void ActivateCollider() {
        collidTrigger.enabled = true;
        Invoke("DesactivateCollider", 0.2f);
    }

    void DesactivateCollider() {
        collidTrigger.enabled = false;
    }

    void OnTriggerEnter(Collider collid) {

        if (collid.gameObject.CompareTag("Player") && !bazekoriBehavior.GetIsKnocked()) {
            bazekoriBehavior.SetFollowPlayer(true);
        }
    }
}
