using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWaterfall : MonoBehaviour {

    private bool switchOn;
    private List<GameObject> objectsOnSwitch;
    [SerializeField] private float maxYRiver = -3;
    [SerializeField] private float minYRiver = -7;
    [SerializeField] private float speed = 5;

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

            foreach (GameObject triggerCascade in triggersCascade) {
                if (triggerCascade.transform.localPosition.y > 0) {
                    triggerCascade.transform.localPosition = new Vector3(triggerCascade.transform.localPosition.x, triggerCascade.transform.localPosition.y - 1, triggerCascade.transform.localPosition.z);
                }
            }

            if (!waterObjects[0].activeSelf) {
                waterObjects[0].SetActive(true);
            }

            if (waterObjects[1].transform.localPosition.y < maxYRiver) {
                waterObjects[1].transform.localPosition = new Vector3(waterObjects[1].transform.localPosition.x, waterObjects[1].transform.localPosition.y + speed * Time.fixedDeltaTime, waterObjects[1].transform.localPosition.z);
            }

        } else {

            foreach (GameObject triggerCascade in triggersCascade) {
                if (triggerCascade.transform.localPosition.y < ) {
                    triggerCascade.transform.localPosition = new Vector3(triggerCascade.transform.localPosition.x, triggerCascade.transform.localPosition.y + 1, triggerCascade.transform.localPosition.z);
                }
            }

            if (waterObjects[0].activeSelf) {
                waterObjects[0].SetActive(false);
            }

            if (waterObjects[1].transform.localPosition.y > minYRiver) {
                waterObjects[1].transform.localPosition = new Vector3(waterObjects[1].transform.localPosition.x, waterObjects[1].transform.localPosition.y - speed * Time.fixedDeltaTime, waterObjects[1].transform.localPosition.z);
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
