using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    public Transform player;
    private Canvas CanvasMinimap;
    //[SerializeField] private GameObject PlayerPrefab;

    void Start() {
        CanvasMinimap = GetComponent<Canvas>();
    }

    private void Awake() {
        //Instantiate(PlayerPrefab).name = "Player";
    }

    private void LateUpdate() {
        //Vector3 newPosition = PlayerPrefab.position;
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        //transform.rotation = Quaternion.Euler(90f, PlayerPrefab.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
