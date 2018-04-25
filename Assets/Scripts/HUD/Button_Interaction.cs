using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//================================================
//SOUNDCONTROLER
//================================================

public class Button_Interaction : MonoBehaviour {

    public void OnPointerEnter(Animator animator) {
        animator.SetTrigger("Highlighted");
        SoundController.instance.SelectHUD("PauseNavigate");
    }

    public void OnPointerExit(Animator animator) {
        animator.SetTrigger("Normal");
    }
}
