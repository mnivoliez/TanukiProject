using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RespawnController : MonoBehaviour {
    [SerializeField]
    private GameObject respawnPoint;

    [SerializeField]
    private float distanceLantern = 10.0f;
    private GameObject lantern;

    [SerializeField]
    private float timeDelay = 1.0f;

    private bool isPlayer = false;
    private GameObject player;
    private bool playerStop = false;
    private float timeStop = 0.0f;

    private Image Black;
    private Animator anim;

    void Start() {
        GameObject transitionImageInstance = GameObject.Find("DeathTransitionImage");

        Black = transitionImageInstance.GetComponent<Image>();
        anim = transitionImageInstance.GetComponent<Animator>();
    }

    private void FixedUpdate() {
        lantern = GameObject.FindGameObjectWithTag("Lantern");
        if (isPlayer) {
            float distance = distanceLantern + 1.0f;
            if (lantern != null) {
                distance = Vector3.Distance(lantern.transform.position, player.transform.position);
            }

            if (distance > distanceLantern) {
                player.GetComponent<Animator>().SetBool("isDead", true);
                StartCoroutine(Fading(player.GetComponent<Animator>()));
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
                    player.GetComponent<Animator>().SetBool("isDead", true);
                    StartCoroutine(Fading(player.GetComponent<Animator>()));
                }
            }
        }
    }

    void OnCollisionStay(Collision col) {
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

    IEnumerator Fading(Animator anim_player) {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => Black.color.a == 1);
        anim.SetBool("Fade", false);
        anim_player.SetBool("isDead", false);
        anim_player.transform.SetPositionAndRotation(respawnPoint.transform.position, Quaternion.identity);
    }
}
