using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FireBarrierController : MonoBehaviour {

    [SerializeField] private int nbYokaiNeeded = 1;
    [SerializeField] private Text nbYokaiNeededText;
    bool disappear = false;
    Renderer renderBarrier;
    float counter = 0.8f;
    GameObject target;
    bool isAlreadyPlayed = false;

    void Start() {
        nbYokaiNeededText.text = "" + nbYokaiNeeded;
        renderBarrier = gameObject.transform.parent.GetComponent<Renderer>();
        target = GameObject.FindGameObjectWithTag("Player");
    }


    void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        if (disappear) {
            counter += Time.deltaTime;
            renderBarrier.material.SetFloat("_Height", counter);

            if (counter > 2) {
                if (isAlreadyPlayed) {
                    isAlreadyPlayed = false;
                    //================================================
                    SoundController.instance.StopFireWallEffect();
                    //================================================
                }
                Destroy(gameObject.transform.parent.gameObject);
            }
        }

    }

    private void FixedUpdate() {
        if (Mathf.Abs((target.transform.position - transform.position).magnitude) < 30f) {
            if (!isAlreadyPlayed) {
                isAlreadyPlayed = true;
                //================================================
                SoundController.instance.SelectFIREWALL("FireWall");
                //================================================
            }
        }
        else {
            if (isAlreadyPlayed) {
                isAlreadyPlayed = false;
                //================================================
                SoundController.instance.StopFireWallEffect();
                //================================================
            }
        }

    }

    void OnTriggerEnter(Collider collid) {
        if (collid.gameObject.CompareTag("Player")) {
            Game.Update_Game();
            if(SceneManager.GetActiveScene().name == "Z1-P1-complete" || SceneManager.GetActiveScene().name == "Z1-P2-complete" || SceneManager.GetActiveScene().name == "Z1-P3-complete") { 
                if (Game.playerData.caught_yokaiZ1 >= nbYokaiNeeded) {
                    //================================================
                    SoundController.instance.SelectENVQuick("FireWallExtinguished");
                    //================================================
                    disappear = true;
                    nbYokaiNeededText.text = " ";
                }
            }
            if (SceneManager.GetActiveScene().name == "Z2-P1-complete" || SceneManager.GetActiveScene().name == "Z2-P2-complete" || SceneManager.GetActiveScene().name == "Z2-P3-complete") {
                if (Game.playerData.caught_yokaiZ2 >= nbYokaiNeeded) {
                    //================================================
                    SoundController.instance.SelectENVQuick("FireWallExtinguished");
                    //================================================
                    disappear = true;
                    nbYokaiNeededText.text = " ";
                }
            }
        }
    }

}
