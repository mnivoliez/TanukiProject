using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealObject : MonoBehaviour {

    [SerializeField] private float healPoint = 1f;

	void Start () {
		
	}
	

	void Update () {
        transform.Rotate(new Vector3(0, 2, 0));
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {

            other.GetComponent<PlayerHealth>().GainHP(healPoint);
            Destroy(gameObject);
        }
    }
}
