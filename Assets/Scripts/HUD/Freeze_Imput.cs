using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze_Imput : MonoBehaviour {

    [SerializeField] private bool isfreeze;

	void Start () {
        GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>().SetFreezeInput(isfreeze);
	}
	

	void Update () {
		
	}
}
