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

    public Text score_to_display_Z1P1;
    public Text score_to_display_Z1P2;
    public Text score_to_display_Z1P3;
    public Text score_to_display_Z2P1;
    public Text score_to_display_Z2P2;
    public Text score_to_display_Z2P3;

    private int score_valueZ1P1 = 0;
    private int score_valueZ1P2 = 0;
    private int score_valueZ1P3 = 0;
    private int score_valueZ2P1 = 0;
    private int score_valueZ2P2 = 0;
    private int score_valueZ2P3 = 0;

    [SerializeField] private Text has_doublejump;
    [SerializeField] private GameObject logoDoubleJump;
    [SerializeField] private Text has_lure;
    [SerializeField] private GameObject logoLure;
    [SerializeField] private Text has_ball;
    [SerializeField] private Text has_shrink;


    // Use this for initialization
    void Start() {
        Time.timeScale = 1;
        CanvasPause = GetComponent<Canvas>();
        CanvasPause.enabled = false;

        PausePanel = CanvasPause.transform.GetChild(0).gameObject;
        OptionsPanel = CanvasPause.transform.GetChild(1).gameObject;
        ExitPanel = CanvasPause.transform.GetChild(2).gameObject;

        Cursor.lockState = CursorLockMode.Locked;
        LoadingCanva = GameObject.Find("LoadingScreen");
        UnpauseGame();
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetButtonDown("Cancel") && !VictorySwitch.Victory) {
            if (!Paused) {
                PauseGame(true);
                //showPaused();
            }
            else if (Paused) {
                UnpauseGame();
            }
        }
    }

    public void UnpauseGame() {
        if (!LoadingCanva.transform.GetChild(0).gameObject.activeInHierarchy) {
            //================================================
            SoundController.instance.SelectHUD("PauseDisabled");
            //================================================
            PausePanel.SetActive(false);
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

            if (showMenu) {
                PausePanel.SetActive(true);
                CanvasPause.enabled = true;
                transform.GetChild(0).gameObject.SetActive(true);
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
        CountCollectableYokai();
        score_to_display_Z1P1.text = "Z1P1 : " + score_valueZ1P1;
        score_to_display_Z1P2.text = "Z1P2 : " + score_valueZ1P2;
        score_to_display_Z1P3.text = "Z1P3 : " + score_valueZ1P3;
        score_to_display_Z2P1.text = "Z2P1 : " + score_valueZ2P1;
        score_to_display_Z2P2.text = "Z2P2 : " + score_valueZ2P2;
        score_to_display_Z2P3.text = "Z2P3 : " + score_valueZ2P3;

        bool has_power = Game.playerData.power_jump;
        logoDoubleJump.SetActive(has_power);
        has_doublejump.text = "Double Jump : ";

        has_power = Game.playerData.power_lure;
        logoLure.SetActive(has_power);
        has_lure.text = "Lure : ";

        has_power = Game.playerData.power_ball;
        has_ball.text = "Ball Form : "; //+ has_power;

        has_power = Game.playerData.power_shrink;
        has_shrink.text = "Shrink : ";// + has_power;
    }

    public void CountCollectableYokai() {

        score_valueZ1P1 = 0;
        score_valueZ1P2 = 0;
        score_valueZ1P3 = 0;
        score_valueZ2P1 = 0;
        score_valueZ2P2 = 0;
        score_valueZ2P3 = 0;

        for (int i = 0; i <30; i++) {
            if (Game.playerData.yokaiRemainingZ1P1[i] == -1) { score_valueZ1P1++; }
            if (Game.playerData.yokaiRemainingZ1P2[i] == -1) { score_valueZ1P2++; }
            if (Game.playerData.yokaiRemainingZ1P3[i] == -1) { score_valueZ1P3++; }
            if (Game.playerData.yokaiRemainingZ2P1[i] == -1) { score_valueZ2P1++; }
            if (Game.playerData.yokaiRemainingZ2P2[i] == -1) { score_valueZ2P2++; }
            if (Game.playerData.yokaiRemainingZ2P3[i] == -1) { score_valueZ2P3++; }
        }
    }
}
