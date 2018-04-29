using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;


public static class Game {

    public static GameObject koda;
    public static PlayerHealth koda_health;
    public static KodaController koda_power;

    public static int selected_slot = 0;
    public static string scene_path;

    public static PlayerData playerData;

    public static int score_valueZ1P1 = 0;
    public static int score_valueZ1P2 = 0;
    public static int score_valueZ1P3 = 0;
    public static int score_valueZ2P1 = 0;
    public static int score_valueZ2P2 = 0;
    public static int score_valueZ2P3 = 0;

    public static void PreSave_Game_and_Save() {
        Update_Game();
        Save();
    }

    public static void Save() {
        BinaryFormatter bf = new BinaryFormatter(); // Will handle the serialization work.
        FileStream file = File.Create(scene_path); // Pathwway to a new file that we can send data to.
        bf.Serialize(file, playerData);
        file.Close();
        Debug.Log("Game Saved !");
    }

    public static void Load() {
        if (File.Exists(Application.persistentDataPath + "/savedGames_slot_" + selected_slot.ToString() + ".gs")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames_slot_" + selected_slot.ToString() + ".gs", FileMode.Open);
            playerData = (PlayerData)bf.Deserialize(file); // Check https://unity3d.com/fr/learn/tutorials/topics/scripting/persistence-saving-and-loading-data video at 44:40
            file.Close();
        }
    }

    public static void Load_and_Post_Load() {
        if (SceneManager.GetActiveScene().name == playerData.current_scene) {
            //////////////////////////////////////////////////////////
            koda = GameObject.FindGameObjectWithTag("Player");
            if (koda != null) { 
                koda_health = koda.GetComponent<PlayerHealth>();
                koda_power = koda.GetComponent<KodaController>();
                //////////////////////////////////////////////////////////
                Load();

                koda_health.SetHealthCurrent(playerData.hp);
                koda_health.SetHealthMax(playerData.hp_max);
                koda_power.SetRespawnPointPosition(playerData.check_point_x, playerData.check_point_y, playerData.check_point_z);
                //SceneManager.SetActiveScene(playerData.current_scene); // Working ONLY if the saved scene IS IN the SceneManager !

                koda_power.SetPowerJump(playerData.lightBoss1);
                koda_power.SetPowerLure(playerData.lightBoss2);
                koda_power.SetPowerBall(playerData.power_ball);
                koda_power.SetPowerShrink(playerData.power_shrink);

                //koda_score.SetnbYokai(playerData.caught_yokai);
                selected_slot = playerData.selected_slot;

                Debug.Log("Game Loaded !");
            }
        }
        else {
            Debug.Log("Trying to load a scene that is not supposed to exist in this game.");
        }
    }

