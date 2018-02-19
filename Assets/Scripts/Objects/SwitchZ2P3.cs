using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchZ2P3 : MonoBehaviour {


    public GameObject platriv;

	// Use this for initialization
	void Start () {
		
	}

    void OnCollisionEnter(Collision collider) {

        platriv.SetActive(true);
       

    }

    // Update is called once per frame
    void Update () {
		
	}
}
