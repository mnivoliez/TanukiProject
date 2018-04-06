using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectNearInteractObject : MonoBehaviour {

    private float rangeNearestObject;
    private GameObject nearestObject;
    private float fieldOfView;

    // Use this for initialization
    void Start () {
        fieldOfView = 45f;
	}
	
	// Update is called once per frame
	void Update () {
		if (nearestObject != null
            && nearestObject.layer == LayerMask.NameToLayer("Catchable")
            && Vector3.Distance(nearestObject.gameObject.transform.position, transform.parent.position) > 4
            && Vector3.Angle(transform.forward, (nearestObject.gameObject.transform.position - transform.parent.position)) > fieldOfView) {
            nearestObject = null;
            rangeNearestObject = 0;
        }
	}

    void OnTriggerStay(Collider collider) {

        bool needDetectObject = ((collider.gameObject.layer == LayerMask.NameToLayer("Catchable")
            && Vector3.Distance(collider.gameObject.transform.position, transform.parent.position) < 4
            && Vector3.Angle(transform.forward, (collider.gameObject.transform.position - transform.parent.position)) <= fieldOfView)
            || collider.gameObject.layer == LayerMask.NameToLayer("Activable")
            || (collider.gameObject.CompareTag("Yokai") && collider.gameObject.GetComponent<YokaiController>().GetIsKnocked()));

        if (needDetectObject) {
            if (nearestObject == null || (nearestObject.name != null && rangeNearestObject > Vector3.Distance(collider.gameObject.transform.position, transform.parent.position))) {
                rangeNearestObject = Vector3.Distance(collider.gameObject.transform.position, transform.parent.position);
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

    /*private void OnGUI() {
        if (nearestObject != null) {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperCenter;
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 100, 200f, 200f), nearestObject.name, style);
        }
    }*/
}
