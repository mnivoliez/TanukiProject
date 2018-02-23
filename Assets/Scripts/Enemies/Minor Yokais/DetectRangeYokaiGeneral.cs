using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DetectRangeYokaiGeneral : MonoBehaviour {

    private YokaiGeneralBehavior parentBehavior;
    private Collider areaCollider;
    [SerializeField]
    private List<GameObject> objectsInArea;

    private 

	// Use this for initialization
	void Start () {
        parentBehavior = GetComponentInParent<YokaiGeneralBehavior>();
        areaCollider = GetComponent<Collider>();
        objectsInArea = new List<GameObject>();

    }
	
	// Update is called once per frame
	void Update () {
		if (parentBehavior.GetIsKnocked()) {
            if (areaCollider.enabled == true) {
                areaCollider.enabled = false;
            }
        } else {
            GameObject target = null;
            string message = "";
            for (int i = 0; i < objectsInArea.Count; i++) {
                GameObject obj = objectsInArea[i];
                if (obj != null) {
                    message += obj.name + ",";
                
                    if (obj.tag == "Lure") {
                        target = obj;
                    } else if (obj.tag == "Player" && target == null) {
                        target = obj;
                    }
                } else {
                    objectsInArea.RemoveAt(i);
                }
                
            }

            parentBehavior.SetTarget(target);
        }
	}

    private void OnTriggerStay(Collider other) {
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Lure")))
        {
            if (!objectsInArea.Contains(other.gameObject)) {
                if (!parentBehavior.TooFarAway(other.transform.position) && !parentBehavior.TooFarAway()) {
                    objectsInArea.Add(other.gameObject);
                }
            } else {
                if (parentBehavior.TooFarAway(other.transform.position) && parentBehavior.TooFarAway()) {
                    objectsInArea.Remove(other.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Lure")) && objectsInArea.Contains(other.gameObject)) {
            objectsInArea.Remove(other.gameObject);
        }
    }
}
