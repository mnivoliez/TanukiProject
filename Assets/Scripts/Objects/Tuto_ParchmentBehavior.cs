using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto_ParchmentBehavior : MonoBehaviour {

    bool isActive;


	void Start () {
		
	}
	
	void Update () {

        if (isActive) {
            if (Input.GetButtonDown("Jump")) {
                transform.GetChild(0).gameObject.SetActive(false);
                GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().SetFreezeInput(false);
                Destroy(gameObject);
            }
        }
		
	}

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")){

            isActive = true;
            transform.GetChild(0).gameObject.SetActive(true);
            other.gameObject.GetComponent<InputController>().SetFreezeInput(true);

        }
    }


}
