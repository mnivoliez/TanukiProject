using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawProjector : MonoBehaviour {
	
	private Material mat;
	private float progress = 1f;

	[SerializeField]
	private float speedProgress = 0.01f;

	void Start() {
		mat = GetComponent<Projector>().material = new Material(GetComponent<Projector>().material);
	}

	void Update () {
		mat.SetFloat("_PawProgress", progress);
		progress -= speedProgress;

		if(progress < 0)
			Destroy(transform.parent.gameObject);
	}
}
