using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHatAttack : MonoBehaviour {

    [SerializeField]
    private GameObject hatHead;
    [SerializeField]
    private GameObject hatHand;
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

        localPositionHead = new Vector3(0.00551694f, 0.4424813f, 0.0006193146f);
        localRotationHead = Quaternion.Euler(new Vector3(-2.453f, 0.283f, -0.234f));

        localPositionHand = new Vector3(-0.162f, 0.216f, -0.4f);
        localRotationHand = Quaternion.Euler(new Vector3(-167.944f, 127.187f, -28.23599f));

        hatHead.transform.localPosition = localPositionHead;
        hatHead.transform.localRotation = localRotationHead;

        hatHand.transform.localPosition = localPositionHand;
        hatHand.transform.localRotation = localRotationHand;
        hatHand.SetActive(false);

        hatInHand = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("g") && !hatInHand)
        {
            hatHead.SetActive(false);
            hatHand.SetActive(true);
            hatInHand = true;
            animBody.SetBool("isAttacking", true);
        }

        if (Input.GetKeyDown("h") && hatInHand)
        {
            hatHead.SetActive(true);
            hatHand.SetActive(false);
            hatInHand = false;
            animBody.SetBool("isAttacking", false);
        }
    }
}
