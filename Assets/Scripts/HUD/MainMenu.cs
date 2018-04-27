using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    private Canvas CanvasMenu;

    private GameObject MainPanel;
    private GameObject OptionsPanel;
    private GameObject ExitPanel;

    public void Start() {
        CanvasMenu = GetComponent<Canvas>();
        MainPanel = CanvasMenu.transform.GetChild(0).gameObject;
        OptionsPanel = CanvasMenu.transform.GetChild(1).gameObject;
        ExitPanel = CanvasMenu.transform.GetChild(2).gameObject;
    }

    public void Load_Game() {
        Game.Load();
        CanvasMenu.GetComponent<LoadSceneManager>().LoadByIndexMM(Game.playerData.current_scene);
        MainPanel.SetActive(false);
        //Debug.Log("Will load ... Soon™.");
    }

    public void Load_Zone_Tuto() {
        CanvasMenu.GetComponent<LoadSceneManager>().LoadByIndexMM("Zone Tuto");
        MainPanel.SetActive(false);
    }
}
