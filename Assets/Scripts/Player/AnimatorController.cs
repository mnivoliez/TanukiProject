using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController :  MonoBehaviour, /*IInterractState,*/ IMovementStateReceiver {

    //Animation
    private Animator animBody;

    // Use this for initialization
    void Start () {
        animBody = GetComponent<Animator>();
    }

    public void UpdateState(MovementState state, float speed, float speedMultyplier) {

        animBody.SetFloat("Speed", speed);
        animBody.SetFloat("SpeedMultiplier", speedMultyplier);

        //currentState = state;
        //switch (state) {


        //    case MovementState.Jump:
                    
                
        //        break;

        //    case MovementState.DoubleJump:
                
        //        break;
        //}
        
    }

    public void OnStateEnter(MovementState state) {
        switch(state) {

            case MovementState.Jump:
                    animBody.SetBool("isJumping", true); //Change for trigger later
                break;
            case MovementState.DoubleJump:
                    animBody.SetBool("isDoubleJumping", true); //Change for trigger later
                break;
        }
    }

    public void OnStateExit(MovementState state) {
        switch (state) {
            case MovementState.Jump:
                    animBody.SetBool("isJumping", false); //Change for trigger later
                break;

            case MovementState.DoubleJump:
                    animBody.SetBool("isDoubleJumping", false); //Change for trigger later
                break;
        }
    }

}
