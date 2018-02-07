using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffect : MonoBehaviour {


    public float coeffRebond;

    private void OnCollisionExit(Collision collision) {
        Rigidbody entity = collision.gameObject.GetComponent<Rigidbody>();
        entity.velocity += Vector3.up * coeffRebond;
    }
}