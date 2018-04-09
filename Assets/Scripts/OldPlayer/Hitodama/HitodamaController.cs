﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitodamaController : MonoBehaviour {

    [SerializeField] private GameObject spawnHitodama;
    private PlayerHealth playerHealth;
    [SerializeField] private float speed = 8;
    [SerializeField] private float initialMinSize = 0.025f;
    private Renderer rendererHitodama;
    [SerializeField] private Color diffEmissionColor = new Color(0.05f, 0.12f, 0.14f, 2f);
    //[SerializeField] private Color diffColor = new Color(0.11f, 0.14f, 0.14f, 0.05f);
    [SerializeField] private Color initColor = new Color(60f / 255f, 70f / 255f, 70f / 255f, 1f);
    [SerializeField] private Color firstColor = new Color(240f / 255f, 50f / 255f, 50f / 255f, 1.2f);
    [SerializeField] private Color secondColor = new Color(250f / 255f, 240f / 255f, 30f / 255f, 1.8f);
    [SerializeField] private Color thirdColor = new Color(0f / 255f, 190f / 255f, 255f / 255f, 2f);
    [SerializeField] private Color fourthColor = new Color(200f / 255f, 70f / 255f, 230f / 255f, 2f);
    [SerializeField] private GameObject gainHPEffect;
    [SerializeField] private GameObject lostHPEffect;
    private float hpPlayer = 3;
    private float hpMaxPlayer = 3;

    GameObject gainHPObject;
    GameObject lostHPObject;
    Color colorHitodama = new Color(10f / 255f, 250f / 255f, 250f / 255f, 1f);
    Color emissionColorHitodama = new Color(0, 0, 0, 1f);
    float newScaleHitodama;

    void Start() {
        rendererHitodama = GetComponent<Renderer>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    void FixedUpdate() {
        if (hpPlayer < playerHealth.GetHealthCurrent()) {
            gainHPObject = Instantiate(gainHPEffect, transform.position, Quaternion.identity);
            gainHPObject.transform.parent = gameObject.transform;
            Destroy(gainHPObject, 2f);
            PlayerUpdateLife();
        }
        else if (hpPlayer > playerHealth.GetHealthCurrent()) {
            lostHPObject = Instantiate(lostHPEffect, transform.position, Quaternion.identity);
            lostHPObject.transform.parent = gameObject.transform;
            Destroy(lostHPObject, 2f);
            PlayerUpdateLife();
        }
        transform.position = Vector3.Lerp(transform.position, spawnHitodama.transform.position, speed * Time.deltaTime);

    }

    void PlayerUpdateLife() {
        hpPlayer = playerHealth.GetHealthCurrent();
        hpMaxPlayer = playerHealth.GetHealthMax();
        rendererHitodama.material.color = CalculNewColor(hpPlayer);
        rendererHitodama.material.SetColor("_EmissionColor", CalculNewEmissionColor(hpPlayer));
        newScaleHitodama = initialMinSize + hpMaxPlayer * initialMinSize;
        transform.localScale.Set(newScaleHitodama, newScaleHitodama, newScaleHitodama);
    }

    Color CalculNewColor(float hpPlayer) {

        switch ((int)hpPlayer) {
            case 4:
                colorHitodama = fourthColor;
                break;

            case 3:
                colorHitodama = thirdColor;
                break;

            case 2:
                colorHitodama = secondColor;
                break;

            case 1:
                colorHitodama = firstColor;
                break;

            default:
                colorHitodama = initColor;
                break;
        }
        //Color colorHitodama = new Color(0, 0, 0, 0.58f);
        //colorHitodama = colorHitodama + (diffColor * hpPlayer);
        return colorHitodama;
    }

    Color CalculNewEmissionColor(float hpPlayer) {
        switch ((int)hpPlayer) {
            case 4:
                emissionColorHitodama = fourthColor;
                break;

            case 3:
                emissionColorHitodama = thirdColor;
                break;

            case 2:
                emissionColorHitodama = secondColor;
                break;

            case 1:
                emissionColorHitodama = firstColor;
                break;

            default:
                emissionColorHitodama = initColor;
                break;
        }
        //emissionColorHitodama = emissionColorHitodama + (diffEmissionColor * hpPlayer);
        return emissionColorHitodama;
    }

}
