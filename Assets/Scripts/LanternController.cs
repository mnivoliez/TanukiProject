using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.parent == null)
        {
            GetComponent<Light>().intensity = 1;
        }
        else
        {
            GetComponent<Light>().intensity = 1.6f;
        }
	}
}
