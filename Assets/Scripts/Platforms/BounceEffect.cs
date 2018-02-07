using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffect : MonoBehaviour {


    public float coeffRebond;


    private void OnCollisionEnter(Collision collision) {
        Rigidbody Koda = collision.gameObject.GetComponent<Rigidbody>();
        Vector3 Koda_old_velocity = Koda.velocity;
        ContactPoint contact = collision.contacts[0];

        Koda.AddForce(-collision.contacts[0].normal * 200, ForceMode.Impulse); // 200 = coeffRebond

        //Debug.Log("BOING");
    }

    /*private void OnCollisionExit(Collision collision) {
        Rigidbody Koda = collision.gameObject.GetComponent<Rigidbody>();
        Koda.velocity += Vector3.up * coeffRebond;
    }*/
}