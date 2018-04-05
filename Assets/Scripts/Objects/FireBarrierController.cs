using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireBarrierController : MonoBehaviour {

    [SerializeField] private int nbYokaiNeeded = 1;
    [SerializeField] private Text nbYokaiNeededText;
    bool disappear = false;
    Renderer renderBarrier;
    float counter = 0.8f;

    void Start () {
        nbYokaiNeededText.text = ""+nbYokaiNeeded;
        renderBarrier = gameObject.transform.parent.GetComponent<Renderer>();
    }
	

	void Update () {

        if (disappear) {
            counter += Time.deltaTime;
            renderBarrier.material.SetFloat("_Height", counter);

            if(counter > 2) {
                Destroy(gameObject.transform.parent.gameObject);
            }
        }
		
	}

    void OnTriggerEnter(Collider collid) {
        if (collid.gameObject.CompareTag("Player")) {

            if(collid.gameObject.GetComponent<PlayerCollectableController>().GetnbYokai() >= nbYokaiNeeded) {
                disappear = true;
                nbYokaiNeededText.text = " ";
            }

        }
    }

}
