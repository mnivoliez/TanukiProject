using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public class LianaGenerator : EditorWindow {

    private static List<GameObject> lianaObjects;
    private float lianaLenght;
    private string lenght;
    private static GameObject lianaMesh;
    private bool autoLenght;

    [MenuItem("Generate/All Liana")]
    public static void GenerateAllLiana() {
        lianaObjects = new List<GameObject>();

        GameObject[] objectsFinded = GameObject.FindGameObjectsWithTag("Liana");
        foreach (GameObject objectFinded in objectsFinded) {
            lianaObjects.Add(objectFinded);
        }

        EditorWindow.GetWindow(typeof(LianaGenerator));
    }

    [MenuItem("Generate/Selected Liana")]
    public static void GenerateSelectedLiana() {
        lianaObjects = new List<GameObject>();

        GameObject[] selectedObjects = Selection.gameObjects;
        foreach (GameObject selectedObject in selectedObjects) {
            if (selectedObject.tag == "Liana") {
                lianaObjects.Add(selectedObject);
            }
        }

        EditorWindow.GetWindow(typeof(LianaGenerator));
    }

    void Awake() {
        autoLenght = false;
        lenght = "";
    }

    private void Generate() {
        Vector3 sizeLianaMesh = lianaMesh.GetComponent<MeshRenderer>().bounds.size;

        float.TryParse(lenght, out lianaLenght);

        if (lianaLenght == 0) {
            lianaLenght = 10;
        }

        float partSizeLianaMesh = sizeLianaMesh.y * 0.2f;
        int nbLianaPart = (int)(lianaLenght / (sizeLianaMesh.y - partSizeLianaMesh));

        foreach (GameObject liana in lianaObjects) {
            Rigidbody rgbody = liana.AddComponent<Rigidbody>();
            FixedJoint hgJoint = liana.AddComponent<FixedJoint>();
            Rigidbody bodyToConnect = rgbody;

            if (autoLenght) {
                nbLianaPart = AutoSize(liana, sizeLianaMesh.y);
            }

            float positionYLianaPart = 0;
            for (int i = 0; i < nbLianaPart; i++) {
                GameObject lianaPart = Instantiate(lianaMesh, liana.transform.position, lianaMesh.transform.rotation);
                lianaPart.transform.parent = liana.transform;
                lianaPart.transform.localPosition = new Vector3(0, positionYLianaPart, 0);
                lianaPart.GetComponent<SpringJoint>().connectedBody = bodyToConnect;
                bodyToConnect = lianaPart.GetComponent<Rigidbody>();
                positionYLianaPart -= sizeLianaMesh.y - partSizeLianaMesh;
            }
        }
    }

    void OnGUI() {

        lianaMesh = EditorGUILayout.ObjectField("Find liana mesh", lianaMesh, typeof(GameObject)) as GameObject;
        autoLenght = EditorGUILayout.Toggle("Auto lenght liana", autoLenght);
        EditorGUI.BeginDisabledGroup(autoLenght);
        lenght = EditorGUILayout.TextField("Lenght liana", lenght);
        EditorGUI.EndDisabledGroup();


        if (GUILayout.Button("OK")) {
            ResetLiana();
            Generate();
            Close();
        }
    }

    private int AutoSize(GameObject liana, float sizeLianaMesh) {
        int nbLianaPart = 0;
        RaycastHit hit;
        float lenght = 0;
        float partSizeLianaMesh = sizeLianaMesh * 0.2f;

        if (Physics.Raycast(liana.transform.position, Vector3.down, out hit, 30)) {
            lenght = hit.distance - 0.5f;
        } else {
            lenght = 10;
        }

        nbLianaPart = (int)(lenght / (sizeLianaMesh - partSizeLianaMesh));

        return nbLianaPart;
    }

    private static void ResetLiana() {
        foreach (GameObject liana in lianaObjects) {
            while(liana.transform.childCount > 0) {
                foreach (Transform child in liana.transform) {
                    DestroyImmediate(child.gameObject);
                }
            }

            Component[] allComponents;
            allComponents = liana.GetComponents(typeof(Component));
            if (allComponents.Length > 1) {

                foreach (Component component in allComponents) {
                    if (component.GetType() != typeof(Transform)) {
                        if (component.GetType() == typeof(Rigidbody)) {
                            foreach (Component componentForJoint in allComponents) {
                                if (componentForJoint.GetType() == typeof(FixedJoint)) {
                                    DestroyImmediate(componentForJoint);
                                }
                            }
                        }
                        DestroyImmediate(component);
                    }  
                }
            }
        }
    }
}
