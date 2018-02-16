using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpMatTest : MonoBehaviour
{

    [SerializeField] private Material _lightMat;
    [SerializeField] private Material _darkMat;
    public float duration = 2.0F;
    private Renderer _rend;

    // Use this for initialization
    void Start()
    {
        _rend = GetComponent<Renderer>();
        _rend.material = _darkMat;
    }

    // Update is called once per frame
    void Update()
    {
        float lerp = Mathf.PingPong(Time.time, duration) / duration;
        _rend.material.Lerp(_darkMat, _lightMat, lerp);
    }
}
