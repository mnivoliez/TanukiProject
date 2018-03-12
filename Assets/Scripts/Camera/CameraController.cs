using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private float mouseSensitivity = 4f;
	[SerializeField] private float scrollSensitivity = 2f;
	[SerializeField] private float orbitDampening = 10f;
	[SerializeField] private float scrollDampening = 6f;
	[SerializeField] private float raycastDampening = 200f;

	[SerializeField] private float minCameraDistance = 7f;
	[SerializeField] private float maxCameraDistance = 20f;
	[SerializeField] private float defaultCameraDistance = 15f;

	[SerializeField] private float minCameraAngle = -20;
	[SerializeField] private float maxCameraAngle = 75;
	[SerializeField] private float defaultCameraAngle = 55;

	[SerializeField] private LayerMask ignoredLayerMask;

	private Transform camBase;
	private Transform player;
	private Transform playerTanukiModel;

	private float cameraDistance = 10f;

	private bool leftClicked = false;
	private bool rightClicked = false;
	private bool centerCamera = false;

	private float currentCameraDistance = 15f;
	//private float currentUnclampedCameraDistance = 15f;

	private float currentCameraAngle = 55f;
	private float currentCameraAngleYRot = 0f;
	private Vector3 currentCameraAngleAxis;
	private float currentCameraHeight = 0;

	private Vector3 diffPos;
	//private Vector3 diffAngle;

    void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerTanukiModel = player.Find ("TanukiPlayer");
		camBase = transform.parent;

		currentCameraDistance = defaultCameraDistance;
		currentCameraAngleYRot = 0f;
		currentCameraAngle = defaultCameraAngle;
		currentCameraAngleAxis = playerTanukiModel.right;
		currentCameraHeight = currentCameraDistance * Mathf.Tan (currentCameraAngle * Mathf.Deg2Rad);
		Debug.Log ("CAMERA currentCameraDistance=" + currentCameraDistance);
		Debug.Log ("CAMERA Mathf.Tan (currentCameraAngle)=" + Mathf.Tan (currentCameraAngle * Mathf.Deg2Rad));
		Debug.Log ("CAMERA currentCameraHeight=" + currentCameraHeight);

		RepositionCamera ();
	}

	private void Update() {
		if (Pause.Paused) {
			return;
		}
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
		if (Input.GetButtonDown("CenterCamera")) {
			Debug.Log ("CAMERA CENTER DOWN!!");
			centerCamera = true;
		}
		if (Input.GetButtonDown("CenterCamera")) {
			Debug.Log ("CAMERA CENTER UP!!");
			centerCamera = false;
		}

		InputParams inputParams = player.GetComponent<InputController>().RetrieveUserRequest();

		if (currentCameraDistance < maxCameraDistance && currentCameraDistance > minCameraDistance)
		{
			if (Mathf.Abs (inputParams.moveX) > 0.01f || Mathf.Abs (inputParams.moveZ) > 0.01f)
			{
				camBase.position = new Vector3 (camBase.position.x, playerTanukiModel.position.y + currentCameraHeight, camBase.position.z);
			}
		}
		else
		{
			Debug.Log ("Dist out of range=" + currentCameraDistance);
			if (
				(inputParams.moveZ < -0.01f && currentCameraDistance < minCameraDistance + 0.05f) ||
				(inputParams.moveZ > +0.01f && currentCameraDistance > maxCameraDistance - 0.05f))
			{
				Debug.Log ("Dist out of range MOVE");
				camBase.position = playerTanukiModel.position + diffPos;
				//camBase.rotation = Quaternion.Euler (playerTanukiModel.rotation.eulerAngles + diffAngle);
			}
			else
			{
				camBase.position = new Vector3 (camBase.position.x, playerTanukiModel.position.y + currentCameraHeight, camBase.position.z);
			}
		}

		//Rotation of the camera based on Mouse Coordinates
		float horizontal = Input.GetAxis("MoveCameraGamepadHorizontal") * mouseSensitivity;
		float vertical = Input.GetAxis("MoveCameraGamepadVertical") * mouseSensitivity;

		if (Mathf.Abs (horizontal) > 0.01f)
		{
			camBase.RotateAround(playerTanukiModel.position, Vector3.up, horizontal * Time.deltaTime * orbitDampening);

			currentCameraAngleYRot += horizontal * Time.deltaTime * orbitDampening;
		}
		diffPos = camBase.position - playerTanukiModel.position;
		//diffAngle = camBase.rotation.eulerAngles - playerTanukiModel.rotation.eulerAngles;

		camBase.LookAt (player.position);

		float step = 0.2f;
		// all layers are = 0xFFFFFFFF => -1
		int layerAll = -1;

		// get new angle
		Vector3 direction = playerTanukiModel.position - camBase.position;
		direction = direction.normalized;
		Vector3 directionNoY = direction;
		directionNoY.y = 0;
		currentCameraAngle = Vector3.SignedAngle (directionNoY, direction, Vector3.Cross(directionNoY, direction));
		//Debug.Log ("CAMERA currentCameraAngle=" + currentCameraAngle);

		// get new ground distanced
		Vector3 playerPosNoY = playerTanukiModel.position;
		Vector3 camPosNoY = camBase.position;
		playerPosNoY.y = 0;
		camPosNoY.y = 0;
		currentCameraDistance = Vector3.Distance (playerPosNoY, camPosNoY);
		//Debug.Log ("CAMERA currentCameraDistance=" + currentCameraDistance);

		// update the height
		/*currentCameraHeight = camBase.position.y - playerTanukiModel.position.y;
		Debug.Log ("CAMERA currentCameraHeight=" + currentCameraHeight);*/

		// check for clamping local camera
		RaycastHit hit;
		if (Physics.Raycast(playerTanukiModel.position, -direction, out hit, currentCameraDistance / Mathf.Sin(currentCameraAngle) * 1.1f, layerAll - ignoredLayerMask.value)) {
			//Debug.Log ("CLAMP!!");
			transform.position = Vector3.Lerp(transform.position, hit.point + direction * step, Time.deltaTime * raycastDampening);
		} else {
			//Debug.Log ("CLAMP RELEASE!!");
			transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * raycastDampening);
		}
	}

	private void RepositionCamera() {
		camBase.rotation = Quaternion.identity;
		camBase.position = new Vector3 
			(
				playerTanukiModel.position.x,
				playerTanukiModel.position.y + currentCameraHeight,
				playerTanukiModel.position.z - currentCameraDistance
			);
		//camBase.RotateAround (playerTanukiModel.position, currentCameraAngleAxis, currentCameraAngle);
		camBase.RotateAround (playerTanukiModel.position, Vector3.up, currentCameraAngleYRot);
	}

	public void ResizeDistanceCamera(bool playerIsTiny, float coefResize) {
		if (playerIsTiny) {
			minCameraDistance = minCameraDistance / coefResize;
			maxCameraDistance = maxCameraDistance / coefResize;
			cameraDistance = cameraDistance / coefResize;
		}
		else {
			minCameraDistance = minCameraDistance * coefResize;
			maxCameraDistance = maxCameraDistance * coefResize;
			cameraDistance = cameraDistance * coefResize;
		}

	}
}
