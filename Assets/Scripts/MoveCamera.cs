using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public GameObject target; // The target in which to follow

    // Use this for initialization
    void Start () {
        target = GameObject.FindGameObjectWithTag("Player");

    }
	
	// Update is called once per frame
	void FixedUpdate () {

        //gameObject.transform.RotateAround(target.transform.position, new Vector3(0, 1, 0), Input.GetAxis("MoveCamera"));
        target.transform.RotateAround(target.transform.position, new Vector3(0, 1, 0), Input.GetAxis("MoveCamera")*2);
       

    }
}
