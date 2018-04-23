using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTutoEnemi : MonoBehaviour {

    [SerializeField] private GameObject tutoParchment;
    YokaiController yokaiStat;
    bool isOnScreen = false;

	void Start () {
        yokaiStat = gameObject.GetComponent<YokaiController>();

    }
	
	// Update is called once per frame
	void Update () {
        if (!isOnScreen) {

            if (yokaiStat.GetIsKnocked()) {
                isOnScreen = true;
                tutoParchment.GetComponent<Tuto_ParchmentBehavior>().ActiveParchment();
            }
        }

    }
}
