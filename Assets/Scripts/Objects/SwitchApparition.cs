using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchApparition : MonoBehaviour
{


    public GameObject porte;


    void OnCollisionEnter(Collision collider)
    {

			porte.SetActive (true);
		
    }
}
