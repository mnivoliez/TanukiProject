using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteleBehavior : MonoBehaviour {

    [SerializeField] private GameObject nextStele;
    [SerializeField] private bool lastStele;

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player")) {
            HitodamaController hitodama = GameObject.FindGameObjectWithTag("Hitodama").GetComponent<HitodamaController>();
            if (lastStele) {
                hitodama.SetIsGuiding(false);
            }
            else {
                hitodama.SetIsGuiding(true);
                hitodama.SetTargetStele(nextStele);
            }
        }

    }
}
