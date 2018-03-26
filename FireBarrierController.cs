using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBarrierController : MonoBehaviour {

    [SerializeField] private int nbYokaiNeeded;

	void Start () {
		
	}
	

	void Update () {
		
	}

    void OnTriggerEnter(Collider collid) {
        if (collid.gameObject.CompareTag("Player")) {

            if(collid.gameObject.GetComponent<PlayerCollectableController>().GetnbYokai() >= nbYokaiNeeded) {


            }

        }
    }

}
