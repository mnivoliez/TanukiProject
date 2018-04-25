using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//================================================
//SOUNDCONTROLER
//================================================

public class PlayerHealth : MonoBehaviour {

    [SerializeField] [Range(3, 7)] private float playerHealthMax = 3.0f;
    [SerializeField] [Range(0, 7)] private float playerHealthCurrent = 3.0f;

    [SerializeField] private float knockBackForce = 50f;
    [SerializeField] private float invincibleTime = 2f;
    private float knockBackCounter;
    private bool isInvincible = false;

    [SerializeField] private GameObject respawnPoint;
    [SerializeField] private GameObject deathTransition;
    [SerializeField] private GameObject CanvasHealth;
    private Image Black;
    private Animator animTransition;
    private Animator animPlayer;
    //[SerializeField] private GameObject Tanuki_Body;
    //private Material matAuraIFrame;


    void Start() {
        if (respawnPoint == null) {
            respawnPoint = new GameObject();
            respawnPoint.transform.position = transform.position;
        }
        animPlayer = gameObject.GetComponent<Animator>();
        GameObject transitionImageInstance = Instantiate(deathTransition);

        Black = transitionImageInstance.GetComponent<Image>();
        animTransition = transitionImageInstance.GetComponent<Animator>();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().RecenterCamera();
        //matAuraIFrame = Tanuki_Body.GetComponent<Renderer>().materials[1];
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
            //matAuraIFrame.SetFloat("_Edge", 0f);
            //matAuraIFrame.SetFloat("_RimPower", 10f);
            //matAuraIFrame.SetFloat("_Outline", 0.002f);
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
            //================================================
            SoundController.instance.SelectKODA("Hurt");
            //================================================
            animPlayer.SetTrigger("Hit");
            playerHealthCurrent = playerHealthCurrent - dmg;
            knockBackCounter = invincibleTime;
            isInvincible = true;
            if (playerHealthCurrent <= 0) {
                PlayerDie();
            }
            else {
                if (playerHealthCurrent == 1) {
                    CanvasHealth.SetActive(true);
                    StartCoroutine(LowHPEffect());
                }
                KnockBack();
            }
        }

    }

    public void GainHP(float nbHP) {
        //================================================
        SoundController.instance.SelectENVQuick("Kaki");
        //================================================
        if (playerHealthCurrent == 1) {
            CanvasHealth.SetActive(false);
        }
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
        CanvasHealth.SetActive(false);
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        gameObject.transform.SetPositionAndRotation(respawnPoint.transform.position, respawnPoint.transform.rotation);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().RecenterCamera();
        animTransition.SetBool("Fade", false);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        playerHealthCurrent = playerHealthMax;
        gameObject.GetComponent<InputController>().SetFreezeInput(false);
        isInvincible = false;
    }

    public void KnockBack() {

        //matAuraIFrame.SetFloat("_Edge", 0.4f);
        //matAuraIFrame.SetFloat("_RimPower", 1.6f);
        //matAuraIFrame.SetFloat("_Outline", 0.77f);
        Vector3 knockBackDirection = -transform.forward + Vector3.up;
        GetComponent<Rigidbody>().AddForce(knockBackForce * knockBackDirection, ForceMode.Impulse);
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


    IEnumerator LowHPEffect() {
        float lerpAlpha1 = 0, lerpAlpha2 = 0.5f, lerpAlpha3 = 0;
        Image DyingScreen1 = CanvasHealth.transform.GetChild(0).GetComponent<Image>();
        Image DyingScreen2 = CanvasHealth.transform.GetChild(1).GetComponent<Image>();
        Image DyingScreen3 = CanvasHealth.transform.GetChild(2).GetComponent<Image>();
        RectTransform DS1RecTransform = CanvasHealth.transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform DS3RecTransform = CanvasHealth.transform.GetChild(2).GetComponent<RectTransform>();
        Color colorFade = Color.white;
        bool inverse = false;
        float scaleDistort1 = 1;
        Vector3 scaleDistortVector1 = Vector3.one;
        float scaleDistort3 = 1;
        Vector3 scaleDistortVector3 = Vector3.one;

        while (playerHealthCurrent == 1) {

            if (lerpAlpha3 > 0.9f) {
                inverse = true;
            }

            if (lerpAlpha3 < 0.35f) {
                inverse = false;
            }


            if (inverse) {
                //Alpha
                lerpAlpha1 = Mathf.Lerp(lerpAlpha1, 0.1f, 0.3f);
                colorFade.a = lerpAlpha1;
                DyingScreen1.color = colorFade;
                lerpAlpha2 = Mathf.Lerp(lerpAlpha2, 0.5f, 0.05f);
                colorFade.a = lerpAlpha2;
                DyingScreen2.color = colorFade;
                lerpAlpha3 = Mathf.Lerp(lerpAlpha3, 0.3f, 0.2f);
                colorFade.a = lerpAlpha3;
                DyingScreen3.color = colorFade;
                //Scale
                scaleDistort1 = Mathf.Lerp(scaleDistort1, 1.1f, 0.05f);
                scaleDistortVector1.Set(scaleDistort1, scaleDistort1, scaleDistort1);
                DS1RecTransform.localScale = scaleDistortVector1;

                scaleDistort3 = Mathf.Lerp(scaleDistort3, 1, 0.05f);
                scaleDistortVector3.Set(scaleDistort3, scaleDistort3, scaleDistort3);
                DS3RecTransform.localScale = scaleDistortVector3;

            }
            else {
                //Alpha
                lerpAlpha1 = Mathf.Lerp(lerpAlpha1, 1, 0.3f);
                colorFade.a = lerpAlpha1;
                DyingScreen1.color = colorFade;
                lerpAlpha2 = Mathf.Lerp(lerpAlpha2, 1, 0.05f);
                colorFade.a = lerpAlpha2;
                DyingScreen2.color = colorFade;
                lerpAlpha3 = Mathf.Lerp(lerpAlpha3, 1, 0.2f);
                colorFade.a = lerpAlpha3;
                DyingScreen3.color = colorFade;
                //Scale
                scaleDistort1 = Mathf.Lerp(scaleDistort1, 1, 0.05f);
                scaleDistortVector1.Set(scaleDistort1, scaleDistort1, scaleDistort1);
                DS1RecTransform.localScale = scaleDistortVector1;

                scaleDistort3 = Mathf.Lerp(scaleDistort3, 1.1f, 0.05f);
                scaleDistortVector3.Set(scaleDistort3, scaleDistort3, scaleDistort3);
                DS3RecTransform.localScale = scaleDistortVector3;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

}