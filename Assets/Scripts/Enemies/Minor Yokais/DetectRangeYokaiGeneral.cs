using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectRangeYokaiGeneral : MonoBehaviour {

    private YokaiGeneralBehavior parentBehavior;
    private Collider areaCollider;

	// Use this for initialization
	void Start () {
        parentBehavior = GetComponentInParent<YokaiGeneralBehavior>();
        areaCollider = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
		if (parentBehavior.GetIsKnocked() && areaCollider.enabled == true) {
            areaCollider.enabled = false;
        }
	}

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player" && !parentBehavior.TooFarAway(other.transform.position)) {
            parentBehavior.SetTarget(other.gameObject);
            parentBehavior.SetComeBack(false);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            parentBehavior.SetTarget(null);
            parentBehavior.SetComeBack(true);
        }
    }
}
