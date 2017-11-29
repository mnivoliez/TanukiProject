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
    //private Vector3 localPositionHead;
    //private Quaternion localRotationHead;
    //private Vector3 localPositionHand;
    //private Quaternion localRotationHand;
    private Animator animBody;


    void Start () {
        animBody = GetComponent<Animator>();
        initialTransform = transform;

        //localPositionHead = new Vector3(0.00551694f, 0.4424813f, 0.0006193146f);
        //localRotationHead = Quaternion.Euler(new Vector3(-2.453f, 0.283f, -0.234f));

        //localPositionHand = new Vector3(-0.162f, 0.216f, -0.4f);
        //localRotationHand = Quaternion.Euler(new Vector3(-167.944f, 127.187f, -28.23599f));

        //LeafHead.transform.localPosition = localPositionHead;
        //LeafHead.transform.localRotation = localRotationHead;

        //LeafHand.transform.localPosition = localPositionHand;
        //LeafHand.transform.localRotation = localRotationHand;
        leafHand.SetActive(false);

        leafInHand = false;
    }

    // Update is called once per frame
    void Update(){

        if (Input.GetButtonDown("Fire1") && !leafInHand){
            
            animBody.SetBool("isAttacking", true);
            leafHead.SetActive(false);
            leafHand.SetActive(true);
            leafInHand = true;
        }

        if (this.animBody.GetCurrentAnimatorStateInfo(0).IsName("PostAttack") && leafInHand) {
            leafHead.SetActive(true);
            leafHand.SetActive(false);
            leafInHand = false;
            animBody.SetBool("isAttacking", false);
        }
        
    }
}
