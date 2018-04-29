using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
//================================================
//SOUNDCONTROLER
//================================================

public class InteractBehavior : MonoBehaviour {

    [Header("GLIDE")]
    [Space(8)]
    [SerializeField]
    private SkinnedMeshRenderer ParachuteLeaf;

    [Header("MELEE ATTACK")]
    [Space(8)]
    [SerializeField]
    private GameObject leafHead;
    [SerializeField] private GameObject leafHand;
    [SerializeField] private GameObject attackRange;
    [SerializeField] private float meleeDamage;

    [Header("DISTANT ATTACK")]
    [Space(8)]
    [SerializeField] private GameObject tempLeafHead; //delete when leaf animation is done
    [SerializeField] private GameObject leafPrefab;
    [SerializeField] private GameObject spawnLeaf;
    [SerializeField] private float distantAttackRange = 8f;
    [SerializeField] private float distandAttackConeAngle = 20f;
    private HashSet<GameObject> enemiesInRange;
    [SerializeField] private float distantDamage;

    [Header("INFLATE")]
    [Space(8)]
    [SerializeField] private GameObject normalForm;
    [SerializeField] private GameObject inflateForm;
    [SerializeField] private GameObject smokeSpawner;

    [Header("RESIZE")]
    [Space(8)]
    [SerializeField]
    private float coefResize = 4;


    [Header("ABSORB")]
    [Space(8)]
    private bool absorbing;
    //[SerializeField] private float absorptionTimer = 4f;
    [SerializeField] private GameObject sakePot;
    [SerializeField] private GameObject sakeJnt;

    private bool pressedAbsorbingOnce = false;

    [Header("CARRY")]
    [Space(8)]
    [SerializeField][Range(2000, 10000)]
    private int throwCoef = 5000;

