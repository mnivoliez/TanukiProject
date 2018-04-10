using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LanternsRenderingController : MonoBehaviour
{
    private List<LanternController> lanterns;
    private List<Vector4> positions;
    private List<float> distances;

    [Header("Dissolve")]
    [Space(10)]
    public Color dissoColor;
    [Range(0, 5)]
    public float interpolation;
    [Range(0, 5)]
    public float offset;
    [Range(0, 1)]
    public float frequency;
    [Range(0, 5)]
    public float speed;
    private bool _isDirty;

    // If changed, restart unity to it to be effective.
    private const int MAX_NUMBER_OF_LANTERNS = 20;
    void Awake()
    {
        lanterns = new List<LanternController>();
        positions = new List<Vector4>();
        distances = new List<float>();
        foreach (GameObject l in GameObject.FindGameObjectsWithTag("Lantern"))
        {
            lanterns.Add(l.GetComponent<LanternController>());
        }

        Shader.SetGlobalVectorArray("_CentersLantern", new Vector4[MAX_NUMBER_OF_LANTERNS]);
        Shader.SetGlobalFloatArray("_DistancesLantern", new float[MAX_NUMBER_OF_LANTERNS]);

        UpdateShaderWithLanternsPosition();
        _isDirty = false;
    }

    // Update is called once per frame
    void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        int i = 0;
        while (!_isDirty && i < lanterns.Count)
        {
            _isDirty = !lanterns[i].transform.position.Equals(positions[i]);
            ++i;
        }

        if (_isDirty)
        {
            UpdateShaderWithLanternsPosition();
            _isDirty = false;
        }

    }

    void UpdateShaderWithLanternsPosition()
    {
        positions.Clear();
        distances.Clear();
        foreach (var lantern in lanterns)
        {
            positions.Add(lantern.transform.position);
            distances.Add(lantern.GetRadiusEffect());
        }

        Shader.SetGlobalColor("_ColorDisso", dissoColor);
        Shader.SetGlobalFloat("_Interpolation", interpolation);
        Shader.SetGlobalFloat("_Offset", offset);
        Shader.SetGlobalFloat("_Freq", frequency);
        Shader.SetGlobalFloat("_Speed", speed);

        Shader.SetGlobalInt("_LanternCount", lanterns.Count);
        Shader.SetGlobalFloatArray("_DistancesLantern", distances);
        Shader.SetGlobalVectorArray("_CentersLantern", positions);

    }
}
