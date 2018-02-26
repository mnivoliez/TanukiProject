using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelControl : MonoBehaviour {

	public string levelName;
	public GameObject TransitionImage;

	private Image Black;
	private Animator anim;

	void Start() {
		GameObject transitionImageInstance = Instantiate (TransitionImage);

		Black = transitionImageInstance.GetComponent<Image> ();
		anim = transitionImageInstance.GetComponent<Animator> ();
	}

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            StartCoroutine(Fading());
        }
    }

    IEnumerator Fading() {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => Black.color.a == 1);
		SceneManager.LoadScene(levelName);
    }
}
