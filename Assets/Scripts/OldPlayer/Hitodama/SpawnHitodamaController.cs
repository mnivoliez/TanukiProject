using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHitodamaController : MonoBehaviour {

    public GameObject player;

    void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        transform.RotateAround(player.transform.position, Vector3.up, 20 * Time.deltaTime);
    }
}
