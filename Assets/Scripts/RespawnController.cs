using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour {

    public GameObject respawnPoint;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnCollisionEnter(Collision col) {

        if (col.gameObject.CompareTag("Player")) {
            col.gameObject.transform.SetPositionAndRotation(respawnPoint.transform.position, Quaternion.identity);
        }


    }
}
