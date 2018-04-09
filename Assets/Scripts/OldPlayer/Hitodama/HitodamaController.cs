using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitodamaController : MonoBehaviour {

    [SerializeField] private GameObject spawnHitodama;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float speed = 8;
    [SerializeField] private float initialMinSize = 0.025f;
    private Renderer rendererHitodama;
    [SerializeField] private Color diffEmissionColor = new Color(0.05f, 0.12f, 0.14f);
    [SerializeField] private Color diffColor = new Color(0.11f, 0.14f, 0.14f, 0.05f);
    [SerializeField] private GameObject gainHPEffect;
    [SerializeField] private GameObject lostHPEffect;
    private float hpPlayer = 3;
    private float hpMaxPlayer = 3;

    void Start() {
        rendererHitodama = GetComponent<Renderer>();
    }

    void FixedUpdate() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        if (hpPlayer < playerHealth.GetHealthCurrent()) {
            GameObject gainHPObject = Instantiate(gainHPEffect, transform.position, Quaternion.identity);
            gainHPObject.transform.parent = gameObject.transform;
            Destroy(gainHPObject, 2f);
        }else if (hpPlayer > playerHealth.GetHealthCurrent()) {
            GameObject lostHPObject = Instantiate(lostHPEffect, transform.position, Quaternion.identity);
            lostHPObject.transform.parent = gameObject.transform;
            Destroy(lostHPObject, 2f);
        }
        hpPlayer = playerHealth.GetHealthCurrent();
        hpMaxPlayer = playerHealth.GetHealthMax();
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
