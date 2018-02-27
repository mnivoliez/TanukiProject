using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictorySwitch : MonoBehaviour
{
	private Image Black;
	private Animator anim;
	private Pause pauseScript;
	private GameObject transitionImageInstance;
	private GameObject pauseCanvasInstance;

	public static bool Victory = false;

	// Use this for initialization
	void Start ()
	{
		pauseCanvasInstance = GameObject.Find("PauseCanvas");
		pauseScript = pauseCanvasInstance.GetComponent<Pause> ();

		transitionImageInstance = GameObject.Find("VictoryTransitionImage");

		Black = transitionImageInstance.GetComponent<Image> ();
		anim = transitionImageInstance.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("Player"))
		{
			Victory = true;
			StartCoroutine(Fading());
		}
	}

	IEnumerator Fading() {
		anim.SetBool("Fade", true);
		yield return new WaitUntil(() => Black.color.a == 1);
		transitionImageInstance.transform.GetChild (0).gameObject.SetActive (true);
		pauseScript.PauseGame (false);
	}
}
