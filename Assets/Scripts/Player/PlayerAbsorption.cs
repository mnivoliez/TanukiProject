using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbsorption : MonoBehaviour {

    private Animator animBody;
    public GameObject sakePot;

    void Start () {
        animBody = GetComponent<Animator>();
        sakePot.SetActive(false);
    }
	
	void Update () {
		
	}

    void OnTriggerStay(Collider collid) {
        if (Input.GetButtonDown("Fire3")) {

            if (collid.gameObject.CompareTag("Yokai")) {
                animBody.SetBool("isAbsorbing", true);
                sakePot.SetActive(true);

                collid.gameObject.GetComponent<YokaiController>().Absorbed();
                gameObject.GetComponent<PlayerCollectableController>().AddYokai();
            }
        }


    }
}
