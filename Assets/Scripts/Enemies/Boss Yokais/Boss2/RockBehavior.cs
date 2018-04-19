using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehavior : MonoBehaviour {

    void Start() {
        GameObject[] yokais = GameObject.FindGameObjectsWithTag("Yokai");
        Collider myCollider = GetComponent<Collider>();
        foreach (GameObject yokai in yokais) {
            Physics.IgnoreCollision(myCollider, yokai.GetComponent<Collider>());
        }
    }

    void OnCollisionEnter(Collision collision) {
        Destroy(gameObject);
    }
}
