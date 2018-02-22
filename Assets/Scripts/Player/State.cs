using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{

    protected KodaController character;

    public abstract void Tick();

    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }

    public State(KodaController character){
        this.character = character;
    }
}
