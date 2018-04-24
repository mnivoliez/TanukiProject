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
                Pause.Paused = false;
                //================================================
                SoundController.instance.SelectHUD("TutoPictureExit");
                //================================================
                if (currentTuto < nbTuto - 1) {
                    currentTuto++;
                    transform.GetChild(currentTuto).gameObject.SetActive(true);
                }
                else {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().SetFreezeInput(false);
                    if (!isStele) {
                        Destroy(gameObject);
                    }
                }

            }
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            //================================================
            SoundController.instance.SelectHUD("PauseOpenClose");
            //================================================
            Pause.Paused = true;
            isActive = true;
            currentTuto = 0;
            transform.GetChild(0).gameObject.SetActive(true);
            other.gameObject.GetComponent<InputController>().SetFreezeInput(true);

        }
    }

    public void ActiveParchment() {
        //================================================
        SoundController.instance.SelectHUD("PauseOpenClose");
        //================================================
        isActive = true;
        transform.GetChild(0).gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().SetFreezeInput(true);
    }


}
