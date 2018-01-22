using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch1 : MonoBehaviour {


    public GameObject porte;


    // Use this for initialization
    void Start() {
    }


    void OnCollisionEnter(Collision collider) {

        porte.SetActive(false);

    }


    // Update is called once per frame
    void Update() {

    }
}
