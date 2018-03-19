using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[Header("MOUSE")]
	[Space(10)]
	[SerializeField] private float mouseSensitivity = 4f;
	[SerializeField] private float scrollSensitivity = 2f;
	[SerializeField] private float orbitDampening = 10f;
	[SerializeField] private float scrollDampening = 6f;
	[SerializeField] private float raycastDampening = 200f;

	[Header("DISTANCE")]
	[Space(10)]
	[SerializeField] private float minCameraDistance = 7f;
	[SerializeField] private float maxCameraDistance = 20f;
	[SerializeField] private float defaultCameraDistance = 15f;

	[Header("ANGLE")]
	[Space(10)]
	[SerializeField] private float minCameraAngle = -20;
	[SerializeField] private float maxCameraAngle = 75;
	[SerializeField] private float defaultCameraAngle = 55;

	[Header("CENTER")]
	[Space(10)]
	[SerializeField] private float timeToCenter;

	[Header("LAYER")]
	[Space(10)]
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

	private float minCameraHeight = 2;
	private float maxCameraHeight = 7;
	private float defaultCameraHeight = 3.5f;
	private float raycastCameraHeight = 3.5f;

	private Vector3 diffPos;
	//private Vector3 diffAngle;

	private float centerTime = 0;
	private Vector3 centerCamPos;
	private Vector3 centerCamDirection;
	private Quaternion centerCamRot;

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
		//Debug.Log ("CAMERA currentCameraDistance=" + currentCameraDistance);
		//Debug.Log ("CAMERA Mathf.Tan (currentCameraAngle)=" + Mathf.Tan (currentCameraAngle * Mathf.Deg2Rad));
		//Debug.Log ("CAMERA currentCameraHeight=" + currentCameraHeight);

		// get the height relative to the default distance
		minCameraHeight = defaultCameraDistance * Mathf.Tan (minCameraAngle * Mathf.Deg2Rad);
		maxCameraHeight = defaultCameraDistance * Mathf.Tan (maxCameraAngle * Mathf.Deg2Rad);
		defaultCameraHeight = defaultCameraDistance * Mathf.Tan (defaultCameraAngle * Mathf.Deg2Rad);

		RepositionCamera ();
	}

	private void Update() {
		if (Pause.Paused) {
			return;
		}

		GetInputData ();

		ManagerCenterCamera ();

		if (!centerCamera)
		{
			GetCameraMovement ();

			GetCameraRotation ();
		}

		RecalculateValues ();

		CalibrateInternalCamera ();
	}

	private void GetInputData()
	{
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
			if (!centerCamera)
			{
				centerTime = Time.time;
				centerCamDirection = playerTanukiModel.forward;
				centerCamPos = camBase.position;
				centerCamRot = camBase.rotation;
				centerCamera = true;

				currentCameraAngle = defaultCameraAngle;
				currentCameraDistance = currentCameraDistance;
				currentCameraHeight = defaultCameraHeight;
			}
		}
		if (Input.GetButtonDown("CenterCamera")) {
			Debug.Log ("CAMERA CENTER UP!!");
			//centerCamera = false;
		}
	}

	private void ManagerCenterCamera()
	{
		if (centerCamera)
		{
			Debug.Log ("CAMERA CENTER START!!");
			float diffTime = Time.time - centerTime;
			if (diffTime < timeToCenter + 0.1f)
			{
				Debug.Log ("CAMERA CENTER BEGIN!!");
				Vector3 resetPos = playerTanukiModel.position + Vector3.up * defaultCameraHeight - centerCamDirection * defaultCameraDistance;
				camBase.position = Vector3.Lerp (centerCamPos, resetPos, Mathf.Clamp01(diffTime) / timeToCenter);
				camBase.rotation = Quaternion.Lerp (centerCamRot, Quaternion.identity, Mathf.Clamp01(diffTime) / timeToCenter);
			}
			else
			{
				centerCamera = false;
			}
		}
	}

	private void GetCameraMovement()
	{
		InputParams inputParams = player.GetComponent<InputController>().RetrieveUserRequest();

		if (currentCameraDistance <= maxCameraDistance && currentCameraDistance >= minCameraDistance) // we are inside the range
		{
			camBase.position = new Vector3 (camBase.position.x, playerTanukiModel.position.y + currentCameraHeight, camBase.position.z);
		}
		else
		{
			//Debug.Log ("Dist out of range=" + currentCameraDistance);

			if ( // we are out of range and moving
				(inputParams.moveZ < -0.01f && currentCameraDistance < minCameraDistance) ||
				(inputParams.moveZ > +0.01f && currentCameraDistance > maxCameraDistance))
			{
				//Debug.Log ("Dist out of range MOVE");
				camBase.position = playerTanukiModel.position + diffPos;
			}
			else
			{
				//Debug.Log ("Dist out of range OTHER");
				currentCameraDistance = Mathf.Clamp (currentCameraDistance, minCameraDistance + 0.05f, maxCameraDistance - 0.05f);
				Vector3 directionNoY = playerTanukiModel.position - camBase.position;
				directionNoY.y = 0;
				directionNoY.Normalize();
				camBase.position = playerTanukiModel.position + Vector3.up * currentCameraHeight - currentCameraDistance * directionNoY ;
			}
		}
	}

	private void GetCameraRotation()
	{
		//Rotation of the camera based on Mouse Coordinates
		float cam_horizontal = Input.GetAxis("MoveCameraGamepadHorizontal") * mouseSensitivity;
		float cam_vertical = Input.GetAxis("MoveCameraGamepadVertical") * mouseSensitivity;

		if (Mathf.Abs (cam_horizontal) > 0.01f)
		{
			camBase.RotateAround(playerTanukiModel.position, Vector3.up, cam_horizontal * Time.deltaTime * orbitDampening);

			currentCameraAngleYRot += cam_horizontal * Time.deltaTime * orbitDampening;
		}

		if (Mathf.Abs (cam_vertical) > 0.01f)
		{
			currentCameraHeight -= cam_vertical * scrollDampening * Time.deltaTime;
			currentCameraHeight = Mathf.Clamp (currentCameraHeight, minCameraHeight, maxCameraHeight);
			camBase.position = new Vector3 (camBase.position.x, playerTanukiModel.position.y + currentCameraHeight, camBase.position.z);
		}
	}

	private void RecalculateValues()
	{
		diffPos = camBase.position - playerTanukiModel.position;

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
	}

	private void CalibrateInternalCamera()
	{
		Vector3 direction = playerTanukiModel.position - camBase.position;
		direction = direction.normalized;

		float step = 0.2f;
		// all layers are = 0xFFFFFFFF => -1
		int layerAll = -1;

		RaycastHit hit;
		// check for clamping local camera
		if (Physics.Raycast(playerTanukiModel.position, -direction, out hit, currentCameraDistance * 1.1f, layerAll - ignoredLayerMask.value)) {
			float colDist = Vector3.Distance (playerTanukiModel.position, hit.point + direction * step);
			if (colDist > minCameraDistance)
			{
				//Debug.Log ("CLAMP NORMAL!!");
				transform.position = Vector3.Lerp (transform.position, hit.point + direction * step, Time.deltaTime * raycastDampening);
			}
			else
			{
				RaycastHit hit1;
				RaycastHit hit2;
				//Debug.Log ("CLAMP MINIMUM!!");
				Vector3 hitPointnoY = hit.point - transform.position;
				hitPointnoY.y = 0;

				Vector3 startingPoint = playerTanukiModel.position + Vector3.up * raycastCameraHeight;
				if (Physics.Raycast (startingPoint, (camBase.position - startingPoint).normalized, out hit2, maxCameraDistance * 1.1f, layerAll - ignoredLayerMask.value))
				{
					//Debug.Log ("CLAMP MINIMUM REPAIRED!! " + hit2.point);
					//Debug.Log ("CLAMP MINIMUM REPAIRED BETTER!! " + transform.position);
					//Debug.Log ("CLAMP MINIMUM REPAIRED EVEN BETTER!! " + (hit2.point + direction * step + Vector3.up * currentCameraHeight));
					transform.position = Vector3.Lerp (transform.position, hit2.point + direction * step + Vector3.up * currentCameraHeight, Time.deltaTime * raycastDampening);
				}
			}
		} else {
			//Debug.Log ("CLAMP RELEASE!!");
			transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * raycastDampening);
		}

		camBase.LookAt (playerTanukiModel.position);
		transform.LookAt (playerTanukiModel.position);
	}

	private void RepositionCamera() {
		camBase.rotation = Quaternion.identity;
		camBase.position = new Vector3 
			(
				playerTanukiModel.position.x - currentCameraDistance * Mathf.Sin(currentCameraAngleYRot * Mathf.Deg2Rad),
				playerTanukiModel.position.y + currentCameraHeight,
				playerTanukiModel.position.z - currentCameraDistance * Mathf.Cos(currentCameraAngleYRot * Mathf.Deg2Rad)
			);
		//camBase.RotateAround (playerTanukiModel.position, Vector3.up, currentCameraAngleYRot);
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
