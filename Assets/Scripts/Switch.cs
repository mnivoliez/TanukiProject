using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour {

    public GameObject plateforme;
    public GameObject plateforme1;
    public GameObject plateforme2;
    public GameObject porte;


    // Use this for initialization
    void Start() {
    }


    void OnCollisionEnter(Collision collider) {
        plateforme.SetActive(true);
        plateforme1.SetActive(true);
        plateforme2.SetActive(true);
        porte.SetActive(false);

    }


    // Update is called once per frame
    void Update() {

    }
}
