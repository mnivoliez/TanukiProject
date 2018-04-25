using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//================================================
//SOUNDCONTROLER
//================================================

public class CheckPoint : MonoBehaviour {
    public static GameObject LoadingScreen;

    private void OnTriggerEnter(Collider other) {

        if(other.CompareTag("Player")) {
            //================================================
            SoundController.instance.SelectENVQuick("Checkpoint");
            //================================================
            Game.PreSave_Game_and_Save();
            StartCoroutine(SavingLogo());
            //Debug.Log(Application.persistentDataPath);
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
}
