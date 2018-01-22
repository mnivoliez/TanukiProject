using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBallForm : MonoBehaviour {


    //Camera Follow Smooth
    public GameObject target; // The target in which to follow
    public Vector3 CameraOffset; // This will allow us to offset the camera for the player's view.
    public bool useOffsetValues;
    private Transform positionTarget;

    void Start() {
        //CameraOffset = new Vector3(0,5,-10);
        //positionTarget = new Transform();
    }

    void FixedUpdate() {
        if (target != null) {

            transform.position = Vector3.Lerp(transform.position, target.transform.position + CameraOffset, 20 * Time.deltaTime);
            transform.LookAt(target.transform.position);

        }
    }


}
