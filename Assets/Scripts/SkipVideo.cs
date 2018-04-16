using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//================================================
//SOUNDCONTROLER
//================================================

public class SkipVideo : MonoBehaviour {

	public string nameScene;

	private Image Black;
	private Animator anim;

	void Start() {
        //================================================
        SoundController.instance.StopTheme();
        //================================================
        GameObject transitionImageInstance = GameObject.Find("TransitionImage");

		Black = transitionImageInstance.GetComponent<Image> ();
		anim = transitionImageInstance.GetComponent<Animator> ();

		Invoke ("SkipVideoToNextScene", 268f);
		//Invoke ("SkipVideoToNextScene", 5f);
	}

    private void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================

        if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel")) {
            CancelInvoke();
            SkipVideoToNextScene();
        }
    }

    private void SkipVideoToNextScene(){
		StartCoroutine(Fading());
	}

	IEnumerator Fading() {
		anim.SetBool("Fade", true);
		yield return new WaitUntil(() => Black.color.a == 1);
		SceneManager.LoadScene(nameScene);
	}
}
