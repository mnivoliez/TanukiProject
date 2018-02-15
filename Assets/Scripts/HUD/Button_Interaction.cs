using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Interaction : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnPointerEnter(Animator animator) {
        animator.SetTrigger("Highlighted");
    }

    public void OnPointerExit(Animator animator) {
        animator.SetTrigger("Normal");
    }
}
