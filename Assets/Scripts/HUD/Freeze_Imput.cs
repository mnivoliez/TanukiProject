using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze_Imput : MonoBehaviour {

    [SerializeField] private bool isfreeze;
    private InputController playerInput;

	void Start () {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>();
        
	}
	

	void Update () {
        playerInput.SetFreezeInput(isfreeze);
    }
}
