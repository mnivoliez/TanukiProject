using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class Save_Load {

    public static Game savedGames = new Game(); // Create a list of type "Game" and call it "savedGames" (Need to create a public class Game that will handle the state of the overall game)
    public static int selected_slot;

    public static void Save() {
        savedGames = Game.current; // Adds our current game to our list of saved games.
        BinaryFormatter bf = new BinaryFormatter(); // Will handle the serialization work.
        FileStream file = File.Create(savedGames.scene_path); // Pathwway to a new file that we can send data to.
        bf.Serialize(file, Save_Load.savedGames);
        file.Close();
    }

    public static void Load() {
        if (File.Exists(Application.persistentDataPath + "/savedGames_slot_" + selected_slot.ToString() + ".gs")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames_slot_" + selected_slot.ToString() + ".gs", FileMode.Open);
            Save_Load.savedGames = (Game) bf.Deserialize(file); // Check https://unity3d.com/fr/learn/tutorials/topics/scripting/persistence-saving-and-loading-data video at 44:40
            file.Close();
        }
    }
}
