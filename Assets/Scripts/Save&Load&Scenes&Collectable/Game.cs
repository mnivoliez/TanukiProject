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
    public static PlayerCollectableController koda_score;

    public static int selected_slot = 0;
    public static string scene_path;

    public static PlayerData playerData;

    /*public static Game current;

    void Awake() {
        if (current == null) {
            DontDestroyOnLoad(gameObject);
            //this.scene_save_path = scene_path;
            current = this;
            //Debug.Log("Game Alive");
        }
        else if (current != this) {
            //current.scene_save_path = scene_path;
            Destroy(gameObject);
            //Debug.Log("Game Dead");
        }
    }

    private void Start() {
        this.koda = GameObject.FindGameObjectWithTag("Player");
        this.koda_health = koda.GetComponent<PlayerHealth>();
        this.koda_power = koda.GetComponent<KodaController>();
    }*/

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
            koda_health = koda.GetComponent<PlayerHealth>();
            koda_power = koda.GetComponent<KodaController>();
            koda_score = koda.GetComponent<PlayerCollectableController>();
            //////////////////////////////////////////////////////////
            Load();

            koda_health.SetHealthCurrent(playerData.hp);
            koda_health.SetHealthMax(playerData.hp_max);

            koda_power.SetRespawnPointPosition(playerData.check_point_x, playerData.check_point_y, playerData.check_point_z);

            //SceneManager.SetActiveScene(playerData.current_scene); // Working ONLY if the saved scene IS IN the SceneManager !

            koda_power.SetPowerJump(playerData.power_jump);
            koda_power.SetPowerLure(playerData.power_lure);
            koda_power.SetPowerBall(playerData.power_ball);
            koda_power.SetPowerShrink(playerData.power_shrink);

            koda_score.SetnbYokai(playerData.caught_yokai);
            selected_slot = playerData.selected_slot;

            Debug.Log("Game Loaded !");
        }

        else {
            //SceneManager.LoadScene(playerData.current_scene);
            //Load_and_Post_Load();
            Debug.Log("Trying to load a scene that is not supposed to exist in this game.");
        }
    }

    public static void Load_From_Main_Menue() {
        /*
         SceneManager.LoadScene(playerData.current_scene);
         Load_and_Post_Load()
         */
        Debug.Log("Will load ... Soon™.");
    }

    public static void Update_Game() {
        //////////////////////////////////////////////////////////
        koda = GameObject.FindGameObjectWithTag("Player");
        koda_health = koda.GetComponent<PlayerHealth>();
        koda_power = koda.GetComponent<KodaController>();
        koda_score = koda.GetComponent<PlayerCollectableController>();
        //////////////////////////////////////////////////////////

        playerData.hp_max = koda_health.GetHealthMax();
        playerData.hp = koda_health.GetHealthCurrent();

        Transform check_point = koda_power.GetRespawnPointPosition();
        playerData.check_point_x = check_point.position.x;
        playerData.check_point_y = check_point.position.y;
        playerData.check_point_z = check_point.position.z;

        playerData.current_scene = SceneManager.GetActiveScene().name;

        playerData.power_jump = koda_power.GetPowerJump();
        playerData.power_lure = koda_power.GetPowerLure();
        playerData.power_ball = koda_power.GetPowerBall();
        playerData.power_shrink = koda_power.GetPowerShrink();

        switch (playerData.current_scene) {
            case "Z1-P1-complete":
                playerData.caught_yokaiZ1P1 = koda_score.GetnbYokai();
                break;

            case "Z1-P2-complete":
                playerData.caught_yokaiZ1P2 = koda_score.GetnbYokai();
                break;

            case "Z1-P3-complete":
                playerData.caught_yokaiZ1P3 = koda_score.GetnbYokai();
                break;

            case "Z2-P1-complete":
                playerData.caught_yokaiZ2P1 = koda_score.GetnbYokai();
                break;

            case "Z2-P2-complete":
                playerData.caught_yokaiZ2P2 = koda_score.GetnbYokai();
                break;

            case "Z2-P3-complete":
                playerData.caught_yokaiZ2P3 = koda_score.GetnbYokai();
                break;

            case "Scene_AirStream":
                playerData.caught_yokai_test = koda_score.GetnbYokai();
                break;
        }

        playerData.caught_yokai = koda_score.GetnbYokai();
        playerData.selected_slot = selected_slot;

        scene_path = Application.persistentDataPath + "/savedGames_slot_" + playerData.selected_slot.ToString() + ".gs";
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
    public int caught_yokai_test;

    public int caught_yokai;

    public bool visited_Z1P1;
    public bool visited_Z1P2;
    public bool visited_Z1P3;
    public bool visited_Z2P1;
    public bool visited_Z2P2;
    public bool visited_Z2P3;
    public bool visited_test;
}

/*public class Player
{


}

[Serializable]
class SaveData
{
    public PlayerData _player;
    public Dictionary<String, LevelData> _levels;
}

[Serializable]
class LevelData
{
    public List<Transform> _enemiesPos;
}*/
