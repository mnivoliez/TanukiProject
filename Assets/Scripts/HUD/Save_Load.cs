using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Save_Load {

    public static List<Game> savedGames = new List<Game>(); // Create a list of type "Game" and call it "savedGames" (Needto create a public class Game that will handle the state of the overall game)

    public static void Save() {
        savedGames.Add(Game.current); // Adds our current game to our list of saved games.
        BinaryFormatter bf = new BinaryFormatter(); // Willhandle the serialization work.
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd"); // Pathwway to a new file that we can send data to.
        bf.Serialize(file, Save_Load.savedGames);
        file.Close();
    }

    public static void Load() {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            Save_Load.savedGames = (List<Game>)bf.Deserialize(file); // Check https://unity3d.com/fr/learn/tutorials/topics/scripting/persistence-saving-and-loading-data video at 44:40
            file.Close();
        }
    }
}
