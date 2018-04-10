using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    private GameObject player;
    private Canvas MinimapCanvas;
    private Canvas PauseCanvas;
    private Vector3 newPosition;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    private void LateUpdate() {

        newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);        
    }
}
