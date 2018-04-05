using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingRockZ2P3 : MonoBehaviour {

    private Vector3 position_movingrock;
    private Rigidbody MovingBlock;
    private float pi = 3.14159265359f;
    private float x = 0.0f;

    private bool LureBlocking = false;
    private bool Player_Crunched = false;

    private GameObject Lure;
    private PlayerHealth KodaPlayerHP;

    // Use this for initialization
    void Start () {
        position_movingrock = transform.position;

        MovingBlock = gameObject.GetComponent<Rigidbody>();

        KodaPlayerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }
	
	// Update is called once per frame
	void Update () {
        Lure = GameObject.FindGameObjectWithTag("Lure");

        if (Lure == null) {
            LureBlocking = false;
        }

        if (Player_Crunched) {
            KodaPlayerHP.LooseHP(5.0f);
        }

        if (!Pause.Paused && !LureBlocking) {

            if (x >= 2.0f) {
                x = 0.0f;
            }

            position_movingrock.y = 3 * Mathf.Cos(x * pi) + 3;
            transform.position = position_movingrock;
            x = x + 0.002f;
        }
        
	}


    private void OnCollisionEnter(Collision col) {
        Debug.Log("OnCollisionStay" + col.collider.name);

        if (col.gameObject.CompareTag("Lure")) {
            LureBlocking = true;
        }
        if(col.gameObject.CompareTag("Player")){
            Player_Crunched = true;
        }
    }

    private void OnCollisionExit(Collision col) {
        if (col.gameObject.CompareTag("Player")) {
            Player_Crunched = false;
        }
    }
}
