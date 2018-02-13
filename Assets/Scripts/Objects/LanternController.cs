using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternController : MonoBehaviour
{
    [SerializeField] Light _light;

    // Update is called once per frame
    void Update()
    {

        if (transform.parent == null)
        {
            _light.intensity = 1;
        }
        else
        {
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
