﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehavior : MonoBehaviour {

    [SerializeField] private LanternController Lantern_Hiyoribou;
    private bool isAlreadyLight = false;
    public static GameObject LoadingScreen;

    void Start () {
		
	}
	

	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        if (!isAlreadyLight) {
            if (other.CompareTag("Player")) {
                isAlreadyLight = true;
                StartCoroutine(LightTransition());
                other.gameObject.GetComponent<KodaController>().SetPowerJump(true);
                Game.playerData.lightBoss1 = true;
                Game.PreSave_Game_and_Save();
                //================================================
                StartCoroutine(SoundController.instance.FadeOnEnterTheme()); //Will launch an other theme automatically
                //================================================
                StartCoroutine(SavingLogo());
            }

        }

    }

    IEnumerator SavingLogo() {
        LoadingScreen = GameObject.Find("LoadingScreen");
        if (LoadingScreen != null) {
            LoadingScreen.transform.GetChild(1).gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            LoadingScreen.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    private IEnumerator LightTransition() {
        float lerpRay = 0;
        while ( lerpRay < 1000f) {
            lerpRay+= 0.2f;
            Lantern_Hiyoribou.SetRadiusForPurification(lerpRay);
            yield return new WaitForSeconds(0.01f);
        }

    }

}
