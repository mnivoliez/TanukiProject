using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch1 : MonoBehaviour
{


    public GameObject porte;
    public GameObject other;


    // Use this for initialization
    void Start()
    {
    }


    void OnCollisionEnter(Collision collider)
    {

        porte.SetActive(false);
        other.SetActive(false);

    }


    // Update is called once per frame
    void Update()
    {

    }
}
