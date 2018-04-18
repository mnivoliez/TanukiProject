using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KillZone : MonoBehaviour {

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Player")) {
            col.gameObject.GetComponent<PlayerHealth>().PlayerDie();
        }
    }

}
