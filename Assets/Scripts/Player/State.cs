using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{

    protected CharacterController character;

    public abstract void Tick();

    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }

    public State(CharacterController character){
        this.character = character;
    }
}
