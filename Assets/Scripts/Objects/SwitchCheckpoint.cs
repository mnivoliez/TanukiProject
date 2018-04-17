using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCheckpoint : MonoBehaviour {
    [SerializeField] private SwitchObject[] actionOnSwitch;

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Player")) {

            foreach (SwitchObject obj in actionOnSwitch) {
                if (obj.gameObject != null) {
                    obj.gameObject.SetActive(obj.action.Equals(ActionLantern.Activate));
                }
            }
        }
    }

}
