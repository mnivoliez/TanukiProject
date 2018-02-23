using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraOrbit : MonoBehaviour {

    protected Transform xFromCamera;
    protected Transform xFromParent;

    private GameObject player;

    protected Vector3 localRotation;
    protected float cameraDistance = 10f;

    [SerializeField]
    private float mouseSensitivity = 4f;
    [SerializeField]
    private float scrollSensitivity = 2f;
    [SerializeField]
	private float orbitDampening = 10f;
	[SerializeField]
	private float scrollDampening = 6f;
	[SerializeField]
	private float raycastDampening = 200f;
	[SerializeField]
	private float minCameraDistance = 7f;
	[SerializeField]
	private float maxCameraDistance = 20f;
	[SerializeField]
	private float minCameraAngle = -20;
	[SerializeField]
	private float maxCameraAngle = 90;
	[SerializeField]
	private bool inverseCam = false;
	[SerializeField]
	private bool scrolEnabled = false;
	[SerializeField]
	private LayerMask ignoredLayerMask;

    private bool leftClicked = false;
    private bool rightClicked = false;

    private Vector3 cameraPositionRemember;

    // Use this for initialization
    void Start() {
        this.xFromCamera = this.transform;
        this.xFromParent = this.transform.parent;
		cameraPositionRemember = this.xFromCamera.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindGameObjectWithTag ("Player");
		localRotation = new Vector3(player.transform.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.x + 20);
		xFromParent.rotation = Quaternion.Euler(localRotation.y, localRotation.x, 0);
    }

    private void Update() {
        xFromParent.position = new Vector3(player.transform.position.x, player.transform.position.y + 1f, player.transform.position.z);
        if (Input.GetMouseButtonDown(0)) {
            leftClicked = true;
        }
        if (Input.GetMouseButtonUp(0)) {
            leftClicked = false;
        }
        if (Input.GetMouseButtonDown(1)) {
            rightClicked = true;
        }
		if (Input.GetMouseButtonUp(1)) {
            rightClicked = false;
        }
    }

    // LateUpdate is called once per frame, afpter Update() on every game object in the scene
    void LateUpdate() {
        //Rotation of the camera based on Mouse Coordinates
        /*if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) {
            localRotation.x += Input.GetAxis("Mouse X") * mouseSensitivity * -1;
            localRotation.y += Input.GetAxis("Mouse Y") * mouseSensitivity * -1;

            //Clamp the y rotation to horizon and not flipping over at the top 
			localRotation.y = Mathf.Clamp(localRotation.y, minCameraAngle, maxCameraAngle);
        }*/

		float horizontal;
		/*if (Input.GetJoystickNames().Length == 0) {
			horizontal = Input.GetAxis("Mouse X") * mouseSensitivity;
		} else*/ {
			horizontal = Input.GetAxis("MoveCameraGamepadHorizontal") * mouseSensitivity;
		}

		localRotation.x += horizontal;

		float vertical;
		/*if (Input.GetJoystickNames().Length == 0) {
			vertical = Input.GetAxis("Mouse Y") * mouseSensitivity;
		} else*/ {
			vertical = Input.GetAxis("MoveCameraGamepadVertical") * mouseSensitivity;
		}


		if (inverseCam) {
			localRotation.y += vertical;
		} else {
			localRotation.y += vertical * -1;
		}
		//Clamp the y rotation to horizon and not flipping over at the top 
		localRotation.y = Mathf.Clamp(localRotation.y, minCameraAngle, maxCameraAngle);

		//Zooming Input from out Mouse Scroll Wheel
		if (scrolEnabled && Input.GetAxis("Mouse ScrollWheel") != 0f)
		{
            float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * mouseSensitivity;
            //Make camera zoom faster the futher away it is from the target
            scrollAmount *= cameraDistance * 0.3f;
            cameraDistance += scrollAmount * -1f;

			//This makes camera go no closer than minCameraDistance and no futher than maxCameraDistance from target;
			cameraDistance = Mathf.Clamp(cameraDistance, minCameraDistance, maxCameraDistance);
        }

        //Actual Camera Rig Transformations
        Quaternion QT = Quaternion.Euler(localRotation.y, localRotation.x, 0);
        xFromParent.rotation = Quaternion.Lerp(xFromParent.rotation, QT, Time.deltaTime * orbitDampening);
        if (rightClicked)
		{
            Quaternion rot = Quaternion.identity;
            rot.eulerAngles = new Vector3(player.transform.rotation.eulerAngles.x, xFromParent.rotation.eulerAngles.y, player.transform.rotation.eulerAngles.z);
            player.transform.rotation = rot;
        }
        
        if (xFromCamera.localPosition.z != cameraDistance * -1f) {
			cameraPositionRemember = new Vector3(0f, 0f, Mathf.Lerp(xFromCamera.localPosition.z, cameraDistance * -1f, Time.deltaTime * scrollDampening));
        }

		float step = 0.2f;

		// direction Camera-Pivot(Player)
        Vector3 direction = xFromParent.position - xFromCamera.position;
		direction = direction.normalized;

		// all layers are = 0xFFFFFFFF => -1
		int layerAll = -1;

		RaycastHit hit;
		if (Physics.Raycast (xFromParent.position, -direction, out hit, cameraDistance * 1.1f, layerAll - ignoredLayerMask.value))
		{
			float dis = Vector3.Distance (player.transform.position, hit.point + direction * step);
			xFromCamera.localPosition = new Vector3 (0f, 0f, Mathf.Lerp (xFromCamera.localPosition.z, dis * -1f, Time.deltaTime * raycastDampening));
		}
		else
		{
			xFromCamera.localPosition = cameraPositionRemember;
		}
    }
}
