using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable] // Tells Unity that this script can be serialized—in other words, that we can save all the variables in this script. MUST BE IN ALL SCRIPTS THAT NEEDS TO BE SAVED !
public class Game {

    //public static Game current;
    //public Zone_Level = Zone; // Need to know the current zone the player is. We need a Zone script that handles the state of the zone, the checkpoints, etc ...
    //public Character Koda; // Need to know the state Koda is in : His unlocked powers, HP remaining, current checkpoint unlocked, the amount of Yokai killed, ... Script required !

    public int selected_slot;
    public string scene_path;

    /*public GameObject koda;
    public PlayerHealth koda_health;
    public KodaController koda_power;*/

    PlayerData playerData = new PlayerData();

    /*void Awake() {
        if (current == null) {
            DontDestroyOnLoad(gameObject);
            //this.scene_save_path = scene_path;
            current = this;
        }
        else if (current != this) {
            //current.scene_save_path = scene_path;
            Destroy(gameObject);
        }
    }*/

    /*private void Start() {
        koda = GameObject.FindGameObjectWithTag("Player");
        koda_health = koda.GetComponent<PlayerHealth>();
        koda_power = koda.GetComponent<KodaController>();
        //my_Game();
    }*/

    public void PreSave_Game(float hp_max, float hp_current, float check_point_x, float check_point_y, float check_point_z, string current_scene, bool power_jump, bool power_ball, bool power_shrink) {

        scene_path = Application.persistentDataPath + "/savedGames_slot_" + selected_slot.ToString() + ".gs";

        playerData.hp_max = hp_max;
        playerData.hp = hp_current;

        //playerData.caught_Yokai = koda_power.GetCaughtYokai();

        playerData.check_point_x = check_point_x;
        playerData.check_point_y = check_point_y;
        playerData.check_point_z = check_point_z;
        playerData.current_scene = current_scene;

        playerData.power_jump = power_jump;
        playerData.power_ball = power_ball;
        playerData.power_shrink = power_shrink;
    }

}

[System.Serializable]
struct PlayerData {
    public float hp;
    public float hp_max;
    public int caught_yokai;

    public string current_scene;
    public float check_point_x;
    public float check_point_y;
    public float check_point_z;

    public bool power_jump;
    public bool power_ball;
    public bool power_shrink;
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