    public static void Update_Game() {
        //////////////////////////////////////////////////////////
        koda = GameObject.FindGameObjectWithTag("Player");
        if (koda != null) {
            koda_health = koda.GetComponent<PlayerHealth>();
            koda_power = koda.GetComponent<KodaController>();
            //////////////////////////////////////////////////////////

            playerData.hp_max = koda_health.GetHealthMax();
            playerData.hp = koda_health.GetHealthCurrent();

            Transform check_point = koda_power.GetRespawnPointPosition();
            playerData.check_point_x = check_point.position.x;
            playerData.check_point_y = check_point.position.y;
            playerData.check_point_z = check_point.position.z;

            playerData.current_scene = SceneManager.GetActiveScene().name;

            if (!playerData.lightBoss1) { playerData.power_jump = koda_power.GetPowerJump(); }
            if (!playerData.lightBoss2) { playerData.power_lure = koda_power.GetPowerLure(); }
            playerData.power_ball = koda_power.GetPowerBall();
            playerData.power_shrink = koda_power.GetPowerShrink();


            CountCollectableYokai();
            playerData.caught_yokaiZ1P1 = score_valueZ1P1;
            playerData.caught_yokaiZ1P2 = score_valueZ1P2;
            playerData.caught_yokaiZ1P3 = score_valueZ1P3;
            playerData.caught_yokaiZ2P1 = score_valueZ2P1;
            playerData.caught_yokaiZ2P2 = score_valueZ2P2;
            playerData.caught_yokaiZ2P3 = score_valueZ2P3;

            switch (playerData.current_scene) {
                case "Z1-P1-complete":
                    //playerData.caught_yokaiZ1P1 = koda_score.GetnbYokai();
                    playerData.caught_yokai = score_valueZ1P1;
                    break;

                case "Z1-P2-complete":
                    //playerData.caught_yokaiZ1P2 = koda_score.GetnbYokai();
                    playerData.caught_yokai = score_valueZ1P2;
                    break;

                case "Z1-P3-complete":
                    //playerData.caught_yokaiZ1P3 = koda_score.GetnbYokai();
                    playerData.caught_yokai = score_valueZ1P3;
                    break;

                case "Z2-P1-complete":
                    //playerData.caught_yokaiZ2P1 = koda_score.GetnbYokai();
                    playerData.caught_yokai = score_valueZ2P1;
                    break;

                case "Z2-P2-complete":
                    //playerData.caught_yokaiZ2P2 = koda_score.GetnbYokai();
                    playerData.caught_yokai = score_valueZ2P2;
                    break;

                case "Z2-P3-complete":
                    //playerData.caught_yokaiZ2P3 = koda_score.GetnbYokai();
                    playerData.caught_yokai = score_valueZ2P3;
                    break;

                default:
                    break;
            }

            playerData.selected_slot = selected_slot;

            scene_path = Application.persistentDataPath + "/savedGames_slot_" + playerData.selected_slot.ToString() + ".gs";
        }
    }

    public static void yokai_Collectable_Caught(int yokaiID) {
        switch (playerData.current_scene) {
            case "Z1-P1-complete":
                playerData.yokaiRemainingZ1P1[yokaiID] = -1;
                break;

            case "Z1-P2-complete":
                playerData.yokaiRemainingZ1P2[yokaiID] = -1;
                break;

            case "Z1-P3-complete":
                playerData.yokaiRemainingZ1P3[yokaiID] = -1;
                break;

            case "Z2-P1-complete":
                playerData.yokaiRemainingZ2P1[yokaiID] = -1;
                break;

            case "Z2-P2-complete":
                playerData.yokaiRemainingZ2P2[yokaiID] = -1;
                break;

            case "Z2-P3-complete":
                playerData.yokaiRemainingZ2P3[yokaiID] = -1;
                break;

            default:
                break;
        }
    }

    public static void CountCollectableYokai() {
        
        score_valueZ1P1 = 0;
        score_valueZ1P2 = 0;
        score_valueZ1P3 = 0;
        score_valueZ2P1 = 0;
        score_valueZ2P2 = 0;
        score_valueZ2P3 = 0;

        for (int i = 0; i < 30; i++) {
            if (playerData.yokaiRemainingZ1P1[i] == -1) { score_valueZ1P1++; }
            if (playerData.yokaiRemainingZ1P2[i] == -1) { score_valueZ1P2++; }
            if (playerData.yokaiRemainingZ1P3[i] == -1) { score_valueZ1P3++; }
            if (playerData.yokaiRemainingZ2P1[i] == -1) { score_valueZ2P1++; }
            if (playerData.yokaiRemainingZ2P2[i] == -1) { score_valueZ2P2++; }
            if (playerData.yokaiRemainingZ2P3[i] == -1) { score_valueZ2P3++; }
        }
    }

