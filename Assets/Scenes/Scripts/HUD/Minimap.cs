using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    //public Transform player;
    private GameObject player;
    private Canvas MinimapCanvas;

    void Start() {

        //MinimapCanvas = GetComponent<Canvas>();

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate() {
        //Vector3 newPosition = PlayerPrefab.position;
        //GameObject player = GameObject.Find("Player");
        //player = GameObject.FindGameObjectsWithTag("Player");

        //MinimapCanvas.enabled = true;
        Vector3 newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);        
    }
}
