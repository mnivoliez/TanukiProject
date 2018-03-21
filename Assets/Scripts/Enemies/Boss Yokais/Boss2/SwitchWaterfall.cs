using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWaterfall : MonoBehaviour {

    private bool switchOn;
    private List<GameObject> objectsOnSwitch;

    [SerializeField] private List<GameObject> triggersCascade;
    [SerializeField] private List<GameObject> waterObjects;

	// Use this for initialization
	void Start () {
        switchOn = false;
        objectsOnSwitch = new List<GameObject>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        

        for (int i = 0; i < objectsOnSwitch.Count; i++) {
            GameObject obj = objectsOnSwitch[i];
            if (obj == null) {
                objectsOnSwitch.RemoveAt(i);
            }
        }

        if (objectsOnSwitch.Count == 0 && switchOn) {
            switchOn = false;
        } else if (objectsOnSwitch.Count > 0 && !switchOn) {
            switchOn = true;
        }

        if (switchOn) {
            foreach (GameObject waterObject in waterObjects) {
                if (!waterObject.activeSelf) {
                    waterObject.SetActive(true);
                }
            }

            foreach (GameObject triggerCascade in triggersCascade) {
                if (triggerCascade.transform.localPosition.y > -35) {
                    triggerCascade.transform.localPosition = new Vector3(triggerCascade.transform.localPosition.x, triggerCascade.transform.localPosition.y - 1, triggerCascade.transform.localPosition.z);
                }
            }
        } else {
            foreach (GameObject waterObject in waterObjects) {
                if (waterObject.activeSelf) {
                    waterObject.SetActive(false);
                }
            }

            foreach (GameObject triggerCascade in triggersCascade) {
                if (triggerCascade.transform.localPosition.y < 0) {
                    triggerCascade.transform.localPosition = new Vector3(triggerCascade.transform.localPosition.x, triggerCascade.transform.localPosition.y + 1, triggerCascade.transform.localPosition.z);
                }
            }
        }
	}

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Lure" || collision.gameObject.name == "Boss") {
            objectsOnSwitch.Add(collision.gameObject);
        }
    }

    void OnCollisionExit(Collision collision) {
        if (objectsOnSwitch.Contains(collision.gameObject)) {
            objectsOnSwitch.Remove(collision.gameObject);
        }
    }
}
