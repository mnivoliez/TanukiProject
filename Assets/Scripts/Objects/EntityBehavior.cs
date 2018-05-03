using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntityBehavior : MonoBehaviour {

    [SerializeField] private LanternController Lantern_Hiyoribou;
    private bool isAlreadyLight = false;
    public static GameObject LoadingScreen;

    private void OnTriggerEnter(Collider other) {
        if (!isAlreadyLight) {
            if (other.CompareTag("Player")) {
                Lantern_Hiyoribou.transform.position = transform.position;
                isAlreadyLight = true;
                StartCoroutine(LightTransition());
                if(SceneManager.GetActiveScene().name == "Boss1") { Game.playerData.lightBoss1 = true; }
                if (SceneManager.GetActiveScene().name == "Boss2") { Game.playerData.lightBoss2 = true; }
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
        float lerpRay = 10;
        while ( lerpRay < 1000f) {
            lerpRay+= 0.2f;
            Lantern_Hiyoribou.SetRadiusForPurification(lerpRay);
            yield return new WaitForSeconds(0.01f);
        }

    }

}
