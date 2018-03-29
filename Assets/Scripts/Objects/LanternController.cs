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

    [SerializeField] private AudioClip lanterSound;
    private bool is_being_played = false;
    private bool token_play_once = false;

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

            is_being_played = false;
            token_play_once = false;
        }
        else {
            _range = _max_radius;
            _light.intensity = _max_intensity;

            is_being_played = true;
            if (is_being_played && !token_play_once)
            {
                token_play_once = true;
                AudioSource.PlayClipAtPoint(lanterSound, transform.position, 1.0f);
                Debug.Log("Playing Sound");
            }
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

            is_being_played = false;
            token_play_once = false;
        }
        else {
            _range = _max_radius;
            _light.intensity = _max_intensity;

            is_being_played = true;
            if (is_being_played && !token_play_once) {
                token_play_once = true;
                AudioSource.PlayClipAtPoint(lanterSound, transform.position, 1.0f);
                Debug.Log("Playing Sound");
            }
        }
        _light.range = _range;
        _bbox.radius = _range - 0.5f;
    }

    private void FixedUpdate() {
        if (GetComponent<Rigidbody>() && GetComponent<Rigidbody>().IsSleeping())
            GetComponent<Rigidbody>().WakeUp();
        if (shallRespawn) {
            if ((Time.time - timeoutRespawn) > elaspTimeBeforeRespawn) {
                lanternBody = GetComponent<Rigidbody>();
                lanternBody.velocity = Vector3.zero;
                transform.position = positionOrigin;
                transform.rotation = rotationOrigin;
                shallRespawn = false;
                gameObject.layer = LayerMask.NameToLayer("Catchable");
                GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water")) {
            
            if (shallRespawn == false) {
                GameObject air = Instantiate(airLantern, transform.position, Quaternion.identity);
                gameObject.layer = 0;
                shallRespawn = true;
                elaspTimeBeforeRespawn = Time.time;
                GetComponent<BoxCollider>().enabled = false;
            }
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
