using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    [SerializeField] [Range(3, 7)] private float playerHealthMax = 3.0f;
    [SerializeField] [Range(0, 7)] private float playerHealthCurrent = 3.0f;

    [SerializeField] private float knockBackForce = 50f;
    [SerializeField] private float invincibleTime = 2f;
    private float knockBackCounter;
    private bool isInvincible = false;

    [SerializeField] private GameObject respawnPoint;
    [SerializeField] private GameObject deathTransition;
    private Image Black;
    private Animator anim;
    private Animator animPlayer;

    void Start() {
        if(respawnPoint == null) {
            respawnPoint = new GameObject();
            respawnPoint.transform.position = transform.position;
        }
        animPlayer = gameObject.GetComponent<Animator>();
        GameObject transitionImageInstance = Instantiate(deathTransition);

        Black = transitionImageInstance.GetComponent<Image>();
        anim = transitionImageInstance.GetComponent<Animator>();
    }

    void Update() {

        if(knockBackCounter > 0) {
            knockBackCounter -= Time.deltaTime;
        }
        else {
            isInvincible = false;
            GetComponent<Renderer>().sharedMaterial.SetFloat("_width", 0);

        }
    }

    //void OnGUI() {
    //    float x = 10;
    //    for (int i = 1; i <= playerHealthCurrent; i++) {
    //        GUI.DrawTexture(new Rect(x, 10, 60, 60), spriteHeart, ScaleMode.ScaleToFit);
    //        x += 70;
    //    }
    //}

    public void LooseHP(float dmg) {
        if (!isInvincible) {
            playerHealthCurrent = playerHealthCurrent - dmg;
            KnockBack();
            if (playerHealthCurrent <= 0) {
                PlayerDie();
            }
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
        gameObject.GetComponent<KodaController>().ResetPlayer();
        playerHealthCurrent = playerHealthMax;
        knockBackCounter = 0;
        GetComponent<Renderer>().sharedMaterial.SetFloat("_width", 0);
        isInvincible = false;

    }

    IEnumerator Fading() {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => Black.color.a == 1);
        anim.SetBool("Fade", false);
        animPlayer.SetBool("isDead", false);
        animPlayer.transform.SetPositionAndRotation(respawnPoint.transform.position, Quaternion.identity);
    }

    public void KnockBack() {
        
        knockBackCounter = invincibleTime;
        Vector3 knockBackDirection = -transform.forward + Vector3.up;
        GetComponent<Rigidbody>().AddForce(knockBackForce * knockBackDirection, ForceMode.Impulse);
        isInvincible = true;
        GetComponent<Renderer>().sharedMaterial.SetFloat("_width", 0.035f);
        GetComponent<Renderer>().sharedMaterial.SetVector("_color", new Vector3(1,0,1));

    }
}
