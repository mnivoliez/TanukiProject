using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LoadSceneManager : MonoBehaviour {

    public string sceneNameToLoad;
    public string sceneNameToUnload;

    private Image Black;
    private Animator anim;
    private Canvas canvasLoading;
    private GameObject loadingMainPanel;

    GameObject transitionImageInstance;
    GameObject loadingScreen;

    private bool loadScene = false;


    void Start() {
        transitionImageInstance = GameObject.Find("SceneTransitionImage");
        if (transitionImageInstance != null) {
            Black = transitionImageInstance.GetComponent<Image>();
            anim = transitionImageInstance.GetComponent<Animator>();
        }

        loadingScreen = GameObject.Find("LoadingScreen");
        if (loadingScreen != null) {
            canvasLoading = loadingScreen.GetComponent<Canvas>();
            loadingMainPanel = canvasLoading.transform.GetChild(0).gameObject;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            LoadByIndex(sceneNameToLoad, sceneNameToUnload);
        }
    }

    public void LoadByIndexMM(string sceneNameToLoad) {
        if(SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "CreditScene" && SceneManager.GetActiveScene().name != "Introduction" && SceneManager.GetActiveScene().name != "FinTuto") { Game.PreSave_Game_and_Save(); }
        if (Pause.Paused) {
            GameObject PauseCanvas = GameObject.Find("PauseCanvas");
            if(PauseCanvas != null) {
                PauseCanvas.transform.GetComponent<Pause>().UnpauseGame();
            }
        }
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        StartCoroutine(LoadAsyncScene(sceneNameToLoad));
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    public void LoadByIndex(string sceneNameToLoad, string sceneNameToUnload) {
        if (SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "CreditScene" && SceneManager.GetActiveScene().name != "Introduction" && SceneManager.GetActiveScene().name != "FinTuto") { Game.PreSave_Game_and_Save(); }
        if (Pause.Paused) {
            GameObject PauseCanvas = GameObject.Find("PauseCanvas");
            if (PauseCanvas != null) {
                PauseCanvas.transform.GetComponent<Pause>().UnpauseGame();
            }
        }
        SceneManager.UnloadSceneAsync(sceneNameToUnload);
        StartCoroutine(LoadAsyncScene(sceneNameToLoad));
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

    // The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
    IEnumerator LoadAsyncScene(string sceneNameToLoad) {

        if (anim != null) {
            StartCoroutine(FadeIn());
            yield return new WaitForSeconds(1);
        }
        loadingMainPanel.SetActive(true);
        if (anim != null) {
            transitionImageInstance.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(2);
        AsyncOperation async_Load = SceneManager.LoadSceneAsync(sceneNameToLoad);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async_Load.isDone) {
            yield return null;
        }
    }

    IEnumerator FadeIn() {
        if (anim != null) {
            anim.SetBool("Fade", true);
            yield return new WaitUntil(() => Black.color.a == 1);
        }
    }
}
