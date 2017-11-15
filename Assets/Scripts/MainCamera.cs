using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	//Camera Fixe
//	public GameObject target; // The target in which to follow
//	public Vector3 CameraOffset = new Vector3(0,10,-20); // This will allow us to offset the camera for the player's view.
//	public bool useOffsetValues;
//
//	void Start () {
//
//	}
//
//	void Update(){
		//if (target != null) {
	//		transform.position = CameraOffset;
	//		transform.LookAt (target.transform);
	//	}
//	}


//	//======================================================================================
//	//Camera Follow
//	public GameObject target; // The target in which to follow
//	public Vector3 CameraOffset = new Vector3(0,5,-10); // This will allow us to offset the camera for the player's view.
//	public bool useOffsetValues;
//
//	void Start () {
//	CameraOffset = new Vector3(0,5,-10);
//	}
//
//	void Update(){
//		if (target != null) {
//			transform.position = target.transform.position + CameraOffset;
//			transform.LookAt (target.transform);
//		}
//	}

	//======================================================================================
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
			transform.position = Vector3.Lerp(transform.position, target.transform.position + CameraOffset, 10 * Time.deltaTime);
			positionTarget = target.transform;
			positionTarget.position.Set(target.transform.position.x, target.transform.position.y, target.transform.position.z);
			transform.LookAt (positionTarget);

		}
	}


}
