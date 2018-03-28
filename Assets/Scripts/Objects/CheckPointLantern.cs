using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointLantern : MonoBehaviour {

    [SerializeField] private GameObject checkPoint;

	void Start () {
		
	}
	

	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            transform.GetChild(0).gameObject.SetActive(true);
            other.gameObject.GetComponent<PlayerHealth>().SetRespawnPoint(checkPoint);
        }
    }
}
