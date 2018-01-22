using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateformeMobile : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    void OnCollisionEnter(Collision col) {

        col.transform.parent = transform.parent;

    }

    void OnCollisionExit(Collision col) {
        col.transform.parent = null;
    }
    // Update is called once per frame
    void Update() {

    }
}
