using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteleBehavior : MonoBehaviour {

    [SerializeField] private GameObject nextStele;

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player")) {
            HitodamaController hitodama = GameObject.FindGameObjectWithTag("Hitodama").GetComponent<HitodamaController>();
            hitodama.SetIsGuiding(true);
            hitodama.SetTargetStele(nextStele);
        }

    }
}
