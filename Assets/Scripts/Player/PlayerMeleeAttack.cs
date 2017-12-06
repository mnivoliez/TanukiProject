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


    void Start () {
        animBody = GetComponent<Animator>();
        initialTransform = transform;
        
        leafHand.SetActive(false);

        leafInHand = false;
    }

    // Update is called once per frame
    void Update(){

        if (timerAttack > 0.5)
        {
            animBody.SetBool("isChargingAttack", true);
            leafHead.SetActive(false);
            leafHand.SetActive(true);
            leafInHand = true;
        }

        if (Input.GetButtonDown("Fire1") && !leafInHand)
        {
            timerAttack = 0;
        }

        if (Input.GetButton("Fire1") && !leafInHand)
        {
            timerAttack += Time.deltaTime;
            Debug.Log(timerAttack);
        }

        if (Input.GetButtonUp("Fire1") && !leafInHand){
            // Basic attack
            if (timerAttack < 0.5)
            {
                animBody.SetBool("isAttacking", true);
                leafHead.SetActive(false);
                leafHand.SetActive(true);
                leafInHand = true;
            }
            // Charged attack
            else
            {
                //animBody.SetBool("isChargedAttack", true);
            }
        }        

        if (this.animBody.GetCurrentAnimatorStateInfo(0).IsName("PostAttack") && leafInHand) {
            leafHead.SetActive(true);
            leafHand.SetActive(false);
            leafInHand = false;
            animBody.SetBool("isAttacking", false);
        }
        
    }
}
