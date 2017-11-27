using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHatAttack : MonoBehaviour {

    [SerializeField]
    private GameObject hat;
    private Animator animBody;

    // Use this for initialization
    void Start () {
        animBody = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown("g"))
        {
            
            Transform hand = GameObject.FindGameObjectWithTag("Hand").transform;
            hat.transform.parent = null;
            hat.transform.parent = hand;
            hat.transform.localPosition = new Vector3(-0.162f, 0.216f, -0.4f);
            hat.transform.localRotation = Quaternion.Euler(new Vector3(-167.944f, 127.187f, -28.23599f));
            animBody.SetBool("isAttacking", true);
        }

        //animBody.SetBool("isAttacking", false);

    }
}
