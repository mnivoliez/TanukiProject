using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeaf : MonoBehaviour {

    private float initialSpeed = 15f;
    private float currentSpeed;
    private float rotationSpeed = 50f;
    private float damage;
    private GameObject spawnLeaf = null;
    private Vector3 targetPosition = Vector3.zero;
    private bool arrived = false;
    [SerializeField] private GameObject disparitionEffect;
    [SerializeField] private AudioClip throwLeaf;
    [SerializeField] private AudioClip vanishLeaf;
    private Renderer renderLeaf;
    private Renderer renderStem;

    void Start() {
        currentSpeed = initialSpeed;
        //renderLeaf = transform.GetChild(0).GetComponent<Renderer>();
        //renderStem = GetComponent<Renderer>();
    }

    void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================

        if (!arrived) {
            MoveTo();
            if (transform.position == targetPosition) {
                arrived = true;
                //StartCoroutine(LeafDisappear());
            }
        }
        else {
            BackTo();
        }
    }

    public void MoveTo() {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.left, rotationSpeed);
    }

    public void BackTo() {

        GameObject FXDisappear = Instantiate(disparitionEffect, transform.position, Quaternion.identity);
        FXDisappear.transform.localScale = FXDisappear.transform.localScale / 10f;
        SoundController.instance.PlayLeafSingle(vanishLeaf);
        Destroy(FXDisappear, 1f);
        Destroy(gameObject);
    }

    IEnumerator LeafDisappear() {
        float disappearValue = 0;
        float dissolveValue = -0.25f;

        while (disappearValue < 0.8f) {
            //Debug.Log("DISAPPEAR: " + disappearValue);
            disappearValue = Mathf.Lerp(disappearValue, 1, 0.3f);
            dissolveValue = Mathf.Lerp(dissolveValue, 0.25f, 0.5f);
            renderLeaf.material.SetFloat("_DisLineWidth", dissolveValue);
            renderLeaf.material.SetFloat("_DisAmount", disappearValue);
            renderStem.material.SetFloat("_DisAmount", disappearValue);            
            yield return new WaitForSeconds(0.000001f);
        }
        GameObject FXDisappear = Instantiate(disparitionEffect, transform.position, Quaternion.identity);
        SoundController.instance.PlayLeafSingle(vanishLeaf);
        arrived = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<KodaController>().StopDistantAttackState();
        Destroy(FXDisappear, 1f);
        FXDisappear.transform.localScale = FXDisappear.transform.localScale / 10f;
        Destroy(gameObject);

    }

    public void SetSpawnPosition(GameObject spPos) {
        SoundController.instance.PlayLeafSingle(throwLeaf);
        spawnLeaf = spPos;
    }

    public void SetTargetPosition(Vector3 pos) {
        targetPosition = pos;
    }

    public void SetDamage(float dmg) {
        damage = dmg;
    }

    public float GetDamage() {
        return damage;
    }

    void OnTriggerEnter(Collider collid) {

        //if (collid.gameObject.CompareTag("Yokai") && !collid.gameObject.GetComponent<YokaiController>().GetIsKnocked()) {

        //    collid.gameObject.GetComponent<YokaiController>().BeingHit();
        //    collid.gameObject.GetComponent<YokaiController>().LooseHp(damage);

        //    targetPosition = collid.gameObject.transform.position;
        //}

    }

    void OnTriggerExit(Collider collid) {

        //if (collid.gameObject.CompareTag("Yokai") && !collid.gameObject.GetComponent<YokaiController>().GetIsKnocked()) {
        //    collid.gameObject.GetComponent<YokaiController>().EndHit();
        //}

    }
}
