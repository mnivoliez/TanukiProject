using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateformeMobile : MonoBehaviour {

    void OnCollisionEnter(Collision col) {

        col.transform.parent = transform.parent;

    }

    void OnCollisionExit(Collision col) {
        col.transform.parent = null;
    }
}
