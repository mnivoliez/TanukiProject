using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour, /*IInterractState,*/ IMovementStateReceiver {

    //Animation
    private Animator animBody;

    private float smoothVertSpeed;

    void Start() {
        animBody = GetComponent<Animator>();
    }

    public void UpdateState(MovementState state, float speed, float lateralBend, float verticalSpeed) {

        animBody.SetFloat("Speed", speed);

        animBody.SetFloat("LateralBend", lateralBend);

        smoothVertSpeed = Mathf.Lerp(smoothVertSpeed, verticalSpeed, .3f);
        animBody.SetFloat("VerticalSpeed", smoothVertSpeed);

        //if(Input.GetKeyDown(KeyCode.F)) animBody.SetTrigger("InstantAttack");
        //if(Input.GetKeyDown(KeyCode.G)) animBody.SetTrigger("DistantAttack");
        //if(Input.GetKeyDown(KeyCode.H)) animBody.SetBool("IsAbsorbing", !animBody.GetBool("IsAbsorbing"));
        //if(Input.GetKeyDown(KeyCode.J)) animBody.SetTrigger("Hit");
        //if(Input.GetKeyDown(KeyCode.K)) animBody.SetTrigger("Death");

        //currentState = state;
        //switch (state) {


        //    case MovementState.Jump:


        //        break;

        //    case MovementState.DoubleJump:

        //        break;
        //}
        int layer = 1 << 10 + 1 << 8;
    }

    public void OnStateEnter(MovementState state) {
        switch (state) {
            case MovementState.Jump:
                animBody.SetBool("isInAir", true);
                break;
            case MovementState.PushUp:
                animBody.SetBool("isInAir", true);
                break;
            case MovementState.Fall:
                animBody.SetBool("isInAir", true);
                break;
            case MovementState.DoubleJump:
                animBody.SetTrigger("DoubleJump");
                break;
            case MovementState.Idle:
                animBody.SetBool("isInAir", false);
                break;
            case MovementState.Run:
                animBody.SetBool("isInAir", false);
                break;
        }
    }

    public void OnStateExit(MovementState state) {
        switch (state) {
            case MovementState.Jump:
                break;

            case MovementState.DoubleJump:
                break;
        }
    }

    public void OnStateEnter(InteractState state) {
        switch (state) {

            case InteractState.Glide:
                animBody.SetBool("isGliding", true);
                break;

            case InteractState.MeleeAttack:
                animBody.SetTrigger("InstantAttack");
                //animBody.SetLayerWeight (1, 1);
                break;

            case InteractState.DistantAttack:
                animBody.SetTrigger("DistantAttack");
                //animBody.SetLayerWeight (1, 1);
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
                animBody.SetBool("IsAbsorbing", true);
                //animBody.SetLayerWeight(1, 1);
                break;

            case InteractState.Nothing:
                break;
        }
    }

    public void OnStateExit(InteractState state) {
        switch (state) {
            case InteractState.Glide:
                animBody.SetBool("isGliding", false);
                break;

            case InteractState.MeleeAttack:
                //animBody.SetBool("isAttacking", false);
                break;

            case InteractState.DistantAttack:
                //animBody.SetBool("isDistantAttacking", false);
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
                animBody.SetBool("IsAbsorbing", false);
                break;
        }
    }

}
