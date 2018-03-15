using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiDetectAreav2 : MonoBehaviour {

    YokaiAIv2Controller parent;

    private void Start() {
        parent = transform.parent.GetComponent<YokaiAIv2Controller>();
    }

    private void OnTriggerStay(Collider other) {

        if (!parent.comeBack) {
            if (other.gameObject.tag == "Lure") {
                Debug.Log(other.gameObject.tag);
                parent.SetTarget(other.gameObject);
            } else if (parent.GetTarget() == null && other.gameObject.tag == "Player") {
                parent.SetTarget(other.gameObject);
            }
        }
    }
}
