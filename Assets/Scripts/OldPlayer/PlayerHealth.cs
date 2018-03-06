using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    [SerializeField] [Range(3, 7)] private float playerHealthMax = 3.0f;
    [SerializeField] [Range(0, 7)] private float playerHealthCurrent = 3.0f;

    [SerializeField] private GameObject respawnPoint;
    [SerializeField] private GameObject deathTransition;
    private Image Black;
    private Animator anim;
    private Animator animPlayer;

    void Start() {
        animPlayer = gameObject.GetComponent<Animator>();
        GameObject transitionImageInstance = Instantiate(deathTransition);

        Black = transitionImageInstance.GetComponent<Image>();
        anim = transitionImageInstance.GetComponent<Animator>();
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

    public void LooseHP(float dmg) {
        playerHealthCurrent = playerHealthCurrent - dmg;
        if (playerHealthCurrent <= 0) {
            PlayerDie();
        }
    }

    public void GainHP(float nbHP) {
        playerHealthCurrent = playerHealthCurrent + nbHP;
        if (playerHealthCurrent > playerHealthMax) {
            playerHealthCurrent = playerHealthMax;
        }
    }

    public float GetHealthCurrent() {
        return playerHealthCurrent;
    }

    public float GetHealthMax() {
        return playerHealthMax;
    }

    public void PlayerDie() {
        animPlayer.SetBool("isDead", true);
        StartCoroutine(Fading());
        playerHealthCurrent = playerHealthMax;
    }

    IEnumerator Fading() {
        Debug.Log("COUCOU !");
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => Black.color.a == 1);
        Debug.Log("never !");
        anim.SetBool("Fade", false);
        animPlayer.SetBool("isDead", false);
        animPlayer.transform.SetPositionAndRotation(respawnPoint.transform.position, Quaternion.identity);
    }
}
