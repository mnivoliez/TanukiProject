using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbsorption : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider collid) {
        if (Input.GetButtonDown("Fire3")) {

            if (collid.gameObject.CompareTag("Yokai")) {
                collid.gameObject.GetComponent<YokaiController>().Absorbed();
                gameObject.GetComponent<PlayerCollectableController>().AddYokai();
            }
        }


    }
}
