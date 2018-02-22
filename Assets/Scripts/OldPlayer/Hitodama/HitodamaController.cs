using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitodamaController : MonoBehaviour {

    public GameObject spawnHitodama;
    [SerializeField] private float speed = 8;
    [SerializeField] private float initialMinSize = 0.025f;
    private Renderer rendererHitodama;
    [SerializeField] private Color diffEmissionColor = new Color(0.05f, 0.12f, 0.14f);
    [SerializeField] private Color diffColor = new Color(0.11f, 0.14f, 0.14f, 0.05f);
    [SerializeField] [Range(0,7)] int hpPlayer = 7;
    [SerializeField] [Range(3, 7)] int hpMaxPlayer = 7;

    void Start() {
        rendererHitodama = GetComponent<Renderer>();
        Debug.Log("Color:" + rendererHitodama.material.color);
    }

    void FixedUpdate() {

        transform.position = Vector3.Lerp(transform.position, spawnHitodama.transform.position, speed * Time.deltaTime);
        PlayerUpdateLife();
    }

    void PlayerUpdateLife() {
        //float hpPlayer = 7;
        float newScale;
        rendererHitodama.material.color = CalculNewColor(hpPlayer);
        rendererHitodama.material.SetColor("_EmissionColor", CalculNewEmissionColor(hpPlayer));
        newScale = initialMinSize + hpMaxPlayer * initialMinSize;
        transform.localScale = new Vector3 (newScale, newScale, newScale);
    }

    Color CalculNewColor(float hpPlayer) {
        Color colorHitodama = new Color(0,0,0,0.58f);
        colorHitodama = colorHitodama + (diffColor * hpPlayer);
        return colorHitodama;
    }

    Color CalculNewEmissionColor(float hpPlayer) {
        Color emissionColorHitodama = new Color(0, 0, 0, 0);
        emissionColorHitodama = emissionColorHitodama + (diffEmissionColor * hpPlayer);
        return emissionColorHitodama;
    }

}
