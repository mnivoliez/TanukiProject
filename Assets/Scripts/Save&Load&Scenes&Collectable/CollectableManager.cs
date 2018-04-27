using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectableManager : MonoBehaviour {
    public static GameObject LoadingScreen;

    private GameObject KiyomoriLightHiyoribou;
    private GameObject[] gOList;
    private int yokaiGeneralCurrentScene = 0;
    //private GameObject[] yokais;

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if(SceneManager.GetActiveScene().name == "Zone Tuto") {
            Game.Reset_Game();
            Game.PreSave_Game_and_Save();
            StartCoroutine(SavingLogo());
        }

        Game.Load();
        
        if (SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "Zone Tuto" && SceneManager.GetActiveScene().name != "Sandbox GGS" && SceneManager.GetActiveScene().name != "FinTuto") {
            FindYokaiGeneralAndSetID();
            FindYokaiGeneralAndDisable();
            Game.Update_Game();
            isPurified(Game.playerData.current_scene);
        }
    }

    IEnumerator SavingLogo() {
        LoadingScreen = GameObject.Find("LoadingScreen");
        if (LoadingScreen != null) {
            LoadingScreen.transform.GetChild(1).gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            LoadingScreen.transform.GetChild(1).gameObject.SetActive(false);
        }        
    }

    void isPurified(string currentScene) {
        KiyomoriLightHiyoribou = GameObject.Find("Kiyomori_Light_Hiyoribou");

        switch (currentScene) {
            case "Z1-P1-complete":
                if (Game.playerData.lightBoss1) {
                    KiyomoriLightHiyoribou.GetComponent<Light>().range = 1200;
                    KiyomoriLightHiyoribou.GetComponent<LanternController>()._min_radius = 1200;
                }
                break;

            case "Z1-P2-complete":
                if (Game.playerData.lightBoss1) {
                    KiyomoriLightHiyoribou.GetComponent<Light>().range = 1200;
                    KiyomoriLightHiyoribou.GetComponent<LanternController>()._min_radius = 1200;
                }
                break;

            case "Z1-P3-complete":
                if (Game.playerData.lightBoss1) {
                    KiyomoriLightHiyoribou.GetComponent<Light>().range = 1200;
                    KiyomoriLightHiyoribou.GetComponent<LanternController>()._min_radius = 1200;
                }
                break;

            case "Z2-P1-complete":
                if (Game.playerData.lightBoss2) {
                    KiyomoriLightHiyoribou.GetComponent<Light>().range = 1200;
                    KiyomoriLightHiyoribou.GetComponent<LanternController>()._min_radius = 1200;
                }
                break;

            case "Z2-P2-complete":
                if (Game.playerData.lightBoss2) {
                    KiyomoriLightHiyoribou.GetComponent<Light>().range = 1200;
                    KiyomoriLightHiyoribou.GetComponent<LanternController>()._min_radius = 1200;
                }
                break;

            case "Z2-P3-complete":
                if (Game.playerData.lightBoss2) {
                    KiyomoriLightHiyoribou.GetComponent<Light>().range = 1200;
                    KiyomoriLightHiyoribou.GetComponent<LanternController>()._min_radius = 1200;
                }
                break;

            default:
                break;
        }
    }

    void FindYokaiGeneralAndSetID() {
        yokaiGeneralCurrentScene = 0;
        gOList = (GameObject[])FindObjectsOfType(typeof(GameObject));

        for (int i = 0; i < gOList.Length; i++) {
            if (gOList[i].name.Contains("Yokai_General")) {
                gOList[i].GetComponent<YokaiAIv2Controller>().yokaiID = yokaiGeneralCurrentScene;
                yokaiGeneralCurrentScene = yokaiGeneralCurrentScene + 1;
            }
        }
    }

    void FindYokaiGeneralAndDisable() {
        gOList = (GameObject[])FindObjectsOfType(typeof(GameObject));

        for (int i = 0; i < gOList.Length; i++) {
            if (gOList[i].name.Contains("Yokai_General")) {
                DisableYokai(SceneManager.GetActiveScene().name, gOList[i].GetComponent<YokaiAIv2Controller>().yokaiID, i);
            }
        }
    }

    void DisableYokai(string currentScene, int yokaiID, int gONbr) {
        switch (currentScene) {
            case "Z1-P1-complete":
                if (Game.playerData.yokaiRemainingZ1P1[yokaiID] != yokaiID) {
                    gOList[gONbr].SetActive(false);
                }
                break;

            case "Z1-P2-complete":
                if (Game.playerData.yokaiRemainingZ1P2[yokaiID] != yokaiID) {
                    gOList[gONbr].SetActive(false);
                }
                break;

            case "Z1-P3-complete":
                if (Game.playerData.yokaiRemainingZ1P3[yokaiID] != yokaiID) {
                    gOList[gONbr].SetActive(false);
                }
                break;

            case "Z2-P1-complete":
                if (Game.playerData.yokaiRemainingZ2P1[yokaiID] != yokaiID) {
                    gOList[gONbr].SetActive(false);
                }
                break;

            case "Z2-P2-complete":
                if (Game.playerData.yokaiRemainingZ2P2[yokaiID] != yokaiID) {
                    gOList[gONbr].SetActive(false);
                }
                break;

            case "Z2-P3-complete":
                if (Game.playerData.yokaiRemainingZ2P3[yokaiID] != yokaiID) {
                    gOList[gONbr].SetActive(false);
                }
                break;

            case "Scene_AirStream":
                if (Game.playerData.yokaiRemainingTEST[yokaiID] != yokaiID) {
                    gOList[gONbr].SetActive(false);
                }
                break;

            default:
                break;
        }
    }
}
