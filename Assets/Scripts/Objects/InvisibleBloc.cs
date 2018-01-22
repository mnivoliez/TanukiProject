using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBloc : MonoBehaviour {

    private Collider boxCollider;
    private MeshRenderer meshRenderer;

    // Use this for initialization
    void Start() {
        boxCollider = transform.parent.GetComponent<Collider>();
        meshRenderer = transform.parent.GetComponent<MeshRenderer>();
        boxCollider.enabled = false;
        meshRenderer.enabled = false;
    }

    void OnTriggerStay(Collider collider) {
        if (collider.gameObject.tag == "Lantern") {
            boxCollider.enabled = true;
            meshRenderer.enabled = true;
        }
    }

    void OnTriggerExit(Collider collider) {
        if (collider.gameObject.tag == "Lantern") {
            boxCollider.enabled = false;
            meshRenderer.enabled = false;
        }
    }
}
