using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {

    [SerializeField] float speed = 30;

    void Start() {
        Destroy(gameObject, 10);
    }


    void Update() {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
