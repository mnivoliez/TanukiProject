using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathCreator))]
public class SteleBehavior : MonoBehaviour {

    [SerializeField] private GameObject nextStele;
    [SerializeField] private bool lastStele;
    private PathCreator creator;
	private PathPlatform path;

    void Start() {
        if(nextStele) {
            creator = GetComponent<PathCreator>();
		    path = creator.path;
        }
	}

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player")) {
            HitodamaController hitodama = GameObject.FindGameObjectWithTag("Hitodama").GetComponent<HitodamaController>();
            if (lastStele) {
                hitodama.SetIsGuiding(false);
            }
            else {
                hitodama.SetIsGuiding(true);
                hitodama.SetTargetStele(nextStele);
                hitodama.SetPath(this.path, creator.startPos);
            }
        }

    }
}
