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

    public void OnStateEnter(InteractState state) {
        switch (state) {

            case InteractState.Glide:
                animBody.SetBool("isGliding", true);
                break;

            case InteractState.MeleeAttack:
                animBody.SetBool("isAttacking", true);
                break;

            case InteractState.DistantAttack:
                animBody.SetBool("isDistantAttacking", true);
                break;

            case InteractState.SpawnLure:
                //animBody.SetBool("isAttacking", true);
                break;

            case InteractState.Inflate:
                //animBody.SetBool("isAttacking", true);
                break;

            case InteractState.Tiny:
                //animBody.SetBool("isAttacking", true);
                break;

            case InteractState.Activate:
                //animBody.SetBool("isAttacking", true);
                break;

            case InteractState.Absorb:
                animBody.SetBool("isAbsorbing", true);
                break;
        }
    }

    public void OnStateExit(InteractState state) {
        switch (state) {
            case InteractState.Glide:
                animBody.SetBool("isJumping", false);
                break;

            case InteractState.MeleeAttack:
                animBody.SetBool("isAttacking", false);
                break;

            case InteractState.DistantAttack:
                animBody.SetBool("isDistantAttacking", false);
                break;

            case InteractState.SpawnLure:
                //animBody.SetBool("isAttacking", false);
                break;

            case InteractState.Inflate:
                //animBody.SetBool("isAttacking", false);
                break;

            case InteractState.Tiny:
                //animBody.SetBool("isAttacking", false);
                break;

            case InteractState.Activate:
                //animBody.SetBool("isAttacking", false);
                break;

            case InteractState.Absorb:
                animBody.SetBool("isAbsorbing", false);
                break;
        }
    }

}
