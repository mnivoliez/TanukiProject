using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffect : MonoBehaviour {

   
    public int forceRebond = 30;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter(Collision collision) {

        Rigidbody entity = collision.gameObject.GetComponent<Rigidbody>();
        entity.velocity = Vector3.zero;
        if (collision.gameObject.name == "Player") {
            //collision.gameObject.GetComponent<PlayerController>().SetIsJumping(true);
            Debug.Log("BOING");
        }
        entity.AddForce(Vector3.up * forceRebond, ForceMode.VelocityChange);
    }
}
