using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchVideo : MonoBehaviour {

    [SerializeField] private float timeToSkipInSecond = 0f;

	void Start () {

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
        Pause.Paused = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().SetFreezeInput(false);
        Destroy(gameObject);
    }
}
