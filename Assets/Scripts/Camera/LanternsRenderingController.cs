using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternsRenderingController : MonoBehaviour
{

    private LanternController[] _lanterns;
    private List<Vector4> _positions;
    private List<float> _distances;
    private bool _isDirty;

    // If changed, restart unity to it to be effective.
    private const int MAX_NUMBER_OF_LANTERNS = 5;
    void Awake()
    {
        List<LanternController> temp = new List<LanternController>();
        foreach (GameObject l in GameObject.FindGameObjectsWithTag("Lantern"))
        {
            temp.Add(l.GetComponent<LanternController>());
        }
        _lanterns = temp.ToArray();
        _positions = new List<Vector4>(5);
        _distances = new List<float>(5);
        Shader.SetGlobalVectorArray("_centers", new Vector4[MAX_NUMBER_OF_LANTERNS]);
        Shader.SetGlobalFloatArray("_distances", new float[MAX_NUMBER_OF_LANTERNS]);
        foreach (var lantern in _lanterns)
        {
            _positions.Add(lantern.transform.position);
            _distances.Add(lantern.GetRadiusEffect());
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
        _positions.Clear();
        _distances.Clear();
        foreach (var lantern in _lanterns)
        {
            _positions.Add(lantern.transform.position);
            _distances.Add(lantern.GetRadiusEffect());
        }
        Shader.SetGlobalVectorArray("_centers", _positions);
        Shader.SetGlobalFloatArray("_distances", _distances);
        Shader.SetGlobalInt("_numberOfCenters", _positions.Count);

    }
}
