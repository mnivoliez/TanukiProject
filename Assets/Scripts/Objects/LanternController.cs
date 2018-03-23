using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LanternController : MonoBehaviour
{
    Light _light;

    private float _range;
    [SerializeField] private float _min_radius;
    [SerializeField] private float _max_radius;
    [SerializeField] private float _min_intensity;
    [SerializeField] private float _max_intensity;

    [SerializeField]
    private GameObject airLantern;
    [SerializeField]
    private float timeoutRespawn = 2.0f;
    private float elaspTimeBeforeRespawn;
    private bool shallRespawn;
    private Transform origin;

    private SphereCollider _bbox;

    private void Awake()
    {
        _light = GetComponent<Light>();
        if (transform.parent == null)
        {
            _range = _min_radius;
            _light.intensity = _min_intensity;
        }
        else
        {
            _range = _max_radius;
            _light.intensity = _max_intensity;
        }
        _bbox = GetComponent<SphereCollider>();
        _bbox.radius = _range;
    }

    private void Start()
    {
        origin = transform;
        elaspTimeBeforeRespawn = 0f;
        shallRespawn = false;
    }

    private void Update()
    {
        if (transform.parent == null)
        {
            _range = _min_radius;
            _light.intensity = _min_intensity;
        }
        else
        {
            _range = _max_radius;
            _light.intensity = _max_intensity;
        }
        _light.range = _range;
        _bbox.radius = _range - 0.5f;
    }

    void FixedUpdate()
    {
        if (shallRespawn)
        {
            elaspTimeBeforeRespawn += Time.fixedDeltaTime;
            if (elaspTimeBeforeRespawn >= timeoutRespawn)
            {
                transform.position = origin.position;
                transform.rotation = origin.rotation;
                shallRespawn = false;
                elaspTimeBeforeRespawn = 0f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //layer 4 = water
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            GameObject air = Instantiate(airLantern, transform.position, Quaternion.identity);
            shallRespawn = true;
        }
    }

    public bool isInEffectArea(Vector3 point)
    {
        float dist = Vector3.Distance(transform.position, point);
        return dist < _range;
    }

    public float GetRadiusEffect()
    {
        return _range;
    }
}
