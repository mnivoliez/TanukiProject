using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable] // Tells Unity that this script can be serialized—in other words, that we can save all the variables in this script. MUST BE IN ALL SCRIPTS THAT NEEDS TO BE SAVED !
public class Game {

    public static Game current;
    //public Zone_Level = Zone; // Need to know the current zone the player is. We need a Zone script that handles the state of the zone, the checkpoints, etc ...
    //public Character Koda; // Need to know the state Koda is in : His unlocked powers, HP remaining, current checkpoint unlocked, the amount of Yokai killed, ... Script required !
    private int slot;
    public var scene_path;

    void Awake()
    {
        scene_path = Application.persistentDataPath + "/savedGames_slot_" + slot + ".gs";

        if (current == null)
        {
            DontDestroyOnLoad(gameObject);
            this.scene_save_path = scene_path;
            current = this;
        }
        else if (current != this)
        {
            current.scene_save_path = scene_path;
            Destroy(gameObject);
        }
    }

    public Game() {
        //Zone = new Zone_Level();
        Koda = new Player();
        Level_Info = new LevelData();
    }

}

public class Player{

    GameObject koda = GameObject.FindGameObjectWithTag("Player");
    //PlayerHealth koda_health = koda.GetComponent<PlayerHealth>();

    PlayerData playerData = new PlayerData();

    playerData.hp = koda.GetHealthCurrent();
    playerData.hp_max = koda.GetHealthMax();
    playerData.caught_Yokai = koda.GetCaughtYokai();
    
    playerData.check_point = koda.GetRespawnPointPosition();
    playerData.current_scene = SceneManager.GetActiveScene().name;

    playerData.power_jump = koda.GetPowerJump();
    playerData.power_ball = koda.GetPowerBall();
    playerData.power_shrink = koda.GetPowerShrink();
}

[Serializable]
class SaveData
{
    public PlayerData _player;
    public Dictionary<String, LevelData> _levels;
}

[Serializable]
class PlayerData
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