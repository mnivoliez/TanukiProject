using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//================================================
//SOUNDCONTROLER
//================================================

public class Pause : MonoBehaviour {

	public static bool Paused = false;
	//public GameObject eventSystem;

	private Canvas CanvasPause;

    private GameObject PausePanel;
    private GameObject OptionsPanel;
    private GameObject ExitPanel;
    private GameObject LoadingCanva;

    public Text score_to_display;

    public Text has_doublejump;
    public Text has_lure;
    public Text has_ball;
    public Text has_shrink;


    // Use this for initialization
    void Start () {
        Time.timeScale = 1;
		CanvasPause = GetComponent<Canvas> ();
        CanvasPause.enabled = false;

        PausePanel = CanvasPause.transform.GetChild(0).gameObject;
        OptionsPanel = CanvasPause.transform.GetChild(1).gameObject;
        ExitPanel = CanvasPause.transform.GetChild(2).gameObject;

        Cursor.lockState = CursorLockMode.Locked;
        LoadingCanva = GameObject.Find("LoadingScreen");
        UnpauseGame ();
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Cancel") && !VictorySwitch.Victory) {
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
        if (!LoadingCanva.transform.GetChild(0).gameObject.activeInHierarchy) { 
            //================================================
            SoundController.instance.SelectHUD("PauseDisabled");
            //================================================
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
    }

	public void PauseGame(bool showMenu) {
        if (!LoadingCanva.transform.GetChild(0).gameObject.activeInHierarchy) {
            //================================================
            SoundController.instance.SelectHUD("PauseEnabled");
            //================================================
            Time.timeScale = 0;
		    Cursor.lockState = CursorLockMode.None;
		    Cursor.visible = true;

		    if (showMenu)
		    {
                PausePanel.SetActive(true);
                CanvasPause.enabled = true;
			    transform.GetChild (0).gameObject.SetActive (true);
                Update_Pause_Menu_data();
            }
            Paused = true;
        }
    }

    public void Load_Game() {
        UnpauseGame();
        Game.Load_and_Post_Load();
    }

    public void Update_Pause_Menu_data() {
        Game.Update_Game();
        int score_value = Game.playerData.caught_yokai;
        score_to_display.text = "Yokai Caught : " + score_value;

        bool has_power = Game.playerData.power_jump;
        has_doublejump.text = "Double Jump : " + has_power;

        has_power = Game.playerData.power_lure;
        has_lure.text = "Lure : " + has_power;

        has_power = Game.playerData.power_ball;
        has_ball.text = "Ball : " + has_power;

        has_power = Game.playerData.power_shrink;
        has_shrink.text = "Shrink : " + has_power;
    }
}
