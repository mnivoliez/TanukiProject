using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour {

	public static bool Paused = false;

	private Canvas CanvasPause;

	// Use this for initialization
	void Start () {
        Time.timeScale = 1;
		CanvasPause = GetComponent<Canvas> ();
		CanvasPause.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Cancel")){
            if(Time.timeScale == 1){
				PauseGame ();
                //showPaused();
            }
            else if (Time.timeScale == 0) {
                Debug.Log("high");
				UnpauseGame ();
                //hidePaused();
            }
        }
	}

	public void UnpauseGame() {
		Time.timeScale = 1;
		Paused = false;
		CanvasPause.enabled = false;
		transform.GetChild(0).gameObject.SetActive(false);
	}

	public void PauseGame() {
		Time.timeScale = 0;
		Paused = true;
		CanvasPause.enabled = true;
		transform.GetChild(0).gameObject.SetActive (true);
	}
}
