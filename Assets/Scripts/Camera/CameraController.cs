using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
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
	[SerializeField] private float wideCameraDistance = 35f;

	[Header("ANGLE")]
	[Space(10)]
	[SerializeField] private float minCameraAngle = -20;
	[SerializeField] private float maxCameraAngle = 75;
	[SerializeField] private float defaultCameraAngle = 55;
	[SerializeField] private float wideCameraAngle = 65;

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


	private float currentCameraDistance = 15f;

	private float currentCameraAngle = 55f;
	private float currentCameraAngleYRot = 0f;
	private Vector3 currentCameraAngleAxis;
	private float currentCameraHeight = 0;

	private float minCameraHeight = 2;
	private float maxCameraHeight = 7;
	private float defaultCameraHeight = 3.5f;
	private float wideCameraHeight = 3.5f;
	private float raycastCameraHeight = 3.5f;

	private Vector3 diffPos;

	private float centerTime = 0;
	private Vector3 centerCamPos;
	private Vector3 centerCamDirection;
	private Quaternion centerCamRot;
	private bool centerCamera = false;

	private float wideTime = 0;
	private Vector3 wideCamPos;
	private Vector3 wideCamDirection;
	private Quaternion wideCamRot;
	private bool wideCamera = false;
	private bool wideCameraLocked = false;
	private Vector3 wideDeltaPos = new Vector3 (0, 8, -3.6f);
	private Vector3 widePlayerPosition;
	private Vector3 lookatPosition;

    private Vector3 wideLookatPos;
    private Vector3 resetPos;
    private Vector3 directionNoY;
    private Vector3 direction;
    private Vector3 eulerAnglesNew;


    void Start() {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerTanukiModel = player.Find("TanukiPlayer");
		camBase = transform.parent;

		currentCameraDistance = defaultCameraDistance;
		currentCameraAngleYRot = 0f;
		currentCameraAngle = defaultCameraAngle;
		currentCameraAngleAxis = playerTanukiModel.right;
		currentCameraHeight = currentCameraDistance * Mathf.Tan(currentCameraAngle * Mathf.Deg2Rad);
		//Debug.Log ("CAMERA currentCameraDistance=" + currentCameraDistance);
		//Debug.Log ("CAMERA Mathf.Tan (currentCameraAngle)=" + Mathf.Tan (currentCameraAngle * Mathf.Deg2Rad));
		//Debug.Log ("CAMERA currentCameraHeight=" + currentCameraHeight);

		// get the height relative to the default distance
		minCameraHeight = minCameraDistance * Mathf.Tan(minCameraAngle * Mathf.Deg2Rad);
		maxCameraHeight = maxCameraDistance * Mathf.Tan(maxCameraAngle * Mathf.Deg2Rad);
		defaultCameraHeight = defaultCameraDistance * Mathf.Tan(defaultCameraAngle * Mathf.Deg2Rad);

		/*wideCameraHeight = defaultCameraHeight + 8;
		Debug.Log ("wideCameraHeight=" + wideCameraHeight);
		wideCameraAngle = Mathf.Atan (wideCameraHeight / wideCameraDistance) * Mathf.Rad2Deg;
		Debug.Log ("wideCameraAngle=" + wideCameraAngle);*/

		wideCameraHeight = wideCameraDistance * Mathf.Tan(wideCameraAngle * Mathf.Deg2Rad);
		//Debug.Log ("wideCameraHeight2=" + wideCameraHeight);

		RepositionCamera();
	}

	//2
	//-8
	//3

	private void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================

        GetInputData();

		ManagerCenterCamera();

		CheckPlayerChangedPosition ();

		if (!centerCamera && !wideCamera)
		{
			if (!wideCameraLocked)
			{
				GetCameraMovement ();
			}

			GetCameraRotation ();
		}

		RecalculateValues();

		CalibrateInternalCamera();
	}

	private void GetInputData() {
		if (Input.GetButtonDown("CenterCamera") || (Input.GetAxisRaw("CenterCamera") == 1)) {
			if (!centerCamera) {
				RecenterCamera();
			}
		}
		if (Input.GetButtonDown("WideCamera") && !wideCameraLocked) {
			if (!wideCamera) {
				WideCamera();
			}
		}
	}

	public void RecenterCamera() {

		wideCameraLocked = false;
		centerTime = Time.time;
		centerCamDirection = playerTanukiModel.forward;
		centerCamPos = camBase.position;
		centerCamRot = camBase.rotation;
		centerCamera = true;

		currentCameraAngle = defaultCameraAngle;
		currentCameraDistance = defaultCameraDistance;
		currentCameraHeight = defaultCameraHeight;
	}

	public void WideCamera() {
		wideTime = Time.time;
		wideCamDirection = playerTanukiModel.forward;
		wideCamPos = camBase.position;
		wideCamRot = camBase.rotation;
		wideCamera = true;

		currentCameraAngle = wideCameraAngle;
		currentCameraDistance = wideCameraDistance;
		currentCameraHeight = wideCameraHeight;
	}

	private void ManagerCenterCamera() {
		wideLookatPos = playerTanukiModel.position + Vector3.up * wideDeltaPos.y * 2 / 3;

		if (centerCamera)
		{
			//Debug.Log("CAMERA CENTER START!!");
			float diffTime = Time.time - centerTime;
			if (diffTime < timeToCenter + 0.1f)
			{
				//Debug.Log("CAMERA CENTER BEGIN!!");
				resetPos = playerTanukiModel.position + Vector3.up * defaultCameraHeight - centerCamDirection * defaultCameraDistance;
				camBase.position = Vector3.Lerp (centerCamPos, resetPos, Mathf.Clamp01 (diffTime) / timeToCenter);
				camBase.rotation = Quaternion.Lerp (centerCamRot, Quaternion.identity, Mathf.Clamp01 (diffTime) / timeToCenter);

				lookatPosition = Vector3.Lerp (lookatPosition, playerTanukiModel.position, Mathf.Clamp01 (diffTime) / timeToCenter);
			}
			else
			{
				centerCamera = false;
				lookatPosition = playerTanukiModel.position;
			}
		}
		else if (wideCamera)
		{
			//Debug.Log("CAMERA CENTER START!!");
			float diffTime = Time.time - wideTime;
			if (diffTime < timeToCenter + 0.1f)
			{
				//Debug.Log("CAMERA CENTER BEGIN!!");
				resetPos = playerTanukiModel.position + Vector3.up * wideCameraHeight - wideCamDirection * wideCameraDistance;
				camBase.position = Vector3.Lerp (wideCamPos, resetPos, Mathf.Clamp01 (diffTime) / timeToCenter);
				camBase.rotation = Quaternion.Lerp (wideCamRot, Quaternion.identity, Mathf.Clamp01 (diffTime) / timeToCenter);

				lookatPosition = Vector3.Lerp (playerTanukiModel.position, wideLookatPos, Mathf.Clamp01 (diffTime) / timeToCenter);
			}
			else
			{
				wideCamera = false;
				wideCameraLocked = true;
				widePlayerPosition = playerTanukiModel.position;
				lookatPosition = wideLookatPos;
			}
		}
		else if (wideCameraLocked)
		{
			lookatPosition = wideLookatPos;
		}
		else
		{
			lookatPosition = playerTanukiModel.position;
		}
	}

	private void GetCameraMovement() {
		InputParams inputParams = player.GetComponent<InputController>().RetrieveUserRequest();

		if (currentCameraDistance <= maxCameraDistance && currentCameraDistance >= minCameraDistance) // we are inside the range
		{
			camBase.position.Set(camBase.position.x, playerTanukiModel.position.y + currentCameraHeight, camBase.position.z);
		}
		else {
			//Debug.Log ("Dist out of range=" + currentCameraDistance);

			if ( // we are out of range and moving
				(inputParams.moveZ < -0.01f && currentCameraDistance < minCameraDistance) ||
				(inputParams.moveZ > +0.01f && currentCameraDistance > maxCameraDistance)) {
				//Debug.Log ("Dist out of range MOVE");
				camBase.position = playerTanukiModel.position + diffPos;
			}
			else {
				//Debug.Log ("Dist out of range OTHER");
				currentCameraDistance = Mathf.Clamp(currentCameraDistance, minCameraDistance + 0.05f, maxCameraDistance - 0.05f);
				directionNoY = playerTanukiModel.position - camBase.position;
				directionNoY.y = 0;
				directionNoY.Normalize();
				camBase.position = Vector3.Lerp(camBase.position, playerTanukiModel.position + Vector3.up * currentCameraHeight - currentCameraDistance * directionNoY, Time.deltaTime);
			}
		}
	}

	private void GetCameraRotation() {
		//Rotation of the camera based on Mouse Coordinates
		float cam_horizontal = Input.GetAxis("MoveCameraGamepadHorizontal") * mouseSensitivity;
		float cam_vertical = Input.GetAxis("MoveCameraGamepadVertical") * mouseSensitivity;

		if (Mathf.Abs(cam_horizontal) > 0.01f) {
			camBase.RotateAround(playerTanukiModel.position, Vector3.up, cam_horizontal * Time.deltaTime * orbitDampening);

			currentCameraAngleYRot += cam_horizontal * Time.deltaTime * orbitDampening;
		}

		if (Mathf.Abs(cam_vertical) > 0.01f) {
			currentCameraHeight -= cam_vertical * scrollDampening * Time.deltaTime;
			currentCameraHeight = Mathf.Clamp(currentCameraHeight, minCameraHeight, maxCameraHeight);
			camBase.position.Set(camBase.position.x, playerTanukiModel.position.y + currentCameraHeight, camBase.position.z);
		}
	}

	private void RecalculateValues() {
		diffPos = camBase.position - playerTanukiModel.position;

		// get new angle
		direction = playerTanukiModel.position - camBase.position;
		direction = direction.normalized;
		directionNoY = direction;
		directionNoY.y = 0;
		currentCameraAngle = Vector3.SignedAngle(directionNoY, direction, Vector3.Cross(directionNoY, direction));
		//Debug.Log ("CAMERA currentCameraAngle=" + currentCameraAngle);

		// get new ground distanced
		Vector3 playerPosNoY = playerTanukiModel.position;
		Vector3 camPosNoY = camBase.position;
		playerPosNoY.y = 0;
		camPosNoY.y = 0;
		currentCameraDistance = Vector3.Distance(playerPosNoY, camPosNoY);
		//Debug.Log ("CAMERA currentCameraDistance=" + currentCameraDistance);

		// update the height
		/*currentCameraHeight = camBase.position.y - playerTanukiModel.position.y;
		Debug.Log ("CAMERA currentCameraHeight=" + currentCameraHeight);*/
	}

	private void CalibrateInternalCamera() {
		direction = playerTanukiModel.position - camBase.position;
		direction = direction.normalized;

		float step = 0.2f;
		// all layers are = 0xFFFFFFFF => -1
		int layerAll = -1;

		RaycastHit hit;
		// check for clamping local camera
		if (Physics.Raycast(playerTanukiModel.position - direction * 0.1f, -direction, out hit, Mathf.Sqrt(currentCameraDistance * currentCameraDistance + currentCameraHeight * currentCameraHeight) * 1.1f, layerAll - ignoredLayerMask.value)) {
			float colDist = Vector3.Distance(playerTanukiModel.position, hit.point + direction * step);
			if (colDist > Mathf.Sqrt(minCameraDistance * minCameraDistance + minCameraHeight * minCameraHeight)) {
				//Debug.Log ("CLAMP NORMAL!!");
				transform.position = Vector3.Lerp(transform.position, hit.point + direction * step, Time.deltaTime * raycastDampening);
				transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.zero, Time.deltaTime * raycastDampening);
			}
			else {
				RaycastHit hit2;
				//Debug.Log ("CLAMP MINIMUM!! " + hit.transform.name);
				Vector3 hitPointnoY = hit.point - transform.position;
				hitPointnoY.y = 0;

				Vector3 startingPoint = playerTanukiModel.position + Vector3.up * raycastCameraHeight;

				if (Physics.Raycast(startingPoint, (camBase.position - startingPoint).normalized, out hit2, Mathf.Sqrt(currentCameraDistance * currentCameraDistance + currentCameraHeight * currentCameraHeight) * 1.1f, layerAll - ignoredLayerMask.value))
				{
					//Debug.Log ("CLAMP MINIMUM REPAIRED!! " + hit2.point);
					//Debug.Log ("CLAMP MINIMUM REPAIRED BETTER!! " + transform.position);
					//Debug.Log ("CLAMP MINIMUM REPAIRED EVEN BETTER!! " + (hit2.point + direction * step + (hit2.point - hit.point).normalized * currentCameraHeight));
					//transform.position = Vector3.Lerp(transform.position, hit.point + direction * step + Vector3.up * currentCameraHeight, Time.deltaTime * raycastDampening);
					transform.position = Vector3.Lerp(transform.position, hit.point + hit.normal * step + (Vector3.up - Vector3.Project(Vector3.up, hit.normal)).normalized * currentCameraHeight, Time.deltaTime * raycastDampening);
				}
				eulerAnglesNew.Set(Vector3.Angle (camBase.position - playerTanukiModel.position, transform.position - playerTanukiModel.position), 0, 0);
				transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, eulerAnglesNew, Time.deltaTime * raycastDampening);;
			}
		}
		else {
			//Debug.Log ("CLAMP RELEASE!!");
			transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * raycastDampening);
			transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, Vector3.zero, Time.deltaTime * raycastDampening);
		}


		camBase.LookAt(lookatPosition);
	}

	private void CheckPlayerChangedPosition()
	{
		if (wideCameraLocked && Vector3.Distance (widePlayerPosition, playerTanukiModel.position) > 0.1f)
		{
			RecenterCamera();
		}
	}

	private void RepositionCamera() {
		camBase.rotation = Quaternion.identity;
		camBase.position.Set
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
