using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    [SerializeField]
    private float playerHealthMax = 3.0f;
    [SerializeField]
    private float playerHealthCurrent;
    [SerializeField]
    private Texture spriteHeart;

    // Use this for initialization
    void Start() {
        playerHealthCurrent = playerHealthMax;
    }

    // Update is called once per frame
    void Update() {

    }

    void OnGUI() {
        float x = 10;
        for (int i = 1; i <= playerHealthCurrent; i++) {
            GUI.DrawTexture(new Rect(x, 10, 60, 60), spriteHeart, ScaleMode.ScaleToFit);
            x += 70;
        }
    }

    public void LooseHP(float dame) {
        playerHealthCurrent = playerHealthCurrent - dame;
    }
}
