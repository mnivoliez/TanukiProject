using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitodamaController : MonoBehaviour {

    private GameObject player;
    private bool reachTop = false;
    private float chronoMove = 0;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating("ChangeSens", 2f, 5f);
    }
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(player.transform.position, Vector3.down, 100 * Time.deltaTime);

        if (!reachTop) {
            transform.Translate(Vector3.up * Time.deltaTime* Random.Range(0.1f, 0.3f));
        }
        else {
            transform.Translate(Vector3.down * Time.deltaTime * Random.Range(0.1f, 0.3f));
        }
        
    }

    void ChangeSens() {
        reachTop = !reachTop;
    }
}
