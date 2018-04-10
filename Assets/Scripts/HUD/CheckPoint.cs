using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {

        if(other.CompareTag("Player")) {

            Game.PreSave_Game_and_Save();
            //Debug.Log(Application.persistentDataPath);
        }
    }
}
