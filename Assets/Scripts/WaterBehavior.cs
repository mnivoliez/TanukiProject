using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaterBehavior : MonoBehaviour {
    [SerializeField] private float distanceLantern = 10.0f;
    private GameObject[] lanterns;

    [SerializeField] private float timeDelay = 1.0f;

    private bool isPlayer = false;
    private GameObject player;
    private bool playerStop = false;
    private float timeStop = 0.0f;

    void Start() {
        lanterns = GameObject.FindGameObjectsWithTag("Lantern");
    }

    private void FixedUpdate() {

        if (isPlayer) {
            GameObject lanternNearest = new GameObject();
            float distance = 0f;
            if (lanterns.Length > 0) {
                lanternNearest = lanterns[0];
                distance = Vector3.Distance(lanternNearest.transform.position, player.transform.position);
            } else {
                lanternNearest = null;
            }
            foreach (GameObject lantern in lanterns) {
                float dis = Vector3.Distance(lantern.transform.position, player.transform.position);
                if (dis < distance) {
                    lanternNearest = lantern;
                    distance = dis;
                }
            }

            if (lanternNearest != null) {
                if (distance > distanceLantern) {
                    Debug.Log("die distance");
                    player.GetComponent<PlayerHealth>().PlayerDie();
                    isPlayer = false;
                } else {
                    Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
                    Debug.Log("velocity" + velocity);
                    if (velocity.x < 0.01f && velocity.y < 0.01f && velocity.z < 0.01f) {
                        //Debug.Log("die stop");
                        if (playerStop == false) {
                            playerStop = true;
                            timeStop = Time.time;
                        }
                    } else {
                        playerStop = false;
                    }
                    if (playerStop && ((Time.time - timeStop) > timeDelay)) { 
                        player.GetComponent<PlayerHealth>().PlayerDie();
                        isPlayer = false;
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Player")) {
            isPlayer = true;
            player = col.gameObject;
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            isPlayer = false;
            player = null;
            playerStop = false;
        }
    }

}
