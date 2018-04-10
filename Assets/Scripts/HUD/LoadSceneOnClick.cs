using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

    public void LoadByIndex(int sceneIndex) {

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(sceneIndex);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
	public void LoadByIndex(string sceneName) {

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(sceneName);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
}
