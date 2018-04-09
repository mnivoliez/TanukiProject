using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

	public void LoadByIndex(int sceneIndex) {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        GC.Collect();
        SceneManager.LoadScene(sceneIndex);
	}
	public void LoadByIndex(string sceneName) {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        GC.Collect();
        SceneManager.LoadScene(sceneName);
    }
}
