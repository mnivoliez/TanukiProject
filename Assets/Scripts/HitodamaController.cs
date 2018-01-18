using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitodamaController : MonoBehaviour {

    public GameObject spawnHitodama;
    private float speed = 8;

    void Start() {

    }

    void FixedUpdate() {

        transform.position = Vector3.Lerp(transform.position, spawnHitodama.transform.position, speed * Time.deltaTime);

    }

}
