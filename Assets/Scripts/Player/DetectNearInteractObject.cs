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
    [SerializeField]
    private float prioritizationRange = 0.2f;
    [SerializeField]
    private float offSet = 0f;

    [SerializeField]
    private Transform direction;

	// Update is called once per frame
	void Update () {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        if (nearestObject != null) {
            float distanceObject = Vector3.Distance(nearestObject.transform.position, direction.position);
            Vector3 offSetPoint = direction.position - direction.forward * offSet;
            float angleObject = Vector3.Angle(direction.forward, nearestObject.transform.position - offSetPoint);
            bool isInCone = distanceObject < rangeInteract && angleObject < fieldOfView;
            if(!isInCone) {
                nearestObject = null;
                rangeNearestObject = 0;
            } 
        }
	}

    void OnTriggerStay(Collider collider) {
        
        float distanceObject = Vector3.Distance(collider.gameObject.transform.position, direction.position);
        Vector3 offSetPoint = direction.position - direction.forward * offSet;
        float angleObject = Vector3.Angle(direction.forward, (collider.gameObject.transform.position - offSetPoint));
        bool isInCone = distanceObject < rangeInteract && angleObject < fieldOfView;
                    
        bool newObjectIsCatchable = collider.gameObject.layer == LayerMask.NameToLayer("Catchable");
        bool newObjectIsActivable = collider.gameObject.layer == LayerMask.NameToLayer("Activable");
        bool newObjectIsAbsorbable = collider.gameObject.CompareTag("Yokai") && collider.gameObject.GetComponent<YokaiController>().GetIsKnocked();

        bool needDetectObject =  isInCone && (newObjectIsCatchable || newObjectIsActivable || newObjectIsAbsorbable);

        if (needDetectObject) {
            Debug.Log(collider.gameObject.layer + " " + LayerMask.LayerToName(collider.gameObject.layer));
            if (nearestObject == null) {
                rangeNearestObject = distanceObject;
                nearestObject = collider.gameObject;
            } else  {
                //the new object should be prioritize if he is catchable and the previous object is not. The prioritization should work only in a short range.

                // if the range is positive, the new object is closer, else it's more distant.
                float distanceDiff = rangeNearestObject - distanceObject;

                // if it is within the prioritize zone, we will check the prioritized status of the new object over the stored one.
                bool withinPrioritizationRange = System.Math.Abs(distanceDiff) < prioritizationRange;
                if(withinPrioritizationRange && newObjectIsCatchable) {
                    rangeNearestObject = distanceObject;
                    nearestObject = collider.gameObject;
                } else if( distanceDiff > 0) {
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
