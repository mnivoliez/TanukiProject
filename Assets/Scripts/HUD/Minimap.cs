using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    private GameObject player;
    private Canvas MinimapCanvas;
    private Canvas PauseCanvas;

    void Start() {

        MinimapCanvas = GameObject.Find("MinimapCanvas").GetComponent<Canvas>();
        PauseCanvas = GameObject.Find("PauseCanvas").GetComponent<Canvas>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update() {
        if(PauseCanvas.transform.GetChild(0).GetChild(0).gameObject.activeSelf) {
            MinimapCanvas.transform.GetChild(0).gameObject.SetActive(true);
            MinimapCanvas.sortingOrder = 1;
        }
        else {
            MinimapCanvas.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void LateUpdate() {

        Vector3 newPosition = player.transform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);        
    }
}
