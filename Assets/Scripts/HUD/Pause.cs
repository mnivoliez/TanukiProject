using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P)){
            if(Time.timeScale == 1){
                Time.timeScale = 0;
                //showPaused();
            }
            else if (Time.timeScale == 0) {
                Debug.Log("high");
                Time.timeScale = 1;
                //hidePaused();
            }
        }
	}
}
