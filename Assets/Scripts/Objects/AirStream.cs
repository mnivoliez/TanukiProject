using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStream : MonoBehaviour {

    [SerializeField] private float airForce = 100f;

	void Start () {
		
	}
	
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (other.gameObject.GetComponent<CharacterController>().GetInteractState() == InteractState.Glide) {
                Rigidbody bodyObject = other.GetComponent<Rigidbody>();
                bodyObject.velocity = new Vector3(bodyObject.velocity.x, 0, bodyObject.velocity.z);
				bodyObject.AddForce(Vector3.up * 80, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerStay(Collider other) {
		if (other.CompareTag("Player")) {
            if (other.GetComponent<CharacterController>().GetInteractState() == InteractState.Glide) {
                Rigidbody bodyObject = other.GetComponent<Rigidbody>();
                bodyObject.AddForce(Vector3.up * airForce + (Vector3.up * Mathf.Abs(bodyObject.velocity.y)), ForceMode.Force);
            }
        }
    }
}
