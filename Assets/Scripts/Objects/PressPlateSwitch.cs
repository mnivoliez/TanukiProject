using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressPlateSwitch : MonoBehaviour {
    [SerializeField]
    private SwitchObject[] actionOnSwitch;

    private bool isPressedByLure;

    void Start() {
        isPressedByLure = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        isPressedByLure = collision.gameObject.CompareTag("Lure");
        if(isPressedByLure) {
            foreach(SwitchObject obj in actionOnSwitch) {
                if (obj.gameObject != null)
                {
                    obj.gameObject.SetActive (obj.action.Equals (ActionLantern.Activate));
                }
            }
        }

    }

    void OnCollisionExit(Collision collision) {
        isPressedByLure = !collision.gameObject.CompareTag("Lure");
        if(!isPressedByLure) {
            foreach(SwitchObject obj in actionOnSwitch) {
                if (obj.gameObject != null)
                {
                    obj.gameObject.SetActive (!obj.action.Equals (ActionLantern.Activate));
                }
            }
        }
    }
}
