using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeAttack : MonoBehaviour {

    [SerializeField]
    private GameObject leafHead;
    [SerializeField]
    private GameObject leafHand;
    private Transform initialTransform;
    private bool leafInHand;
    private Animator animBody;
    private float timerAttack;
    [SerializeField]
    private GameObject attackRange;
    [SerializeField]
    private float meleeDamage;
    private bool leafIsBack;


    void Start() {
        animBody = GetComponent<Animator>();
        initialTransform = transform;
        attackRange.GetComponent<MeleeAttackTrigger>().SetDamage(meleeDamage);
        leafHand.SetActive(false);

        leafInHand = false;

        leafIsBack = GetComponent<PlayerDistantAttack>().GetLeafIsBack();
    }

    // Update is called once per frame
    void Update() {

        leafIsBack = GetComponent<PlayerDistantAttack>().GetLeafIsBack();
        //Debug.Log("Leaf is back: " + leafIsBack);

        if (timerAttack > 0.3) {
            animBody.SetBool("isChargingAttack", true);
            leafHead.SetActive(false);
            leafHand.SetActive(true);
            leafInHand = true;
        }

        if (Input.GetButtonDown("Fire1") && !leafInHand && leafIsBack) {
            timerAttack = 0;
        }

        if (Input.GetButton("Fire1") && !leafInHand && leafIsBack) {
            timerAttack += Time.deltaTime;

        }

        if (Input.GetButtonUp("Fire1") && leafIsBack) {
            // Basic attack
            if (timerAttack < 0.3 && !leafInHand) {
                attackRange.SetActive(true);
                animBody.SetBool("isAttacking", true);
                leafHead.SetActive(false);
                leafHand.SetActive(true);
                leafInHand = true;
            }
            // Charged attack
            else if (timerAttack >= 0.3) {
                attackRange.SetActive(true);
                animBody.SetBool("isChargedAttack", true);
            }

            timerAttack = 0;
        }

        if (this.animBody.GetCurrentAnimatorStateInfo(0).IsName("PostAttack") && leafInHand) {
            attackRange.SetActive(false);
            leafHead.SetActive(true);
            leafHand.SetActive(false);
            leafInHand = false;
            animBody.SetBool("isAttacking", false);
            animBody.SetBool("isChargedAttack", false);
            animBody.SetBool("isChargingAttack", false);
        }

    }
}
