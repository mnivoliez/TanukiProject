using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingRockZ2P3Killing : MonoBehaviour {
    private Vector3 position_movingrock;
    private Vector3 position_movingrock_origin;
    private Rigidbody MovingBlock;
    private float pi = 3.14159265359f;
    private float x = 0.0f;

    private bool LureBlocking = false;
    private bool Player_Crunched = false;

    private GameObject Lure;
    private PlayerHealth KodaPlayerHP;

    // Use this for initialization
    void Start() {
        position_movingrock = transform.position;
        position_movingrock_origin = transform.position;

        MovingBlock = gameObject.GetComponent<Rigidbody>();

        KodaPlayerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    // Update is called once per frame
    void Update() {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        Lure = GameObject.FindGameObjectWithTag("Lure");

        if (Lure == null) {
            LureBlocking = false;
        }

        if (!LureBlocking && Player_Crunched) {
            KodaPlayerHP.LooseHP(5.0f);
        }

        if (!Pause.Paused && !LureBlocking) {

            if (x >= 2.0f) {
                x = 0.0f;
            }

            position_movingrock.y = position_movingrock_origin.y / 2.0f * Mathf.Cos(x * pi) + position_movingrock_origin.y / 2.0f;
            transform.position = position_movingrock;
            x = x + 0.003f;
        }

    }


    private void OnCollisionEnter(Collision col) {

        if (col.gameObject.CompareTag("Lure")) {
            LureBlocking = true;
            Debug.Log("LEURRE TOUCHE !");
        }
        if (col.gameObject.CompareTag("Player")) {
            Player_Crunched = true;
        }
    }

    private void OnCollisionExit(Collision col) {
        if (col.gameObject.CompareTag("Player")) {
            Player_Crunched = false;
        }
    }
}
