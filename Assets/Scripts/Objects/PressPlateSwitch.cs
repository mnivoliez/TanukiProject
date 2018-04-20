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

    void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.CompareTag("Lure")) {
            if (!isPressedByLure) {
                isPressedByLure = true;
                collision.gameObject.transform.parent = gameObject.transform;
                transform.position = transform.position + (Vector3.down * 0.1f);
                foreach (SwitchObject obj in actionOnSwitch) {
                    if (obj.gameObject != null) {
                        obj.gameObject.SetActive(obj.action.Equals(ActionLantern.Activate));
                    }
                }
            }
        }

    }

    //void OnCollisionExit(Collision collision) {
    //    if (collision.gameObject.CompareTag("Lure")) {
    //        if (isPressedByLure) {
    //            isPressedByLure = false;
    //            collision.gameObject.transform.parent = null;
    //            transform.position = transform.position + (Vector3.up * 0.1f);
    //            foreach (SwitchObject obj in actionOnSwitch) {
    //                if (obj.gameObject != null) {
    //                    obj.gameObject.SetActive(!obj.action.Equals(ActionLantern.Activate));
    //                }
    //            }
    //        }
    //    }
    //}

    //Call by a Lure when is destroy
    public void UnpressPlate() {
        if (isPressedByLure) {
            transform.position = transform.position + (Vector3.up * 0.1f);
            isPressedByLure = false;
            foreach (SwitchObject obj in actionOnSwitch) {
                if (obj.gameObject != null) {
                    obj.gameObject.SetActive(!obj.action.Equals(ActionLantern.Activate));
                }
            }
        }
    }

}
