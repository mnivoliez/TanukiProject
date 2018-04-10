using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Interaction : MonoBehaviour {

    public void OnPointerEnter(Animator animator) {
        animator.SetTrigger("Highlighted");
    }

    public void OnPointerExit(Animator animator) {
        animator.SetTrigger("Normal");
    }
}
