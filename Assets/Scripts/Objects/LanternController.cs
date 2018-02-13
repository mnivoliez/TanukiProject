using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternController : MonoBehaviour
{
    [SerializeField] Light _light;
    [SerializeField] float _effectAreaRadius;

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null)
        {
            _effectAreaRadius = 5f;
            _light.intensity = 1;
        }
        else
        {
            _effectAreaRadius = 10f;
            _light.intensity = 1.6f;
        }
    }

    public bool isInEffectArea(Vector3 point)
    {
        float dist = Vector3.Distance(transform.position, point);
        return dist < _effectAreaRadius;
    }
}
