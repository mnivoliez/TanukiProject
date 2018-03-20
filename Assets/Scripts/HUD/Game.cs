using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;


public class Game {

    public GameObject koda;
    public PlayerHealth koda_health;
    public KodaController koda_power;

    public int selected_slot = 0;
    public string scene_path;

    public PlayerData playerData = new PlayerData();

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

    public void PreSave_Game_and_Save() {
        //////////////////////////////////////////////////////////
        this.koda = GameObject.FindGameObjectWithTag("Player");
        this.koda_health = koda.GetComponent<PlayerHealth>();
        this.koda_power = koda.GetComponent<KodaController>();
        //////////////////////////////////////////////////////////

        this.playerData.hp_max = koda_health.GetHealthMax();
        this.playerData.hp = koda_health.GetHealthCurrent();

        //playerData.caught_Yokai = koda_power.GetCaughtYokai();

        Transform check_point = koda_power.GetRespawnPointPosition();
        this.playerData.check_point_x = check_point.position.x;
        this.playerData.check_point_y = check_point.position.y;
        this.playerData.check_point_z = check_point.position.z;

        this.playerData.current_scene = SceneManager.GetActiveScene().name;

        this.playerData.power_jump = koda_power.GetPowerJump();
        this.playerData.power_ball = koda_power.GetPowerBall();
        this.playerData.power_shrink = koda_power.GetPowerShrink();

        this.playerData.selected_slot = this.selected_slot;

        this.scene_path = Application.persistentDataPath + "/savedGames_slot_" + this.playerData.selected_slot.ToString() + ".gs";

        this.Save();
    }

    public void Save() {
        BinaryFormatter bf = new BinaryFormatter(); // Will handle the serialization work.
        FileStream file = File.Create(this.scene_path); // Pathwway to a new file that we can send data to.
        bf.Serialize(file, this.playerData);
        file.Close();
        Debug.Log("Game Saved !");
    }

    public void Load() {
        if (File.Exists(Application.persistentDataPath + "/savedGames_slot_" + selected_slot.ToString() + ".gs")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames_slot_" + selected_slot.ToString() + ".gs", FileMode.Open);
            this.playerData = (PlayerData)bf.Deserialize(file); // Check https://unity3d.com/fr/learn/tutorials/topics/scripting/persistence-saving-and-loading-data video at 44:40
            file.Close();
        }
    }

    public void Load_and_Post_Load() {
        //////////////////////////////////////////////////////////
        this.koda = GameObject.FindGameObjectWithTag("Player");
        this.koda_health = koda.GetComponent<PlayerHealth>();
        this.koda_power = koda.GetComponent<KodaController>();
        //////////////////////////////////////////////////////////
        this.Load();

        this.koda_health.SetHealthCurrent(this.playerData.hp);
        this.koda_health.SetHealthCurrent(this.playerData.hp_max);

        //koda_power.SetCaughtYokai(this.playerData.caught_yokai);

        this.koda_power.SetRespawnPointPosition(this.playerData.check_point_x, this.playerData.check_point_y, this.playerData.check_point_z);

        //SceneManager.SetActiveScene(this.playerData.current_scene); //Marche pas encore lol !

        this.koda_power.SetPowerJump(this.playerData.power_jump);
        this.koda_power.SetPowerBall(this.playerData.power_ball);
        this.koda_power.SetPowerShrink(this.playerData.power_shrink);

        this.selected_slot = this.playerData.selected_slot;

        Debug.Log("Game Loaded !");
    }

}

[Serializable] // Tells Unity that this script can be serialized—in other words, that we can save all the variables in this script. MUST BE IN ALL SCRIPTS THAT NEEDS TO BE SAVED !
public class PlayerData {
    public float hp;
    public float hp_max;
    public int caught_yokai;

    public int selected_slot;

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
