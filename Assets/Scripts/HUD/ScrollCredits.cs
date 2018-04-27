using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCredits : MonoBehaviour {

	[SerializeField] private float scrollSpeed = 1f;

	// Use this for initialization
	void Start () {
        StartCoroutine(WillQuitOnceEndCredit());
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position += Vector3.up * Time.fixedDeltaTime * scrollSpeed;
	}

    public void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
    }

    IEnumerator WillQuitOnceEndCredit() {
        yield return new WaitForSeconds(36);
        Quit();
    }
}
