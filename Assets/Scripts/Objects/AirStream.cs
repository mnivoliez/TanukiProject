using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStream : MonoBehaviour {



	void Start () {
		
	}
	
	void Update () {
		
	}

    private void OnTriggerStay(Collider other) {

        if (other.gameObject.CompareTag("Player")) {
            Rigidbody bodyObject = other.gameObject.GetComponent<Rigidbody>();
            
        }
        
    }



}
