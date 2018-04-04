﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointLantern : MonoBehaviour {

    [SerializeField] private GameObject checkPoint;
    [SerializeField] private GameObject saveEffect;

    void Start () {
		
	}
	

	void Update () {
		
	}

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Cible: " + GetComponent<Collider>().name);
            Destroy(Instantiate(saveEffect, transform.GetChild(0).gameObject.transform.position, Quaternion.identity), 3f);
            transform.GetChild(0).gameObject.SetActive(true);
            other.gameObject.GetComponent<PlayerHealth>().SetRespawnPoint(checkPoint);
        }
    }
}
