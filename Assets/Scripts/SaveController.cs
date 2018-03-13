using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{

    public static SaveController _controller;
    private string player_save_path = Application.persistentDataPath + "/player.dat";
    private string scene_save_path;

    // Use this for initialization
    void Awake()
    {
        var scene_path = Application.persistentDataPath + "/" + SceneManager.GetActiveScene().name + ".dat";
        if (_controller == null)
        {
            DontDestroyOnLoad(gameObject);
            this.scene_save_path = scene_path;
            _controller = this;
        }
        else if (_controller != this)
        {
            _controller.scene_save_path = scene_path;
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Save()
    {
        SavePlayer();

    }

    private void SavePlayer()
    {
        GameObject koda = GameObject.FindGameObjectWithTag("Player");
        PlayerHealth koda_health = koda.GetComponent<PlayerHealth>();

        PlayerData playerData = new PlayerData();
        playerData.hp = koda_health.GetHealthCurrent();
        playerData.hp_max = koda_health.GetHealthMax();
        playerData.check_point = koda_health.GetRespawnPointPosition();
        playerData.current_scene = SceneManager.GetActiveScene().name;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(player_save_path, FileMode.Create);
        bf.Serialize(file, playerData);
        file.Close();
    }

    private void SaveLevel()
    {

    }

    public void Load()
    {
        LoadPlayer();
    }

    public void LoadPlayer()
    {
        if (File.Exists(player_save_path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(player_save_path, FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);

            GameObject koda = GameObject.FindGameObjectWithTag("Player");
            PlayerHealth koda_health = koda.GetComponent<PlayerHealth>();
            koda_health.SetHealthMax(data.hp_max);
            koda_health.SetCurrentHealth(data.hp);

            if (SceneManager.GetActiveScene().name == data.current_scene)
            {
                koda.transform.position = data.check_point.position;
                koda_health.SetRespawnPointPosition(data.check_point.position);
            }
        }
    }

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
    public string current_scene;
    public Transform check_point;
    public float hp;
    public float hp_max;
}

[Serializable]
class LevelData
{
    public List<Transform> _enemiesPos;
}