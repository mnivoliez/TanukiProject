using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLanternController : MonoBehaviour {
    [SerializeField]
    private GameObject lantern;
    [SerializeField]
    private float timeDestroy = 5.0f;

    private Vector3 pointRespawnLantern;
    private float timeInstant;

    private void Start() {
        timeInstant = Time.time;
    }

    private void Update() {
        transform.TransformDirection(Vector3.up);
        if(timeInstant + timeDestroy < Time.time) {
            Instantiate(lantern, pointRespawnLantern, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void setPointRespawnLantern(Vector3 vec) {
        pointRespawnLantern = vec;
    }
}
