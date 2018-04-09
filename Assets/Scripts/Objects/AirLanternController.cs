using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLanternController : MonoBehaviour {
    [SerializeField]
    private float timeDestroy = 5.0f;

    private void Start() {
        Destroy(gameObject, timeDestroy);
    }

    private void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        transform.TransformDirection(Vector3.up);
    }
}
