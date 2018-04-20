using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBarrierBossController : MonoBehaviour {

    [SerializeField] private GameObject boss;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (boss == null) {
            Destroy(gameObject);
        }
	}
}
