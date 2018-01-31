using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : State {

    bool hasJump;

    public JumpState(CharacterController character, bool hasJump) : base(character) {
        this.hasJump = hasJump; 
    }

    public override void Tick() {

        //DOUBLEJUMP
        // A MODIFIER
        if (!character.GetOnGround() &&  Input.GetButtonDown("Jump") && !hasJump) {
            character.GetBody().velocity = new Vector3(0, character.GetJumpForce(), 0);
            character.SetState(new JumpState(character, true));

        }

        //FALL
        if (!character.GetOnGround() && character.GetBody().velocity.y < 0) {

            character.SetState(new FallState(character));

        }

        //IDLE
        if (character.GetOnGround()) {

            character.SetState(new IdleState(character));

        }

    }

    public override void OnStateEnter() {
        //Play anim
        if (hasJump) {
            character.GetAnimbody().SetBool("isDoubleJumping", true); //Change for trigger later
        }
        else {
            character.GetAnimbody().SetBool("isJumping", true); //Change for trigger later
        }
    }

    public override void OnStateExit() {
        if (hasJump) {
            character.GetAnimbody().SetBool("isDoubleJumping", false); //Change for trigger later
            character.GetAnimbody().SetBool("isJumping", false); //Change for trigger later
        }
    }

}
