using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable] // Tells Unity that this script can be serialized—in other words, that we can save all the variables in this script. MUST BE IN ALL SCRIPTS THAT NEEDS TO BE SAVED !
public class Game : MonoBehaviour
{

    public static Game current;
    //public Zone_Level = Zone; // Need to know the current zone the player is. We need a Zone script that handles the state of the zone, the checkpoints, etc ...
    //public Character Koda; // Need to know the state Koda is in : His unlocked powers, HP remaining, current checkpoint unlocked, the amount of Yokai killed, ... Script required !
    private int slot = 0;
    public string scene_path;
    public GameObject koda;
    public PlayerHealth koda_health;
    public KodaController koda_power;
    PlayerData playerData = new PlayerData();

    void Awake() {
        scene_path = Application.persistentDataPath + "/savedGames_slot_" + slot.ToString() + ".gs";

        if (current == null) {
            DontDestroyOnLoad(gameObject);
            //this.scene_save_path = scene_path;
            current = this;
        }
        else if (current != this) {
            //current.scene_save_path = scene_path;
            Destroy(gameObject);
        }
    }

    private void Start() {
        koda = GameObject.FindGameObjectWithTag("Player");
        koda_health = koda.GetComponent<PlayerHealth>();
        koda_power = koda.GetComponent<KodaController>();
        my_Game();
    }

    public void my_Game()
    {
        playerData.hp_max = koda_health.GetHealthMax();
        playerData.hp = koda_health.GetHealthCurrent();
        //playerData.caught_Yokai = koda_power.GetCaughtYokai();

        playerData.check_point = koda_power.GetRespawnPointPosition();
        playerData.current_scene = SceneManager.GetActiveScene().name;

        playerData.power_jump = koda_power.GetPowerJump();
        playerData.power_ball = koda_power.GetPowerBall();
        playerData.power_shrink = koda_power.GetPowerShrink();
    }

}

public class Player
{


}

[Serializable]
class SaveData
{
    public PlayerData _player;
    public Dictionary<String, LevelData> _levels;
}

[System.Serializable]
struct PlayerData
{
    public float hp;
    public float hp_max;
    public int caught_yokai;

    public string current_scene;
    public Transform check_point;

    public bool power_jump;
    public bool power_ball;
    public bool power_shrink;
}

[Serializable]
class LevelData
{
    public List<Transform> _enemiesPos;
}