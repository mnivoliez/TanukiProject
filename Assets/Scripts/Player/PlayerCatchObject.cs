using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatchObject : MonoBehaviour {

    [SerializeField]
    private GameObject leftHand;
    [SerializeField]
    private GameObject parentObjectInHand;
    private GameObject objectInHand;
    private float rangeNearestObject;
    private GameObject nearestObject;
    private PlayerController playerController;

    // Use this for initialization
    void Start() {
        objectInHand = null;
        nearestObject = null;
        rangeNearestObject = 0;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update() {

        if (parentObjectInHand.transform.childCount != 0) {
            playerController.CanDoubleJump = false;
        }

        if (Input.GetButtonDown("Fire3")) {
            if (parentObjectInHand.transform.childCount == 0 && nearestObject != null) {
                objectInHand = nearestObject;
                objectInHand.transform.parent = parentObjectInHand.transform;
                objectInHand.transform.position = parentObjectInHand.transform.position;
                Destroy(objectInHand.GetComponent<Rigidbody>());

            }
            else if (parentObjectInHand.transform.childCount != 0) {
                objectInHand.transform.parent = null;
                Rigidbody body = objectInHand.AddComponent(typeof(Rigidbody)) as Rigidbody;
                body.useGravity = true;
                body.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                objectInHand.GetComponent<Rigidbody>().useGravity = true;
                objectInHand = null;
                playerController.CanDoubleJump = true;
            }
        }
    }

    void OnTriggerStay(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Catchable")) {
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

    private void OnGUI() {
        if (nearestObject != null && objectInHand == null) {
            GUIStyle style = new GUIStyle();
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperCenter;
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 100, 200f, 200f), nearestObject.name, style);
        }
    }
}
