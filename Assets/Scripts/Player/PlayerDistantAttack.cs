using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDistantAttack : MonoBehaviour {

    [SerializeField]
    private GameObject leafHead;
    [SerializeField]
    private GameObject leafHand;
    [SerializeField]
    private GameObject leafPrefab;
    [SerializeField]
    private GameObject spawnLeaf;
    [SerializeField]
    private GameObject rangeMaxLeaf;
    private Transform initialTransform;
    private bool leafInHand;
    private bool leafIsBack;

    private Animator animBody;

    void Start() {
        animBody = GetComponent<Animator>();
        initialTransform = transform;
        leafHand.SetActive(false);
        leafIsBack = true;
        leafInHand = false;
    }

    void Update() {

        if (Input.GetButtonDown("Fire2") && !leafInHand) {
            leafIsBack = false;
            animBody.SetBool("isDistantAttacking", true);
            leafHead.SetActive(false);
            leafInHand = true;
            GameObject LeafBoomerang = Instantiate(leafPrefab, spawnLeaf.transform.position, leafPrefab.transform.rotation);
            LeafBoomerang.GetComponent<MoveLeaf>().setSpawnPosition(spawnLeaf);
            LeafBoomerang.GetComponent<MoveLeaf>().setTargetPosition(rangeMaxLeaf);

        }

        if (this.animBody.GetCurrentAnimatorStateInfo(0).IsName("DistantAttack") && leafInHand) {
            animBody.SetBool("isDistantAttacking", false);
        }

        if (leafIsBack && leafInHand) {
            leafHead.SetActive(true);
            leafInHand = false;
        }
    }

    public void SetLeafIsBack() {
        leafIsBack = true;
    }
}
