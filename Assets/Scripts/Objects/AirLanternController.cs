using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLanternController : MonoBehaviour {
    [SerializeField]
    private float timeDestroy = 5.0f;
    private bool playerInAirStream = false;

    private void Start() {
        Invoke("DestroyAirStreamLantern", timeDestroy);
    }

    private void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        transform.TransformDirection(Vector3.up);
    }

    public void DestroyAirStreamLantern() {
        if (playerInAirStream) {
            GameObject.FindGameObjectWithTag("Player").GetComponent<KodaController>().PlayerOutAirstream();
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            playerInAirStream = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            playerInAirStream = false;
        }
    }
}
