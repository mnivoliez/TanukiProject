using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractBehavior : MonoBehaviour {

    [Header("GLIDE")]
    [Space(8)]
    [SerializeField]
    private GameObject ParachuteLeaf;

    [Header("MELEE ATTACK")]
    [Space(8)]
    [SerializeField]
    private GameObject leafHead;
    [SerializeField] private GameObject leafHand;
    [SerializeField] private GameObject attackRange;
    [SerializeField] private float meleeDamage;

    [Header("DISTANT ATTACK")]
    [Space(8)]
    [SerializeField]
    private GameObject leafPrefab;
    [SerializeField] private GameObject spawnLeaf;
    [SerializeField] private GameObject rangeMaxLeaf;
    [SerializeField] private float distantDamage;

    [Header("INFLATE")]
    [Space(8)]
    [SerializeField]
    private GameObject normalForm;
    [SerializeField] private GameObject inflateForm;
    [SerializeField] private GameObject smokeSpawner;


    [Header("ABSORB")]
    [Space(8)]
    [SerializeField]
    private float absorptionTimer = 4f;
    [SerializeField] private GameObject sakePot;

    [Header("LURE")]
    [Space(8)]
    [SerializeField]
    private GameObject lure;
    [SerializeField]
    private Transform tanukiPlayer;

    //QTE
    private float maxAbsorptionGauge = 4f;
    private float absorptionGauge = 0f;
    public GameObject canvasQTE;
    public Transform loadingBar;
    public Transform centerButton;

    // Use this for initialization
    void Start () {
        attackRange.GetComponent<MeleeAttackTrigger>().SetDamage(meleeDamage);
        leafHand.SetActive(false);
        
        sakePot.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

    }

    public void DoGlide() {
        leafHead.SetActive(false);
        ParachuteLeaf.SetActive(true);
    }

    public void StopGlide() {
        ParachuteLeaf.SetActive(false);
        leafHead.SetActive(true);
    }

    public void DoMeleeAttack() {
        leafHead.SetActive(false);
        attackRange.SetActive(true);
        leafHand.SetActive(true);
    }

    public void StopMeleeAttack() {
        leafHead.SetActive(true);
        attackRange.SetActive(false);
        leafHand.SetActive(false);
    }

    public void DoChargedMeleeAttack() {
        leafHead.SetActive(false);
        attackRange.SetActive(true);
        leafHead.SetActive(false);
        leafHand.SetActive(true);
    }

    public void DoDistantAttack() {
        leafHead.SetActive(false);
        GameObject LeafBoomerang = Instantiate(leafPrefab, spawnLeaf.transform.position, leafPrefab.transform.rotation);
        LeafBoomerang.GetComponent<MoveLeaf>().SetSpawnPosition(spawnLeaf);
        LeafBoomerang.GetComponent<MoveLeaf>().SetTargetPosition(rangeMaxLeaf);
        LeafBoomerang.GetComponent<MoveLeaf>().SetDamage(distantDamage);
    }

    public void StopDistantAttack() {
        leafHead.SetActive(true);
    }

    public void DoInflate(bool inflate) {
        if (inflate) {
            GameObject smoke = Instantiate(smokeSpawner, gameObject.transform.position, Quaternion.identity);
            Destroy(smoke, 4);
            //inflateForm.transform.position = normalForm.transform.position;
            //inflateForm.transform.rotation = normalForm.transform.rotation;
            //inflateForm.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //inflateForm.SetActive(true);
            //normalForm.SetActive(false);
        } else {
            GameObject smoke = Instantiate(smokeSpawner, gameObject.transform.position, Quaternion.identity);
            Destroy(smoke, 4);
            normalForm.transform.position = inflateForm.transform.position;
            //normalForm.SetActive(true);
            //inflateForm.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //inflateForm.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            //inflateForm.transform.rotation = Quaternion.identity;
            //inflateForm.SetActive(false);
        }
    }

    public void DoBeginAbsorption(GameObject absorbableObject) {

        canvasQTE.SetActive(true);
        sakePot.SetActive(true);


    }

    public void DoContinueAbsorption(GameObject absorbableObject) {
        Debug.Log("PAS Coucou");
        if (absorbableObject.CompareTag("Yokai") && absorbableObject.GetComponent<YokaiController>().GetIsKnocked() && absorptionTimer > 0) {

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
                absorbableObject.GetComponent<YokaiController>().Absorbed();
                gameObject.GetComponent<PlayerCollectableController>().AddYokai();

                absorptionTimer = 4f;
                absorptionGauge = 0;
                centerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
                centerButton.GetComponent<Image>().color = Color.white;
                sakePot.SetActive(false);
                canvasQTE.SetActive(false);

            }
        } else {
            
            sakePot.SetActive(false);
            absorptionGauge = 0;
            absorptionTimer = 4f;
            centerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
            centerButton.GetComponent<Image>().color = Color.white;
            canvasQTE.SetActive(false);

        }
    }

    public void doSpawnLure() {
        if (leafHead.activeSelf && GameObject.FindGameObjectWithTag("Lure") == null) {
            leafHead.SetActive(false);
            GameObject clone = Instantiate(lure, tanukiPlayer.position, tanukiPlayer.rotation);
            clone.transform.Translate(0, 3, 2);
        }
    }


}
