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
		UnpauseGame ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Cancel") && !VictorySwitch.Victory){
            if(!Paused){
				PauseGame (true);
                //showPaused();
            }
			else if (Paused) {
				UnpauseGame ();
            }
        }
	}

	public void UnpauseGame() {
		eventSystem.SetActive (false);
		Time.timeScale = 1;
		Paused = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		CanvasPause.enabled = false;
		transform.GetChild(0).gameObject.SetActive(false);
	}

	public void PauseGame(bool showMenu) {
		eventSystem.SetActive (true);
		Time.timeScale = 0;
		Paused = true;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		if (showMenu)
		{
			CanvasPause.enabled = true;
			transform.GetChild (0).gameObject.SetActive (true);
		}

	}
}