    [Header("LURE")]
    [Space(8)]
    [SerializeField]
    private GameObject lure;
    [SerializeField]
    private Transform tanukiPlayer;
    [SerializeField][Range(0,3)]
    private float lureSpawnHeight = 1f;

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
		catchSlot = GameObject.FindGameObjectWithTag ("Hand");
        absorbing = false;
    }

    // Update is called once per frame
    void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        if (absorbing) {
            //absorptionTimer -= Time.deltaTime;
            //loadingBar.GetComponent<Image>().fillAmount = absorptionTimer * 25 / 100;
            //absorptionGauge -= 0.01f;
            //if (absorptionTimer < 0) StopAbsorption();
        }
    }

    public void DoGlide() {
        //leafHead.SetActive(false);
        //ParachuteLeaf.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(DeployLeaf());
    }

    IEnumerator DeployLeaf() {
        float lerpBlend;
        while (ParachuteLeaf.GetBlendShapeWeight(0) < 99) {
            lerpBlend = Mathf.Lerp(ParachuteLeaf.GetBlendShapeWeight(0), 100, 0.3f);
            ParachuteLeaf.SetBlendShapeWeight(0, lerpBlend);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void StopGlide() {
        StopAllCoroutines();

        StartCoroutine(FoldUpLeaf());
        //ParachuteLeaf.SetActive(false);
        //leafHead.SetActive(true);
    }

    IEnumerator FoldUpLeaf() {
        float lerpBlend;
        while (ParachuteLeaf.GetBlendShapeWeight(0) > 1) {
            lerpBlend = Mathf.Lerp(ParachuteLeaf.GetBlendShapeWeight(0), 0, 0.5f);
            ParachuteLeaf.SetBlendShapeWeight(0, lerpBlend);
            yield return new WaitForSeconds(0.03f);
        }
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
        tempLeafHead.SetActive(false);

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
        tempLeafHead.SetActive(true);
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
            //gameObject.GetComponent<ShadowDirectController>().ResizeShadow(true, coefResize);
            //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraOrbit>().ResizeDistanceCamera(true, coefResize);
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
        List<Material> mats = new List<Material>();
        foreach (Transform tr in absorbableObject.GetComponent<YokaiController>().GameobjectMesh) {
            if(tr.GetComponent<SkinnedMeshRenderer>() != null)
                mats.AddRange(tr.GetComponent<SkinnedMeshRenderer>().materials);
            else
                mats.AddRange(tr.GetComponent<MeshRenderer>().materials);
        }
        foreach (Material mat in mats) {
            mat.SetVector("_TargetPos", sakeJnt.transform.position - absorbableObject.transform.position);
            mat.SetFloat("_AbsoptionPercent", 0);
        }
        ActivateAbsorptionQTE();
        absorbing = true;
    }

    private float prevAbsorptionPercent = 0;
    IEnumerator LerpAbsorptionPercent(List<Material> mats) {
        while(prevAbsorptionPercent != absorptionGauge) {
            if(mats == null) break;
            prevAbsorptionPercent = Mathf.Lerp(prevAbsorptionPercent, (float)absorptionGauge/maxAbsorptionGauge, .05f);
            foreach (Material mat in mats) {
                   mat.SetFloat("_AbsorptionPercent", prevAbsorptionPercent);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public Pair<Capacity, float> DoContinueAbsorption(GameObject absorbableObject, InputController input) {
        Pair<Capacity, float> pairCapacity = new Pair<Capacity, float>(Capacity.Nothing, 0);
        InputParams inputParams = input.RetrieveUserRequest();
        List<Material> mats = new List<Material>();
        foreach (Transform tr in absorbableObject.GetComponent<YokaiController>().GameobjectMesh) {
            if(tr.GetComponent<SkinnedMeshRenderer>() != null)
                mats.AddRange(tr.GetComponent<SkinnedMeshRenderer>().materials);
            else
                mats.AddRange(tr.GetComponent<MeshRenderer>().materials);
        }
        foreach (Material mat in mats) {
            mat.SetVector("_TargetPos", sakeJnt.transform.position - absorbableObject.transform.position);
        }
        if (absorbableObject.CompareTag("Yokai") && absorbableObject.GetComponent<YokaiController>().GetIsKnocked() && absorbing) {

            centerButton.GetComponent<Image>().color = Color.white;
            if (inputParams.contextualButtonPressed) {
                //centerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(centerButton.GetComponent<RectTransform>().sizeDelta.x + 5, centerButton.GetComponent<RectTransform>().sizeDelta.y + 5);
                //centerButton.GetComponent<Image>().color = Color.grey;
                absorptionGauge += 1;
                StartCoroutine(LerpAbsorptionPercent(mats));
                inputParams.contextualButtonPressed = false;
                if (!pressedAbsorbingOnce) {
                    //================================================
                    pressedAbsorbingOnce = true;
                    SoundController.instance.SelectKODA("Absorption");
                    //================================================
                }
            }

            if (absorptionGauge > maxAbsorptionGauge) {
                pairCapacity = AbsorbeYokai(absorbableObject);
                DeactivateAbsorptionQTE();
                ResetAbsorptionGauge();
                //================================================
                pressedAbsorbingOnce = false;
                SoundController.instance.StopKoda();
                //================================================
            }
        }
        else {
            //centerButton.GetComponent<Image>().color = Color.white;
            DoBeginAbsorption(absorbableObject);
        }
        input.SetUserRequest(inputParams);
        return pairCapacity;
    }

    public void StopAbsorption() {
        absorbing = false;
        ResetAbsorptionGauge();
        DeactivateAbsorptionQTE();
        //================================================
        pressedAbsorbingOnce = false;
        SoundController.instance.StopKoda();
        //================================================
    }
    private void ResetAbsorptionGauge() {
        absorptionGauge = 0;
        //absorptionTimer = 4f;
        //centerButton.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, 50f);
        //centerButton.GetComponent<Image>().color = Color.white;
    }

    private void ActivateAbsorptionQTE() {
        canvasQTE.SetActive(true);
    }

    private void DeactivateAbsorptionQTE() {
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
            tempLeafHead.SetActive(false);
            //leafHead.SetActive(false);
            Vector3 spawnLurePosition = tanukiPlayer.position + new Vector3(0, lureSpawnHeight, 0) + (tanukiPlayer.forward * 2);
            GameObject smokeSpawn = Instantiate(smokeSpawner, spawnLurePosition, Quaternion.identity);
            smokeSpawn.transform.localScale = Vector3.one * 0.3f;
            Destroy(smokeSpawn, 2f);
            clone = Instantiate(lure, spawnLurePosition, tanukiPlayer.rotation);
            //clone.transform.Translate(0, 3, 2);
        }
        return clone;
    }

    public void DestroyLure(GameObject lure) {
        if (lure != null) {
            GameObject smokeSpawn = Instantiate(smokeSpawner, lure.transform.position, Quaternion.identity);
            smokeSpawn.transform.localScale = Vector3.one * 0.3f;
            Destroy(smokeSpawn, 2f);
            lure.GetComponent<LureController>().DestroyLure();
            tempLeafHead.SetActive(true);
        }
    }

    public void CheckExistingLure(GameObject lure) {
        if (lure == null && leafHead.activeSelf == false) {
            tempLeafHead.SetActive(true);
            leafHead.SetActive(true);
        }
    }

    public void DoCarry(GameObject objectToCarry) {
        objectToCarry.transform.parent = catchSlot.transform;
        objectToCarry.transform.position = catchSlot.transform.position;
        Destroy(objectToCarry.GetComponent<Rigidbody>());
    }

    public void StopCarry(GameObject objectToCarry, Vector3 inputVelocityAxis) {
        objectToCarry.transform.parent = null;
        Rigidbody body = objectToCarry.AddComponent(typeof(Rigidbody)) as Rigidbody;
        body.useGravity = true;
        body.mass = 100;
        body.transform.position += inputVelocityAxis * 0.1f;
        float x = inputVelocityAxis.x;
        float z = inputVelocityAxis.z;
        float y = (float)Math.Sqrt(x*x + z*z) / 2f;
        Vector3 force = new Vector3(x, y, z) * throwCoef;
        //Debug.Log(force);
        body.AddForce(force);
    }

    public void ResetLeaf() {
        tempLeafHead.SetActive(true);
        //leafHead.SetActive(true);
        leafHand.SetActive(false);
        ParachuteLeaf.SetBlendShapeWeight(0, 0);
        //sakePot.SetActive(false);
        //ParachuteLeaf.SetActive(false);
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
