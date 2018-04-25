using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFireflyBehavior : MonoBehaviour {


    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player")) {
            //================================================
            SoundController.instance.SelectFIREFLIES("Fireflies");
            //================================================
        }

    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            //================================================
            SoundController.instance.StopFirefliesEffect();
            //================================================
        }
    }
}
