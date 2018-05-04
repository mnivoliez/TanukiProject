using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurificatorBehavior : MonoBehaviour {

    [SerializeField] private SwitchObject[] actionOnSwitch;
    [SerializeField] private bool isZ1;
    [SerializeField] private bool isZ2;

    void Start () {
        if ((isZ1 && Game.playerData.lightBoss1) || (isZ2 && Game.playerData.lightBoss2)) {
            foreach (SwitchObject obj in actionOnSwitch) {
                if (obj.gameObject != null) {
                    obj.gameObject.SetActive(obj.action.Equals(ActionLantern.Activate));
                }
            }
        }

    }
	
}
