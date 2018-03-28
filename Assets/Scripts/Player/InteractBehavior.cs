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
    [SerializeField] private float distantAttackRange = 8f;
    [SerializeField] private float distandAttackConeAngle = 20f;
    private HashSet<GameObject> enemiesInRange;
    [SerializeField] private float distantDamage;

    [Header("INFLATE")]
    [Space(8)]
    [SerializeField]
    private GameObject normalForm;
    [SerializeField] private GameObject inflateForm;
    [SerializeField] private GameObject smokeSpawner;

    [Header("RESIZE")]
    [Space(8)]
    [SerializeField]
    private float coefResize = 4;


    [Header("ABSORB")]
    [Space(8)]
    [SerializeField]
    private float absorptionTimer = 4f;
    [SerializeField] private GameObject sakePot;
    [SerializeField] private AudioClip absorption;

    [Header("LURE")]
    [Space(8)]
    [SerializeField]
    private GameObject lure;
    [SerializeField] private Transform tanukiPlayer;

    //QTE
    private float maxAbsorptionGauge = 4f;
    private float absorptionGauge = 0f;
    private GameObject absorptionTarget;
    public GameObject canvasQTE;
    public Transform loadingBar;
    public Transform centerButton;

    //Catchable Object
    private GameObject catchSlot;

    // Use this for initialization
    void Start() {
        enemiesInRange = new HashSet<GameObject>();
        attackRange.GetComponent<MeleeAttackTrigger>().SetDamage(meleeDamage);
        leafHand.SetActive(false);
        catchSlot = GameObject.Find("Catchable Object");
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
        GameObject leafBoomerang = Instantiate(leafPrefab, spawnLeaf.transform.position, leafPrefab.transform.rotation);
        MoveLeaf moveLeaf = leafBoomerang.GetComponent<MoveLeaf>();
        moveLeaf.SetSpawnPosition(spawnLeaf);
        moveLeaf.SetDamage(distantDamage);

        /* init target */
        // define default point
        Vector3 target = tanukiPlayer.transform.position + tanukiPlayer.transform.forward * distantAttackRange;
        float smaller_dist = distantAttackRange + 0.5f;

        //check for each yokai if in range and targetable;
        foreach (GameObject yokai in enemiesInRange) {
            float angle = Vector3.Angle(yokai.transform.position - tanukiPlayer.transform.position, tanukiPlayer.transform.forward);
            bool in_distant_attack_cone = angle < distandAttackConeAngle;
            if (in_distant_attack_cone) {
                float tmp_dist = Vector3.Distance(tanukiPlayer.transform.position, yokai.transform.position);
                if (tmp_dist < smaller_dist) {
                    smaller_dist = tmp_dist;
                    target = yokai.transform.position;
                }
            }
        }

        //set the target "y" coord by spawn level
        target.y = spawnLeaf.transform.position.y;
        moveLeaf.SetTargetPosition(target);

    }

    public void StopDistantAttack() {
        leafHead.SetActive(true);
    }

    public void DoInflate(bool inflate) {
        if (inflate) {
            GameObject smoke = Instantiate(smokeSpawner, gameObject.transform.position, Quaternion.identity);
            Destroy(smoke, 4);
            leafHead.SetActive(false);
            //inflateForm.transform.position = normalForm.transform.position;
            //inflateForm.transform.rotation = normalForm.transform.rotation;
            //inflateForm.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //inflateForm.SetActive(true);
            //normalForm.SetActive(false);
        }
        else {
            GameObject smoke = Instantiate(smokeSpawner, gameObject.transform.position, Quaternion.identity);
            Destroy(smoke, 4);
            leafHead.SetActive(true);
            //normalForm.transform.position = inflateForm.transform.position;
            //normalForm.SetActive(true);
            //inflateForm.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //inflateForm.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            //inflateForm.transform.rotation = Quaternion.identity;
            //inflateForm.SetActive(false);
        }
    }

    public void DoResizeTiny(bool resizeTiny) {

        if (resizeTiny) {
            GameObject smoke = Instantiate(smokeSpawner, gameObject.transform.position, Quaternion.identity);
            Destroy(smoke, 4);
            leafHead.SetActive(false);

            //gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, gameObject.transform.localScale / coefResize , Timestamp/tempMaxResize);
            gameObject.transform.localScale = gameObject.transform.localScale / coefResize;
            gameObject.GetComponent<ShadowDirectController>().ResizeShadow(true, coefResize);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraOrbit>().ResizeDistanceCamera(true, coefResize);
            //ResizeDistanceCamera
            //GameObject.FindGameObjectWithTag("Hitodama").transform.localScale = GameObject.FindGameObjectWithTag("Hitodama").transform.localScale / coefResize;
        }
        else {
            GameObject smoke = Instantiate(smokeSpawner, gameObject.transform.position, Quaternion.identity);
            Destroy(smoke, 4);
            leafHead.SetActive(true);

            //gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, gameObject.transform.localScale * coefResize , Timestamp/tempMaxResize);
            gameObject.transform.localScale = gameObject.transform.localScale * coefResize;
            gameObject.GetComponent<ShadowDirectController>().ResizeShadow(false, coefResize);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraOrbit>().ResizeDistanceCamera(false, coefResize);
            //GameObject.FindGameObjectWithTag("Hitodama").transform.localScale = GameObject.FindGameObjectWithTag("Hitodama").transform.localScale * coefResize;
        }

    }

    public void DoBeginAbsorption(GameObject absorbableObject) {
        canvasQTE.SetActive(true);
        sakePot.SetActive(true);
    }

    public Pair<Capacity, float> DoContinueAbsorption(GameObject absorbableObject, InputController input) {
        Pair<Capacity, float> pairCapacity = new Pair<Capacity, float>(Capacity.Nothing, 0);

        if (absorbableObject.CompareTag("Yokai") && absorbableObject.GetComponent<YokaiController>().GetIsKnocked() && absorptionTimer > 0) {

            centerButton.GetComponent<Image>().color = Color.white;
            absorptionTimer -= 0.03f;
            absorptionGauge -= 0.01f;
            InputParams inputParams = input.RetrieveUserRequest();
            if (inputParams.contextualButtonPressed) {
                centerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(centerButton.GetComponent<RectTransform>().sizeDelta.x + 5, centerButton.GetComponent<RectTransform>().sizeDelta.y + 5);
                centerButton.GetComponent<Image>().color = Color.grey;
                absorptionGauge += 1;
                inputParams.contextualButtonPressed = false;
                input.SetUserRequest(inputParams);
                SoundController.instance.PlaySingle(absorption);
            }

            loadingBar.GetComponent<Image>().fillAmount = absorptionTimer * 25 / 100;

            if (absorptionGauge > maxAbsorptionGauge) {
                pairCapacity = AbsorbeYokai(absorbableObject);
                DeactivateAbsorptionQTE();
                ResetAbsorptionGauge();
            }

        }
        else {
            DeactivateAbsorptionQTE();
            ResetAbsorptionGauge();
            SoundController.instance.StopSingle();
        }

        return pairCapacity;
    }

    private void ResetAbsorptionGauge() {
        absorptionGauge = 0;
        absorptionTimer = 4f;
        centerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
        centerButton.GetComponent<Image>().color = Color.white;
    }

    private void ActivateAbsorptionQTE() {
        canvasQTE.SetActive(true);
        sakePot.SetActive(true);
    }

    private void DeactivateAbsorptionQTE() {
        sakePot.SetActive(false);
        canvasQTE.SetActive(false);
    }

    private Pair<Capacity, float> AbsorbeYokai(GameObject absorbableObject) {
        RemoveYokaiInRange(absorbableObject);
        absorbableObject.GetComponent<YokaiController>().Absorbed();

        Capacity capacity = absorbableObject.GetComponent<YokaiController>().GetCapacity();
        if (capacity == Capacity.Nothing) {
            gameObject.GetComponent<PlayerCollectableController>().AddYokai();
        }
        float timerCapacity = absorbableObject.GetComponent<YokaiController>().GetTimerCapacity();
        return new Pair<Capacity, float>(capacity, timerCapacity);
    }

    public GameObject DoSpawnLure() {
        GameObject clone = null;
        if (leafHead.activeSelf && GameObject.FindGameObjectWithTag("Lure") == null) {
            leafHead.SetActive(false);
            clone = Instantiate(lure, tanukiPlayer.position, tanukiPlayer.rotation);
            clone.transform.Translate(0, 3, 2);
        }
        return clone;
    }

    public void DestroyLure(GameObject lure) {
        if (lure != null) {
            Destroy(lure);
            leafHead.SetActive(true);
        }
    }

    public void CheckExistingLure(GameObject lure) {
        if (lure == null && leafHead.activeSelf == false) {
            leafHead.SetActive(true);
        }
    }

    public void DoCarry(GameObject objectToCarry) {
        objectToCarry.transform.parent = catchSlot.transform;
        objectToCarry.transform.position = catchSlot.transform.position;
        Destroy(objectToCarry.GetComponent<Rigidbody>());
    }

    public void StopCarry(GameObject objectToCarry) {
        objectToCarry.transform.parent = null;
        Rigidbody body = objectToCarry.AddComponent(typeof(Rigidbody)) as Rigidbody;
        body.useGravity = true;
        body.mass = 100;
    }

    public void ResetLeaf() {
        leafHead.SetActive(true);
        leafHand.SetActive(false);
        sakePot.SetActive(false);
        ParachuteLeaf.SetActive(false);
    }

    public void AddYokaiInRange(GameObject yokai) {
        enemiesInRange.Add(yokai);
    }

    public void RemoveYokaiInRange(GameObject yokai) {
        enemiesInRange.Remove(yokai);
    }

    public float GetDistantAttackRange() {
        return distantAttackRange;
    }

}
