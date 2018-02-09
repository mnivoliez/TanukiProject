using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectNearInteractObject : MonoBehaviour {

    private float rangeNearestObject;
    private GameObject nearestObject;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider collider) {

        bool needDetectObject = (collider.gameObject.layer == LayerMask.NameToLayer("Catchable") || collider.gameObject.layer == LayerMask.NameToLayer("Activable")
            || (collider.gameObject.CompareTag("Yokai") && collider.gameObject.GetComponent<YokaiController>().GetIsKnocked()));

        if (needDetectObject) {
            if (nearestObject == null || (nearestObject.name != null && rangeNearestObject > Vector3.Distance(collider.gameObject.transform.position, transform.position))) {
                rangeNearestObject = Vector3.Distance(collider.gameObject.transform.position, transform.position);
                nearestObject = collider.gameObject;
            }
        }

    }

    void OnTriggerExit(Collider collider) {
        if (collider.gameObject == nearestObject) {
            nearestObject = null;
            rangeNearestObject = 0;
        }
    }

    public GameObject GetNearestObject() {
        return nearestObject;
    }

    private void OnGUI() {
        if (nearestObject != null) {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperCenter;
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 100, 200f, 200f), nearestObject.name, style);
        }
    }
}
