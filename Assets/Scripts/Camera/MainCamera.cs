using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {


	//Camera Follow Smooth
	public GameObject target; // The target in which to follow
	public Vector3 CameraOffset; // This will allow us to offset the camera for the player's view.
	public bool useOffsetValues;
	private Transform positionTarget;

	void Start () {
		//CameraOffset = new Vector3(0,5,-10);
		//positionTarget = new Transform();
	}

	void FixedUpdate(){
		if (target != null) {
            //transform.RotateAround(target.transform.position, new Vector3(0, 1, 0), Input.GetAxis("MoveCamera") * 2);

            //CameraOffset.x += Input.GetAxis("MoveCamera");
            //CameraOffset.z += Input.GetAxis("MoveCamera");

            

            //transform.RotateAround(target.transform.position, new Vector3(0, 1, 0), Input.GetAxis("MoveCamera") * 2);

            transform.position = Vector3.Lerp(transform.position, target.transform.position + CameraOffset, 20 * Time.deltaTime);

            transform.LookAt (target.transform.position);
            //transform.position = Vector3.Lerp(transform.position, target.transform.position + CameraOffset, 40 * Time.deltaTime);
            //transform.position = Vector3.Lerp(transform.position, target.transform.position + CameraOffset, 40 * Time.deltaTime);
            //positionTarget = target.transform;
            //positionTarget.position.Set(target.transform.position.x, target.transform.position.y, target.transform.position.z);
            //transform.LookAt(positionTarget);

        }
    }


}
