using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckPoint : MonoBehaviour {

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {

        if(other.name == "Player") {

            Game.PreSave_Game_and_Save();
            //Debug.Log(Application.persistentDataPath);
        }
    }
}
