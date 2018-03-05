using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    //public Transform player;
    private Canvas CanvasMinimap;

    void Start() {
        CanvasMinimap = GetComponent<Canvas>();
}

    private void LateUpdate() {
        //Vector3 newPosition = PlayerPrefab.position;
        GameObject player = GameObject.Find("Player");
        Vector3 newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
    }
}
