using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class LianaGenerator : EditorWindow {

    private static GameObject[] lianaObjects;

    [MenuItem("Generate/Liana")]
    public static void GenerateLiana() {
        Generate();
    }

    private static void Generate() {

        lianaObjects = GameObject.FindGameObjectsWithTag("Liana");

        foreach (GameObject gameObject in lianaObjects) {
            BoxCollider sc = gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
        }
    }
}
