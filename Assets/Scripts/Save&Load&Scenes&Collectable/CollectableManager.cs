using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectableManager : MonoBehaviour {

    private GameObject[] gOList;
    private int yokaiGeneralCurrentScene = 0;
    //private GameObject[] yokais;

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        Game.Reset_Game(); // Will reset ONLY if the current scene is "Zone Tuto"
    }

    void FirstVisit(string currentScene) {
        switch (currentScene) {
            case "Z1-P1-complete":
                Game.playerData.visited_Z1P1 = true;
                break;

            case "Z1-P2-complete":
                Game.playerData.visited_Z1P2 = true;
                break;

            case "Z1-P3-complete":
                Game.playerData.visited_Z1P3 = true;
                break;

            case "Z2-P1-complete":
                Game.playerData.visited_Z2P1 = true;
                break;

            case "Z2-P2-complete":
                Game.playerData.visited_Z2P2 = true;
                break;

            case "Z2-P3-complete":
                Game.playerData.visited_Z2P3 = true;
                break;

            case "Scene_AirStream":
                Game.playerData.visited_test = true;
                break;
        }
    }

    void FindYokaiGeneralAndDisableIfPossible(int yokaiCaught) {
        yokaiGeneralCurrentScene = 0;
        gOList = (GameObject[])FindObjectsOfType(typeof(GameObject));

        for (int i = 0; i < gOList.Length; i++) {
            if (gOList[i].name.Contains("Yokai_General")) {
                yokaiGeneralCurrentScene = yokaiGeneralCurrentScene + 1;
            }
        }

        if(yokaiCaught >= yokaiGeneralCurrentScene) {
            for (int i = 0; i < gOList.Length; i++) {
                if (gOList[i].name.Contains("Yokai_General") && gOList[i].GetComponent<YokaiAIv2Controller>().canBeKilled) {
                    gOList[i].SetActive(false);
                }
            }
        }
        else {
            int yokaiToRemove = yokaiGeneralCurrentScene - yokaiCaught;
        }
    }
}
