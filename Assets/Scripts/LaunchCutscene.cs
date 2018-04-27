using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchCutscene : MonoBehaviour {

    [SerializeField] private float timeToSkipInSecond = 0f;
    [SerializeField] private GameObject cameraMain;
    [SerializeField] private GameObject cameraVideo;
    private bool isInvideo = false;
    private bool alreadyPlayed = false;
    [SerializeField] private bool destroyAtEnd;




    void Update() {
        if (isInvideo) {
            if (Input.GetButtonDown("Cancel")) {
                CancelInvoke();
                DestroyVideo();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!alreadyPlayed) {
            if (other.CompareTag("Player")) {
                cameraVideo.SetActive(true);
                isInvideo = true;
                alreadyPlayed = true;
                cameraVideo.GetComponent<Camera>().enabled = true;
                cameraMain.GetComponent<Camera>().enabled = false;
                Invoke("DestroyVideo", timeToSkipInSecond);
                Invoke("PauseTheGame", 0.5f);
            }
        }
    }

    private void PauseTheGame() {
        Pause.Paused = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().SetFreezeInput(true);
    }

    private void DestroyVideo() {
        cameraVideo.GetComponent<Camera>().enabled = false;
        cameraMain.GetComponent<Camera>().enabled = true;
        Pause.Paused = false;
        isInvideo = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().SetFreezeInput(false);
        Destroy(cameraVideo);
        if (destroyAtEnd) {
            Destroy(gameObject);
        }
    }
}
