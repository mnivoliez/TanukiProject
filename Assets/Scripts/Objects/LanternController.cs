using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternController : MonoBehaviour
{
    private Renderer _renderer;
    [SerializeField] Light _light;
    // Use this for initialization
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.sharedMaterial.SetVector("_Center", transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        _renderer.sharedMaterial.SetVector("_Center", transform.position);

        if (transform.parent == null)
        {
            _renderer.sharedMaterial.SetFloat("_Distance", 5f);
            _light.intensity = 1;
        }
        else
        {
            _renderer.sharedMaterial.SetFloat("_Distance", 10f);
            _light.intensity = 1.6f;
        }
    }

    public bool isInEffectArea(Vector3 point)
    {
        float dist = Vector3.Distance(transform.position, point);
        if (transform.parent == null)
        {
            return dist < 5f;
        }
        else
        {
            return dist < 10f;
        }
    }
}
