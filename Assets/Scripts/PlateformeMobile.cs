using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateformeMobile : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

	void OnCollisionEnter (Collision col)
	{
		if (col.gameObject.CompareTag("Player")) {
			col.transform.parent = transform;
		}
	}

	void OnCollisionExit (Collision col)
	{
		if (col.gameObject.CompareTag("Player")) {
			col.transform.parent = null;
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
