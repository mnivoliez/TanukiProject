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
    private Animator animTransition;
    private Animator animPlayer;

    void Start() {
        if(respawnPoint == null) {
            respawnPoint = new GameObject();
            respawnPoint.transform.position = transform.position;
        }
        animPlayer = gameObject.GetComponent<Animator>();
        GameObject transitionImageInstance = Instantiate(deathTransition);

        Black = transitionImageInstance.GetComponent<Image>();
        animTransition = transitionImageInstance.GetComponent<Animator>();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().RecenterCamera();
    }

    void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================

        if (knockBackCounter > 0) {
            knockBackCounter -= Time.deltaTime;
        }
        else {
            isInvincible = false;
            //GetComponent<Renderer>().sharedMaterial.SetFloat("_width", 0);

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
            animPlayer.SetTrigger("Hit");
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

    IEnumerator Fading() {
        animTransition.SetBool("Fade", true);
        yield return new WaitUntil(() => Black.color.a == 1);
        gameObject.transform.rotation = new Quaternion (0,0,0,0);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().RecenterCamera();
        animTransition.SetBool("Fade", false);
        gameObject.transform.SetPositionAndRotation(respawnPoint.transform.position, Quaternion.identity);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerHealthCurrent = playerHealthMax;
        gameObject.GetComponent<InputController>().SetFreezeInput(false);
        isInvincible = false;
    }

    public void KnockBack() {
        
        knockBackCounter = invincibleTime;
        Vector3 knockBackDirection = -transform.forward + Vector3.up;
        GetComponent<Rigidbody>().AddForce(knockBackForce * knockBackDirection, ForceMode.Impulse);
        isInvincible = true;
        //GetComponent<Renderer>().sharedMaterial.SetFloat("_width", 0.035f);
        //GetComponent<Renderer>().sharedMaterial.SetVector("_color", new Vector3(1,0,1));

    }

    public void SetHealthCurrent(float current_hp) {
        playerHealthCurrent = current_hp;
    }
    public void SetHealthMax(float current_max_hp) {
        playerHealthMax = current_max_hp;
    }

    public void SetRespawnPoint(GameObject checkPoint) {
        respawnPoint = checkPoint;
    }

    public void PlayerDie() {
        gameObject.GetComponent<InputController>().SetFreezeInput(true);
        animPlayer.SetTrigger("Death");
        StartCoroutine(Fading());
        gameObject.GetComponent<KodaController>().ResetPlayer();
        knockBackCounter = 0;
        //GetComponent<Renderer>().sharedMaterial.SetFloat("_width", 0);
    }
}