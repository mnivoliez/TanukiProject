using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressPlateSwitch : MonoBehaviour {
    [SerializeField]
    private SwitchObject[] actionOnSwitch;

    private bool isPressedByLure;
    private bool isPressedByPlayer;

    void Start() {
        isPressedByLure = false;
        isPressedByPlayer = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        isPressedByPlayer = collision.gameObject.CompareTag("Player");
        isPressedByLure = collision.gameObject.CompareTag("Lure");
        bool isPressed = isPressedByLure || isPressedByPlayer;
        if(isPressed) {
            foreach(SwitchObject obj in actionOnSwitch) {
                if (obj.gameObject != null)
                {
                    obj.gameObject.SetActive (obj.action.Equals (ActionLantern.Activate));
                }
            }
        }

    }

    void OnCollisionExit(Collision collision) {
        isPressedByPlayer = !collision.gameObject.CompareTag("Player");
        isPressedByLure = !collision.gameObject.CompareTag("Lure");
        bool isPressed = isPressedByLure || isPressedByPlayer;
        if(!isPressed) {
            foreach(SwitchObject obj in actionOnSwitch) {
                if (obj.gameObject != null)
                {
                    obj.gameObject.SetActive (!obj.action.Equals (ActionLantern.Activate));
                }
            }
        }
    }
}
