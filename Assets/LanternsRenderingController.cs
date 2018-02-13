using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternsRenderingController : MonoBehaviour
{

    private GameObject[] _lanterns;
    private List<Vector4> _positions;
    private List<float> _distances;
    private bool _isDirty;

    void Awake()
    {
        _lanterns = GameObject.FindGameObjectsWithTag("Lantern");
        _positions = new List<Vector4>(5);
        _distances = new List<float>(5);
        foreach (var lantern in _lanterns)
        {
            _positions.Add(lantern.transform.position);
            _distances.Add(5f);
        }
        UpdateShaderWithLanternsPosition();
        _isDirty = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShaderWithLanternsPosition();
    }

    void UpdateShaderWithLanternsPosition()
    {
        Shader.SetGlobalVectorArray("_centers", _positions);
        Shader.SetGlobalFloatArray("_distances", _distances);
        Shader.SetGlobalInt("_numberOfCenters", _positions.Count);

    }
}