    public static void Reset_Game() {
        if (SceneManager.GetActiveScene().name == "Zone Tuto") {
            playerData.hp = 3.0f;
            playerData.hp_max = 3.0f;

            playerData.selected_slot = 0;

            playerData.current_scene = "Zone Tuto";
            playerData.check_point_x = 124.1946f;
            playerData.check_point_y = 2.602453f;
            playerData.check_point_z = -39.7136f;

            playerData.power_jump = false;
            playerData.power_lure = false;
            playerData.power_ball = false;
            playerData.power_shrink = false;

            playerData.caught_yokaiZ1P1 = 0;
            playerData.caught_yokaiZ1P2 = 0;
            playerData.caught_yokaiZ1P3 = 0;
            playerData.caught_yokaiZ2P1 = 0;
            playerData.caught_yokaiZ2P2 = 0;
            playerData.caught_yokaiZ2P3 = 0;

            playerData.caught_yokai = 0;

            playerData.lightBoss1 = false;
            playerData.Boss1KO = false;
            playerData.lightBoss2 = false;
            playerData.Boss2KO = false;

            playerData.yokaiRemainingZ1P1 = new int[30]; //3
            Reset_Yokai_Scene(playerData.yokaiRemainingZ1P1, playerData.yokaiRemainingZ1P1.Length);
            playerData.yokaiRemainingZ1P2 = new int[30]; //14
            Reset_Yokai_Scene(playerData.yokaiRemainingZ1P2, playerData.yokaiRemainingZ1P2.Length);
            playerData.yokaiRemainingZ1P3 = new int[30]; //7
            Reset_Yokai_Scene(playerData.yokaiRemainingZ1P3, playerData.yokaiRemainingZ1P3.Length);
            playerData.yokaiRemainingZ2P1 = new int[30]; //1
            Reset_Yokai_Scene(playerData.yokaiRemainingZ2P1, playerData.yokaiRemainingZ2P1.Length);
            playerData.yokaiRemainingZ2P2 = new int[30]; //20
            Reset_Yokai_Scene(playerData.yokaiRemainingZ2P2, playerData.yokaiRemainingZ2P2.Length);
            playerData.yokaiRemainingZ2P3 = new int[30]; //12
            Reset_Yokai_Scene(playerData.yokaiRemainingZ2P3, playerData.yokaiRemainingZ2P3.Length);
            playerData.yokaiRemainingTEST = new int[30]; //3
            Reset_Yokai_Scene(playerData.yokaiRemainingTEST, playerData.yokaiRemainingTEST.Length);

            scene_path = Application.persistentDataPath + "/savedGames_slot_" + playerData.selected_slot.ToString() + ".gs";
        }
    }

    public static void Reset_Yokai_Scene(int[] yokaiRemaining, int nbrYokai) {
        for(int i = 0; i < nbrYokai; i++) {
            yokaiRemaining[i] = i;
        }
    }
}

[Serializable] // Tells Unity that this script can be serialized—in other words, that we can save all the variables in this script. MUST BE IN ALL SCRIPTS THAT NEEDS TO BE SAVED !
public struct PlayerData {
    public float hp;
    public float hp_max;

    public int selected_slot;

    public string current_scene;
    public float check_point_x;
    public float check_point_y;
    public float check_point_z;

    public bool power_jump;
    public bool power_lure;
    public bool power_ball;
    public bool power_shrink;

    public int caught_yokaiZ1P1;
    public int caught_yokaiZ1P2;
    public int caught_yokaiZ1P3;
    public int caught_yokaiZ2P1;
    public int caught_yokaiZ2P2;
    public int caught_yokaiZ2P3;

    public int caught_yokai;

    public bool lightBoss1;
    public bool Boss1KO;
    public bool lightBoss2;
    public bool Boss2KO;

    public int[] yokaiRemainingZ1P1;
    public int[] yokaiRemainingZ1P2;
    public int[] yokaiRemainingZ1P3;
    public int[] yokaiRemainingZ2P1;
    public int[] yokaiRemainingZ2P2;
    public int[] yokaiRemainingZ2P3;
    public int[] yokaiRemainingTEST;
}