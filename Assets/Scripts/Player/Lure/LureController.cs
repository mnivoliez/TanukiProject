using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LureController : MonoBehaviour {

    [SerializeField]
    private float gravityScale = 1.0f;
    [SerializeField]
    private float gravityGlobal = 9.81f;
    [SerializeField]
    private float forceRebound = 2.0f;

    private Vector3 gravity;

    Rigidbody body;

    private void Start() {
        gravity = -gravityGlobal * gravityScale * Vector3.up;
    }

    private void OnEnable() {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
    }

    private void FixedUpdate() {
        body.AddForce(gravity, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Yokai") {
            body.AddForce(-gravity * forceRebound, ForceMode.Acceleration);
            Destroy(collision.gameObject);
        }
    }
}
