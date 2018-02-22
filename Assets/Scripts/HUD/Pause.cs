using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {

	public static bool Paused = false;
	public GameObject CanvasPrefab;

	private GameObject CanvasInstance;

	// Use this for initialization
	void Start () {
        Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Cancel")){
            if(Time.timeScale == 1){
                Time.timeScale = 0;
				Paused = true;

                //showPaused();
            }
            else if (Time.timeScale == 0) {
                Debug.Log("high");
                Time.timeScale = 1;
				Paused = false;
                //hidePaused();
            }
        }
	}
}
