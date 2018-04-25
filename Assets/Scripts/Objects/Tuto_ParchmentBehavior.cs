using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//================================================
//SOUNDCONTROLER
//================================================

public class Tuto_ParchmentBehavior : MonoBehaviour {

    [SerializeField] bool isStele;
    bool isActive;
    int nbTuto = 0;
    int currentTuto = 0;

    void Start() {
        nbTuto = transform.childCount;
        currentTuto = 0;
    }

    void Update() {

        if (isActive) {
            if (Input.GetButtonDown("Jump")) {
                transform.GetChild(currentTuto).gameObject.SetActive(false);

                //================================================
                SoundController.instance.SelectHUD("TutoPictureExit");
                //================================================
                if (currentTuto < nbTuto - 1) {
                    currentTuto++;
                    transform.GetChild(currentTuto).gameObject.SetActive(true);
                }
                else {
                    
                    GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().SetFreezeInput(false);
                    Pause.Paused = false;
                    isActive = false;
                    Time.timeScale = 1;
                    nbTuto = 0;
                    if (!isStele) {
                        Destroy(gameObject);
                    }
                }

            }
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            if (nbTuto != 0) {
                other.gameObject.GetComponent<InputController>().SetFreezeInput(true);
                isActive = true;
                transform.GetChild(0).gameObject.SetActive(true);
                //================================================
                SoundController.instance.SelectHUD("PauseOpenClose");
                //================================================
                Pause.Paused = true;
                Time.timeScale = 0;
            }

        }
    }

    public void ActiveParchment() {
        //================================================
        SoundController.instance.SelectHUD("PauseOpenClose");
        //================================================
        isActive = true;
        transform.GetChild(0).gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().SetFreezeInput(true);
        Time.timeScale = 0;
    }


}
