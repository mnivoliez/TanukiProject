using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceEffect : MonoBehaviour {

    [SerializeField]
    private int forceRebond = 30;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter(Collision collision) {
        GameObject player = collision.gameObject;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        if (player.name == "Player") {
            player.GetComponent<PlayerController>().SetIsJumping(true);
        }
        player.GetComponent<Rigidbody>().AddForce(Vector3.up * forceRebond, ForceMode.VelocityChange);
    }
}
