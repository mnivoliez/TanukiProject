using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour {

	public static bool Paused = false;
	//public GameObject eventSystem;

	private Canvas CanvasPause;

    private GameObject PausePanel;
    private GameObject OptionsPanel;
    private GameObject ExitPanel;


    // Use this for initialization
    void Start () {
        Time.timeScale = 1;
		CanvasPause = GetComponent<Canvas> ();
        CanvasPause.enabled = false;

        PausePanel = CanvasPause.transform.GetChild(0).gameObject;
        OptionsPanel = CanvasPause.transform.GetChild(1).gameObject;
        ExitPanel = CanvasPause.transform.GetChild(2).gameObject;

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
        PausePanel.SetActive (false);
        OptionsPanel.SetActive(false);
        ExitPanel.SetActive(false);
        Time.timeScale = 1;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

        CanvasPause.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        Paused = false;
    }

	public void PauseGame(bool showMenu) {
        Time.timeScale = 0;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		if (showMenu)
		{
            PausePanel.SetActive(true);
            CanvasPause.enabled = true;
			transform.GetChild (0).gameObject.SetActive (true);
		}
        Paused = true;
    }
}
