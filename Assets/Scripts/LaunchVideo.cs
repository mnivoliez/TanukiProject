using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchVideo : MonoBehaviour {

    [SerializeField] private float timeToSkipInSecond = 0f;
    [SerializeField] private GameObject cameraMain;
    [SerializeField] private GameObject cameraVideo;

    void Start () {
        cameraVideo.GetComponent<Camera>().enabled = true;
        cameraMain.GetComponent<Camera>().enabled = false;
        Invoke("DestroyVideo", timeToSkipInSecond);
        Invoke("PauseTheGame", 0.5f);
    }
	

	void Update () {
		if(Input.GetButtonDown("Cancel")) {
            CancelInvoke();
            DestroyVideo();
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
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().SetFreezeInput(false);
        Destroy(cameraVideo);
        Destroy(gameObject);
    }
}
