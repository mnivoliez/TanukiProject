using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour {

	public static bool Paused = false;
	public GameObject eventSystem;

	private Canvas CanvasPause;

	// Use this for initialization
	void Start () {
        Time.timeScale = 1;
		CanvasPause = GetComponent<Canvas> ();
		CanvasPause.enabled = false;

		Cursor.lockState = CursorLockMode.Locked;
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
		eventSystem.SetActive (false);
		Time.timeScale = 1;
		Paused = false;
		CanvasPause.enabled = false;
		transform.GetChild(0).gameObject.SetActive(false);

		Cursor.lockState = CursorLockMode.Locked;
	}

	public void PauseGame() {
		eventSystem.SetActive (true);
		Time.timeScale = 0;
		Paused = true;
		CanvasPause.enabled = true;
		transform.GetChild(0).gameObject.SetActive (true);

		Cursor.lockState = CursorLockMode.None;
	}
}
