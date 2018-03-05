using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    [SerializeField] [Range(3, 7)] private float playerHealthMax = 3.0f;
    [SerializeField] [Range(0, 7)] private float playerHealthCurrent = 3.0f;


    void Start() {

    }

    void Update() {

    }

    //void OnGUI() {
    //    float x = 10;
    //    for (int i = 1; i <= playerHealthCurrent; i++) {
    //        GUI.DrawTexture(new Rect(x, 10, 60, 60), spriteHeart, ScaleMode.ScaleToFit);
    //        x += 70;
    //    }
    //}

    public void LooseHP(float dame) {
        playerHealthCurrent = playerHealthCurrent - dame;
    }

    public void GainHP(float nbHP) {
        playerHealthCurrent = playerHealthCurrent + nbHP;
        if(playerHealthCurrent> playerHealthMax) {
            playerHealthCurrent = playerHealthMax;
        }
    }

    public float GetHealthCurrent() {
        return playerHealthCurrent;
    }

    public float GetHealthMax() {
        return playerHealthMax;
    }
}
