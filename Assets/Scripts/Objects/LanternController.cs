using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternController : MonoBehaviour {
    Light _light;

    private float _range;
    [SerializeField] private float _min_radius;
    [SerializeField] private float _max_radius;
    [SerializeField] private float _min_intensity;
    [SerializeField] private float _max_intensity;

    [SerializeField]
    private GameObject airLantern;
    [SerializeField]
    private float timeDestroy = 2.0f;

    private Vector3 pointRespawnLantern;

    private void Awake() {
        _light = GetComponent<Light>();
        if (transform.parent == null) {
            _range = _min_radius;
            _light.intensity = _min_intensity;
        } else {
            _range = _max_radius;
            _light.intensity = _max_intensity;
        }
    }

    private void Start() {
        pointRespawnLantern = transform.position;
    }

    private void Update() {
        if (transform.parent == null) {
            _range = _min_radius;
            _light.intensity = _min_intensity;
        } else {
            _range = _max_radius;
            _light.intensity = _max_intensity;
        }
        _light.range = _range;
    }

    private void OnCollisionEnter(Collision collision) {
        //layer 4 = water
        if (collision.gameObject.layer == 4) {
            GameObject air = Instantiate(airLantern, transform.position, Quaternion.identity);
            air.GetComponent<AirLanternController>().setPointRespawnLantern(pointRespawnLantern);
            Destroy(gameObject, timeDestroy);
        }
    }

    public bool isInEffectArea(Vector3 point) {
        float dist = Vector3.Distance(transform.position, point);
        return dist < _range;
    }

    public float GetRadiusEffect() {
        return _range;
    }
}
