using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFireflyBehavior : MonoBehaviour {


    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player")) {
            //Joue le son des lucioles
        }

    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            //Coupe le son en fade out
        }
    }
}
