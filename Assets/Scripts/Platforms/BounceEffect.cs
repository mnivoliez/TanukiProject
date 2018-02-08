using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffect : MonoBehaviour {


    [SerializeField] private int coeffRebond = 200;


    private void OnCollisionEnter(Collision collision) {
        Rigidbody Koda = collision.gameObject.GetComponent<Rigidbody>();
        Vector3 Koda_old_velocity = Koda.velocity;
        ContactPoint contact = collision.contacts[0];

        Koda.AddForce(-collision.contacts[0].normal * coeffRebond, ForceMode.Impulse); // 200 = coeffRebond

        //Debug.Log("BOING");
    }

    /*private void OnCollisionExit(Collision collision) {
        Rigidbody Koda = collision.gameObject.GetComponent<Rigidbody>();
        Koda.velocity += Vector3.up * coeffRebond;
    }*/
}