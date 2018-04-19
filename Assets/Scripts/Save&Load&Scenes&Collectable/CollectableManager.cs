﻿using System.Collections;
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
        Debug.Log("I am trying !");
        Game.Reset_Game(); // Will reset ONLY if the current scene is "Zone Tuto"
        FindYokaiGeneralAndDisable();
    }

    void isPurified(string currentScene) {
        switch (currentScene) {
            case "Z1-P1-complete":
                if (Game.playerData.lightBoss1) {
                    //LIGHT
                }
                break;

            case "Z1-P2-complete":
                if (Game.playerData.lightBoss1) {
                    //LIGHT
                }
                break;

            case "Z1-P3-complete":
                if (Game.playerData.lightBoss1) {
                    //LIGHT
                }
                break;

            case "Z2-P1-complete":
                if (Game.playerData.lightBoss2) {
                    //LIGHT
                }
                break;

            case "Z2-P2-complete":
                if (Game.playerData.lightBoss2) {
                    //LIGHT
                }
                break;

            case "Z2-P3-complete":
                if (Game.playerData.lightBoss2) {
                    //LIGHT
                }
                break;
        }
    }

    void FindYokaiGeneralAndDisable() {
        yokaiGeneralCurrentScene = 0;
        gOList = (GameObject[])FindObjectsOfType(typeof(GameObject));

        for (int i = 0; i < gOList.Length; i++) {
            if (gOList[i].name.Contains("Yokai_General")) {
                DisableYokai(SceneManager.GetActiveScene().name, gOList[i].GetComponent<YokaiAIv2Controller>().yokaiID, i);
                Debug.Log("FoundYa !");
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

            case "AirStream":
                if (Game.playerData.yokaiRemainingTEST[yokaiID] != yokaiID) {
                    gOList[gONbr].SetActive(false);
                }
                break;
        }
    }
}
