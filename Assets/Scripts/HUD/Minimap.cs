using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    //public Transform player;
    private GameObject player;
    private Canvas MinimapCanvas;

    void Start() {

        MinimapCanvas = GetComponent<Canvas>();
        //MinimapCanvas.enabled = false;

        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player.name);
    }

    private void LateUpdate() {
        //Vector3 newPosition = PlayerPrefab.position;
        //GameObject player = GameObject.Find("Player");
        //player = GameObject.FindGameObjectsWithTag("Player");

        Vector3 newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
    }
}
