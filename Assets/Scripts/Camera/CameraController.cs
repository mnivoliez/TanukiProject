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

	private Transform player;
	private Transform playerTanukiModel;

	private float cameraDistance = 10f;

	private bool leftClicked = false;
	private bool rightClicked = false;
	private bool centerCamera = false;

	private Vector3 diffPos;
	private Vector3 diffAngle;

    void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerTanukiModel = player.Find ("TanukiPlayer");

		transform.position = new Vector3(playerTanukiModel.transform.position.x, playerTanukiModel.transform.position.y, playerTanukiModel.transform.position.z - defaultCameraDistance);
		transform.rotation = Quaternion.identity;
		transform.RotateAround (playerTanukiModel.transform.position, playerTanukiModel.transform.right, defaultCameraAngle);
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
		Vector3 playerPosNoY = playerTanukiModel.position;
		Vector3 camPosNoY = transform.position;

		playerPosNoY.y = 0;
		camPosNoY.y = 0;

		float dist = Vector3.Distance (playerPosNoY, camPosNoY);

		if (dist < maxCameraDistance && dist > minCameraDistance)
		{
		}
		else
		{
			if (Mathf.Abs (inputParams.moveZ) > 0.01f)
			{
				transform.position = playerTanukiModel.position + diffPos;
				transform.rotation = Quaternion.Euler (playerTanukiModel.rotation.eulerAngles + diffAngle);
			}
		}

		//Rotation of the camera based on Mouse Coordinates
		float horizontal = Input.GetAxis("MoveCameraGamepadHorizontal") * mouseSensitivity;
		float vertical = Input.GetAxis("MoveCameraGamepadVertical") * mouseSensitivity;

		if (Mathf.Abs (horizontal) > 0.01f)
		{
			transform.RotateAround(playerTanukiModel.position, Vector3.up, horizontal * Time.deltaTime * orbitDampening);
		}
		diffPos = transform.position - playerTanukiModel.position;
		diffAngle = transform.rotation.eulerAngles - playerTanukiModel.rotation.eulerAngles;

		transform.LookAt (player.position);
	}
}
