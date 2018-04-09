using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {

    public void LoadByIndex(int sceneIndex) {
        GameObject TimeLine = GameObject.FindGameObjectWithTag("TimeLine");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        if(TimeLine != null) { 
            Destroy(GameObject.FindGameObjectWithTag("TimeLine"));
        }
        SceneManager.LoadScene(sceneIndex);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
	public void LoadByIndex(string sceneName) {
        GameObject TimeLine = GameObject.FindGameObjectWithTag("TimeLine");
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
        if (TimeLine != null) {
            Destroy(GameObject.FindGameObjectWithTag("TimeLine"));
        }
        SceneManager.LoadScene(sceneName);
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }
}
