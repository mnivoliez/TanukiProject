using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DetectRangeYokaiGeneral : MonoBehaviour {

    private YokaiGeneralBehavior parentBehavior;
    private Collider areaCollider;
    private List<GameObject> objectsInArea;

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
            //Debug.Log(parentBehavior.name + ": " +objectsInArea.Count);
            GameObject target = objectsInArea.Where(g => g.tag == "Lure" && !parentBehavior.TooFarAway(g.transform.position)).SingleOrDefault();

            if (target == null) {
                target = objectsInArea.Where(g => g.tag == "Player" && !parentBehavior.TooFarAway(g.transform.position)).SingleOrDefault();
            }
            
            parentBehavior.SetTarget(target);
            parentBehavior.SetComeBack(target == null);
        }
	}

    /*private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.tag + ": " + Vector3.Distance(parentBehavior.PositionOrigin, other.transform.position));
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Lure")) && !objectsInArea.Contains(other.gameObject) && !parentBehavior.TooFarAway(other.transform.position)) {
            objectsInArea.Add(other.gameObject);
        }

    }*/

    private void OnTriggerStay(Collider other) {
        
        if (!objectsInArea.Contains(other.gameObject)) {
            if (other.gameObject.CompareTag("Lure"))
                Debug.Log("new object: " + other.gameObject.tag);
            if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Lure")) && !parentBehavior.TooFarAway(other.transform.position)) {
                objectsInArea.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.tag);
    }

    private void OnTriggerExit(Collider other) {
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Lure")) && objectsInArea.Contains(other.gameObject)) {
            objectsInArea.Remove(other.gameObject);
        }
    }
}
