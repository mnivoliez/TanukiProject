using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State {

    public IdleState(CharacterController character) : base(character){

    }

    public override void Tick(){

        //RUN
        if (character.GetOnGround() && (Mathf.Abs(character.GetBody().velocity.x) + Mathf.Abs(character.GetBody().velocity.z)) >= 0.1f) {
            character.SetState(new RunState(character));
        }



        //JUMP
        if (!character.GetOnGround() && character.GetBody().velocity.y > 0) {

            character.SetState(new JumpState(character, false));

        }


        //FALL
        if (!character.GetOnGround() && character.GetBody().velocity.y < 0) {

            character.SetState(new FallState(character));

        }

    }

    public override void OnStateEnter(){
        //Play anim
        
    }

    public override void OnStateExit(){

    }

}
