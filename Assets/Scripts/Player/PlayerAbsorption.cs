using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerAbsorption : MonoBehaviour {

    private Animator animBody;
    public GameObject sakePot;
    private bool currentAbsorption;
    [SerializeField]
    private float absorptionTimer = 4f;
    [SerializeField]
    private float maxAbsorptionGauge = 4f;
    private float absorptionGauge = 0f;

    public GameObject canvasQTE;
    public Transform loadingBar;
    public Transform centerButton;

    void Start() {
        animBody = GetComponent<Animator>();
        sakePot.SetActive(false);
        currentAbsorption = false;
    }

    void Update() {

    }

    void OnTriggerStay(Collider collid) {
        
        if (!currentAbsorption) {

            if (Input.GetButtonDown("Fire3")) {

                if (collid.gameObject.CompareTag("Yokai") && collid.gameObject.GetComponent<YokaiController>().GetIsKnocked()) {
                    canvasQTE.SetActive(true);
                    currentAbsorption = true;
                    animBody.SetBool("isAbsorbing", true);
                    sakePot.SetActive(true);
                   
                }
            }
        }
        else {
            if (collid.gameObject.CompareTag("Yokai") && collid.gameObject.GetComponent<YokaiController>().GetIsKnocked() && absorptionTimer > 0) {
                
                centerButton.GetComponent<Image>().color = Color.white;
                absorptionTimer -= 0.03f;
                absorptionGauge -= 0.01f;
                if (Input.GetButtonDown("Fire3")) {
                    centerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(centerButton.GetComponent<RectTransform>().sizeDelta.x + 5, centerButton.GetComponent<RectTransform>().sizeDelta.y + 5);
                    centerButton.GetComponent<Image>().color = Color.grey;
                    absorptionGauge += 1;
                    
                }

                loadingBar.GetComponent<Image>().fillAmount = absorptionTimer * 25 / 100;

                if (absorptionGauge > maxAbsorptionGauge) {
                    collid.gameObject.GetComponent<YokaiController>().Absorbed();
                    gameObject.GetComponent<PlayerCollectableController>().AddYokai();
                    currentAbsorption = false;
                    absorptionTimer = 4f;
                    absorptionGauge = 0;
                    centerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
                    centerButton.GetComponent<Image>().color = Color.white;
                    canvasQTE.SetActive(false);
                    
                }


            }
            else {
                animBody.SetBool("isAbsorbing", false);
                sakePot.SetActive(false);
                currentAbsorption = false;
                absorptionGauge = 0;
                absorptionTimer = 4f;
                centerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
                centerButton.GetComponent<Image>().color = Color.white;
                canvasQTE.SetActive(false);

            }
        }
    }
}
