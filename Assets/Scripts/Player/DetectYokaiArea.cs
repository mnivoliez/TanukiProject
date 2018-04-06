using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectYokaiArea : MonoBehaviour {

    private InteractBehavior interBehavior;

	void Start () {
        interBehavior = GameObject.FindGameObjectWithTag("Player").GetComponent<InteractBehavior>();
        GetComponent<SphereCollider>().radius = interBehavior.GetDistantAttackRange();

    }


    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Yokai")) {
            interBehavior.AddYokaiInRange(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Yokai")) {
            interBehavior.RemoveYokaiInRange(other.gameObject);
        }
    }

}
