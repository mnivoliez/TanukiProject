using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHatAttack : MonoBehaviour {

    [SerializeField]
    private GameObject hat;
    private Transform hand;
    private Transform head;
    private Transform initialTransform;
    private bool hatInHand;
    private Vector3 localPositionHead;
    private Quaternion localRotationHead;
    private Vector3 localPositionHand;
    private Quaternion localRotationHand;
    private Animator animBody;

    // Use this for initialization
    void Start () {
        animBody = GetComponent<Animator>();
    // Use this for initialization
        initialTransform = transform;
        head = GameObject.FindGameObjectWithTag("Head").transform;
        hand = GameObject.FindGameObjectWithTag("Hand").transform;

        localPositionHead = new Vector3(0.00551694f, 0.4424813f, 0.0006193146f);
        localRotationHead = Quaternion.Euler(new Vector3(-2.453f, 0.283f, -0.234f));

        localPositionHand = new Vector3(-0.162f, 0.216f, -0.4f);
        localRotationHand = Quaternion.Euler(new Vector3(-167.944f, 127.187f, -28.23599f));

        hat.transform.localPosition = localPositionHead;
        hat.transform.localRotation = localRotationHead;

        hatInHand = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("g") && !hatInHand)
        {
            
            hat.transform.parent = null;
            hat.transform.parent = hand;
            hat.transform.localPosition = localPositionHand;
            hat.transform.localRotation = localRotationHand;
            hatInHand = true;
            animBody.SetBool("isAttacking", true);
        }

        if (Input.GetKeyDown("h") && hatInHand)
        {
            hat.transform.parent = null;
            hat.transform.parent = head;
            hat.transform.localPosition = localPositionHead;
            hat.transform.localRotation = localRotationHead;
            hatInHand = false;
        }
    }
}
