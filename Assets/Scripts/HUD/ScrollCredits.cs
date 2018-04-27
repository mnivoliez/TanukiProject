using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCredits : MonoBehaviour {

	[SerializeField] private float scrollSpeed = 1f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.position += Vector3.up * Time.fixedDeltaTime * scrollSpeed;
	}
}
