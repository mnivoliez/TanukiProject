using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RespawnController : MonoBehaviour {
    public GameObject respawnPoint;

    private Image Black;
    private Animator anim;

    void Start() {
        GameObject transitionImageInstance = GameObject.Find("DeathTransitionImage");

        Black = transitionImageInstance.GetComponent<Image>();
        anim = transitionImageInstance.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.CompareTag("Player")) {
            col.gameObject.GetComponent<Animator>().SetBool("isDead", true);
            StartCoroutine(Fading(col.gameObject.GetComponent<Animator>()));
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
