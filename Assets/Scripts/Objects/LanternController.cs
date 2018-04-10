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
    [SerializeField] private AudioClip lanterFallWater;
    [SerializeField] private AudioClip lanterDome;
    private bool lanterClosestFoundPlay = false;

    private GameObject the_player;
    private Vector3 pos_player;
    private Rigidbody the_lantern;
    private Vector3 pos_lantern;
    private Vector3 vec_distance;
    private float numb_distance;
    private bool dome_playing = false;

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

        _range = _min_radius;
        _light.intensity = _min_intensity;
        
        _bbox = GetComponent<SphereCollider>();
        _bbox.radius = _range;
    }

    private void Start() {
        positionOrigin = transform.position;
        rotationOrigin = transform.rotation;

        the_player = GameObject.FindGameObjectWithTag("Player");
        the_lantern = GetComponent<Rigidbody>();
    }

    private void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        if (transform.parent == null) {
            _range = _min_radius;
            _light.intensity = _min_intensity;

            is_being_played = false;
            token_play_once = false;

        }
        else {
            if (transform.parent.tag == "Hand") {
                _range = _max_radius;
                _light.intensity = _max_intensity;
                is_being_played = true;
                if (is_being_played && !token_play_once) {
                    token_play_once = true;
                    //AudioSource.PlayClipAtPoint(lanterSound, transform.position, 1.0f);
                    SoundController.instance.PlayLanternSingle(lanterSound);
                }
            }
        }
        _light.range = _range;
        _bbox.radius = _range - 0.5f;
    }

    private void FixedUpdate() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================

        //the_player = GameObject.FindGameObjectWithTag("Player");
        //the_lantern = GetComponent<Rigidbody>();

        if (the_lantern != null) {
            pos_player = the_player.transform.position;
            pos_lantern = the_lantern.transform.position;
            vec_distance = pos_player - pos_lantern;
            numb_distance = Mathf.Sqrt(Mathf.Pow(vec_distance.x, 2) + Mathf.Pow(vec_distance.y, 2) + Mathf.Pow(vec_distance.z, 2));
            //Debug.Log(numb_distance);
            if (numb_distance < 60f) {
                if (!dome_playing) {
                    SoundController.instance.PlayLanternSource(lanterDome);
                    dome_playing = true;
                    lanterClosestFoundPlay = true;
                }
                if (dome_playing) {
                    SoundController.instance.AdjustLanternSource(1 - (numb_distance / 60f));
                }
            }
            if (numb_distance >= 60f && lanterClosestFoundPlay) {
                SoundController.instance.StopLanternSource();
                dome_playing = false;
                lanterClosestFoundPlay = false;
            }
        }


        if (GetComponent<Rigidbody>() && GetComponent<Rigidbody>().IsSleeping())
            GetComponent<Rigidbody>().WakeUp();
        if (shallRespawn) {
            if ((Time.time - timeoutRespawn) > elaspTimeBeforeRespawn) {
                Respawn();
            }
        }
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water")) {

            SoundController.instance.PlayLanternSingle(lanterFallWater);

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

    public void Respawn() {
        lanternBody = GetComponent<Rigidbody>();
        lanternBody.velocity = Vector3.zero;
        transform.position = positionOrigin;
        transform.rotation = rotationOrigin;
        shallRespawn = false;
        gameObject.layer = LayerMask.NameToLayer("Catchable");
        GetComponent<BoxCollider>().enabled = true;
    }
}
