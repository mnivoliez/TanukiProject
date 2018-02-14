using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delay : MonoBehaviour {
    public GameObject prefabBumperDroite;
    public GameObject prefabBumperGauche;

    // Use this for initialization
    void Start () {
        InvokeRepeating("SpawnBumperDroite", 0f, 15f);
        InvokeRepeating("SpawnBumperGauche", 0f, 15f);
    }

	void SpawnBumperDroite() {
        Destroy(Instantiate(prefabBumperDroite, transform.position, Quaternion.identity, transform), 32);
        
    }

    void SpawnBumperGauche()
    {
        Destroy(Instantiate(prefabBumperGauche, transform.position, Quaternion.identity, transform), 30);
    }
    // Update is called once per frame
    void Update () {
		
	}
}
