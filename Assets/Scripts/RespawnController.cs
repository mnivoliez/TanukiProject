using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RespawnController : MonoBehaviour {
    [SerializeField] private GameObject respawnPoint;

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
            float distance = distanceLantern + 1.0f;
            foreach(GameObject lantern in lanterns) {
                if (lantern != null) {
                    distance = Vector3.Distance(lantern.transform.position, player.transform.position);
                }

                if (distance > distanceLantern) {
                    player.GetComponent<PlayerHealth>().PlayerDie();
                } else {
                    Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
                    if (velocity.x < 0.01f && velocity.y < 0.01f && velocity.z < 0.01f) {
                        if (playerStop == false) {
                            playerStop = true;
                            timeStop = Time.time;
                        }
                    } else {
                        playerStop = false;
                    }
                    if (playerStop && ((Time.time - timeStop) > timeDelay)) {
                        player.GetComponent<PlayerHealth>().PlayerDie();
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
