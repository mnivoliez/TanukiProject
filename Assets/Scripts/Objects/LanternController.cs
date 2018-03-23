using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LanternController : MonoBehaviour {
    Light _light;

    private float _range;
    [SerializeField] private float _min_radius;
    [SerializeField] private float _max_radius;
    [SerializeField] private float _min_intensity;
    [SerializeField] private float _max_intensity;

    [SerializeField] private GameObject airLantern;
    [SerializeField] private float timeoutRespawn = 2.0f;
    private float elaspTimeBeforeRespawn;
    private bool shallRespawn;

    private Vector3 positionOrigin;
    private Quaternion rotationOrigin;

    private Rigidbody lanternBody;

    private SphereCollider _bbox;

    private void Awake() {
        _light = GetComponent<Light>();
        if (transform.parent == null) {
            _range = _min_radius;
            _light.intensity = _min_intensity;
        }
        else {
            _range = _max_radius;
            _light.intensity = _max_intensity;
        }
        _bbox = GetComponent<SphereCollider>();
        _bbox.radius = _range;
    }

    private void Start() {
        positionOrigin = transform.position;
        rotationOrigin = transform.rotation;
    }

    private void Update() {
        if (transform.parent == null) {
            _range = _min_radius;
            _light.intensity = _min_intensity;
        }
        else {
            _range = _max_radius;
            _light.intensity = _max_intensity;
        }
        _light.range = _range;
        _bbox.radius = _range - 0.5f;
    }

    private void FixedUpdate() {
        if (shallRespawn) {
            if ((Time.time - timeoutRespawn) > elaspTimeBeforeRespawn) {
                lanternBody = GetComponent<Rigidbody>();
                lanternBody.velocity = Vector3.zero;
                transform.position = positionOrigin;
                transform.rotation = rotationOrigin;
                shallRespawn = false;
                gameObject.layer = LayerMask.NameToLayer("Catchable");
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water")) {
            GameObject air = Instantiate(airLantern, transform.position, Quaternion.identity);
            gameObject.layer = 0;
            shallRespawn = true;
            elaspTimeBeforeRespawn = Time.time;
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
