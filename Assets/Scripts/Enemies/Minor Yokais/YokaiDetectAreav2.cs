using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiDetectAreav2 : MonoBehaviour {

    YokaiController parent;

    private void Start() {
        parent = transform.parent.GetComponent<YokaiController>();
    }

    private void OnTriggerStay(Collider other) {



        if (!parent.GetComeBack()) {
            if (other.gameObject.tag == "Lure") {
                parent.SetTarget(other.gameObject);
            } else if (parent.GetTarget() == null && other.gameObject.tag == "Player") {
                parent.SetTarget(other.gameObject);
            }
        }
    }
}
