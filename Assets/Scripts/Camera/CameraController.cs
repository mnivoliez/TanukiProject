using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;
    public Vector3 offset;
    public float rotateSpeed;
    public Transform pivotCam;
    public float maxViewAngle;
    public float minViewAngle;
    public bool useOffsetValues;
    public bool inverseCam;

    void Start() {

        if (!useOffsetValues) {
            offset = target.position - transform.position;
        }

        pivotCam.transform.position = target.position;
        //pivotCam.transform.parent = target;
        pivotCam.transform.parent = null;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void LateUpdate() {
        pivotCam.transform.position = target.position;

        float horizontal;
        if (Input.GetJoystickNames().Length == 0) {
            horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        }
        else {
            horizontal = Input.GetAxis("MoveCameraGamepadHorizontal") * rotateSpeed;
        }

        pivotCam.Rotate(0, horizontal, 0);

        float vertical;
        if (Input.GetJoystickNames().Length == 0) {
            vertical = Input.GetAxis("Mouse Y") * rotateSpeed;
        }
        else {
            vertical = Input.GetAxis("MoveCameraGamepadVertical") * rotateSpeed;
        }


        if (inverseCam) {
            pivotCam.Rotate(vertical, 0, 0);
        }
        else {
            pivotCam.Rotate(-vertical, 0, 0);
        }

        float desiredYangle = pivotCam.eulerAngles.y;
        float desiredXangle = pivotCam.eulerAngles.x;

        if (pivotCam.rotation.eulerAngles.x > maxViewAngle && pivotCam.rotation.eulerAngles.x < 180f) {
            pivotCam.rotation = Quaternion.Euler(maxViewAngle, desiredYangle, 0);
        }
        if (pivotCam.rotation.eulerAngles.x > 180f && pivotCam.rotation.eulerAngles.x < 360f + minViewAngle) {
            pivotCam.rotation = Quaternion.Euler(360f + minViewAngle, desiredYangle, 0);
        }


        Quaternion camRotation = Quaternion.Euler(desiredXangle, desiredYangle, 0);
        transform.position = target.position - (camRotation * offset);

        //transform.position = target.position - offset;
        if (transform.position.y < target.position.y) {
            transform.position = new Vector3(transform.position.x, target.transform.position.y, transform.position.z);
        }

        transform.LookAt(target.position);

    }
}
