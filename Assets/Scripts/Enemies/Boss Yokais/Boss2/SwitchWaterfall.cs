using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWaterfall : MonoBehaviour {

    private bool switchOn;
    private List<GameObject> objectsOnSwitch;

	// Use this for initialization
	void Start () {
        switchOn = false;
        objectsOnSwitch = new List<GameObject>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Yokai") {
            objectsOnSwitch.Add(collision.gameObject);
            switchOn = true;
        }
    }

    void OnCollisionExit(Collision collision) {
        if (objectsOnSwitch.Contains(collision.gameObject)) {
            objectsOnSwitch.Remove(collision.gameObject);
            if (objectsOnSwitch.Count == 0) {
                switchOn = false;
            }
        }
    }
}
