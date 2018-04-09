using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectNearInteractObject : MonoBehaviour {

    private float rangeNearestObject;
    private GameObject nearestObject;

    [SerializeField]
    private float fieldOfView = 45f;
    [SerializeField]
    private float rangeInteract = 4f;
    private float offSet = 0.5f;

    [SerializeField]
    private Transform direction;

	// Update is called once per frame
	void Update () {
		if (nearestObject != null
            && nearestObject.layer == LayerMask.NameToLayer("Catchable")
            && Vector3.Distance(nearestObject.gameObject.transform.position, transform.position) > 4
            && Vector3.Angle(direction.forward, (nearestObject.gameObject.transform.position - transform.position)) > fieldOfView) {
            nearestObject = null;
            rangeNearestObject = 0;
        }
	}

    void OnTriggerStay(Collider collider) {
		if (collider.gameObject.tag == "Catchable") {
			Debug.Log ("Detect: " + collider.name);
		}


        float distanceObject = Vector3.Distance(collider.gameObject.transform.position, transform.position);
        Vector3 offSetPoint = transform.position + transform.forward * -offSet;
        float angleObject = Vector3.Angle(direction.forward, (collider.gameObject.transform.position - offSetPoint));
        bool isInCone = distanceObject < rangeInteract && angleObject < fieldOfView;

        bool needDetectObject =  isInCone && (collider.gameObject.layer == LayerMask.NameToLayer("Catchable")
            || collider.gameObject.layer == LayerMask.NameToLayer("Activable")
            || (collider.gameObject.CompareTag("Yokai") && collider.gameObject.GetComponent<YokaiController>().GetIsKnocked()) );

        if (needDetectObject) {

            if (nearestObject == null) {
                rangeNearestObject = distanceObject;
                nearestObject = collider.gameObject;
            } else  {
                bool isNearest =  rangeNearestObject > distanceObject;
                bool isPrioritize = nearestObject.layer != LayerMask.NameToLayer("Catchable") && collider.gameObject.layer == LayerMask.NameToLayer("Catchable");
                if (nearestObject.name != null && (isNearest || isPrioritize)) {
                    rangeNearestObject = distanceObject;
                    nearestObject = collider.gameObject;
                }
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
