using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleportation : MonoBehaviour {

    public string nameScene;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter (Collider col) {
        Debug.Log("Plopl");
        if (col.CompareTag("Player")){
            Debug.Log("Plop 2");
            SceneManager.LoadScene(nameScene);
        } 
    }
}
