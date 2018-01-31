using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : State {

    public FallState(CharacterController character) : base(character) {

    }

    public override void Tick() {

        //IDLE
        if (character.GetOnGround()) {

            character.SetState(new IdleState(character));

        }

    }

    public override void OnStateEnter() {
        //Play anim
        //Change for trigger
        character.GetAnimbody().SetBool("isDoubleJumping", false);
        character.GetAnimbody().SetBool("isJumping", false);
    }

    public override void OnStateExit() {

    }

}
