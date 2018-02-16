using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InteractState {
    Nothing, Glide, MeleeAttack, DistantAttack, SpawnLure, Inflate, Tiny, Activate, Absorb, Carry, Push
}

public struct InteractStateParam {
	public bool canGlide;
	public bool canMeleeAttack;
	public bool finishedMeleeAttack;
	public bool canDistantAttack;
	public bool finishedDistantAttack;
    public bool canSpawnLure;
    public bool canDestroyLure;
    public bool canInflate;
    public bool canResize;
    public bool canActivate;
    public bool canAbsorb;
    public bool yokaiStillInRange;
    public bool canCarry;
    public bool finishedCarry;
    public bool canPush;
}

public class InteractStateController {

    public InteractState GetNewState(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;
        switch (previous) {

            case InteractState.Nothing:
                newState = ManageNothing(previous, param);
                break;

            case InteractState.Glide:
                newState = ManageGlide(previous, param);
                break;

            case InteractState.MeleeAttack:
                newState = ManageMeleeAttack(previous, param);
                break;

            case InteractState.DistantAttack:
                newState = ManageDistantAttack(previous, param);
                break;

            case InteractState.SpawnLure:
                newState = ManageSpawnLure(previous, param);
                break;

            case InteractState.Inflate:
                newState = ManageInflate(previous, param);
                break;

            case InteractState.Tiny:
                newState = ManageResize(previous, param);
                break;

            case InteractState.Activate:
                newState = ManageActivate(previous, param);
                break;

            case InteractState.Absorb:
                newState = ManageAbsorb(previous, param);
                break;

            case InteractState.Carry:
                newState = ManageCarry(previous, param);
                break;

            case InteractState.Push:
                newState = ManagePush(previous, param);
                break;

        }


        return newState;
    }

    InteractState ManageNothing(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;

        //Glide
		if (param.canGlide) {
            newState = InteractState.Glide;
        }

        //MeleeAttack
		if (param.canMeleeAttack) {
            newState = InteractState.MeleeAttack;
        }

        //DistantAttack
        if (param.canDistantAttack) {
            newState = InteractState.DistantAttack;
        }

        //SpawnLure
        if (param.canSpawnLure) {
            newState = InteractState.SpawnLure;
        }

        //Inflate
        if (param.canInflate) {
            newState = InteractState.Inflate;
        }

        //Resize
        if (param.canResize) {
            newState = InteractState.Tiny;
        }

        //Activate
        if (param.canActivate) {
            newState = InteractState.Activate;
        }

        //Absorb
        if (param.canAbsorb) {
            newState = InteractState.Absorb;
        }

        //Carry
        if (param.canCarry) {
            newState = InteractState.Carry;
        }

        //Push
        if (param.canPush) {
            newState = InteractState.Push;
        }

        //// STILL MOVING UP
        //if (IsGoingUp(param)) {
        //    if (param.jumpRequired) {
        //        newState = InteractState.DoubleJump;
        //    }
        //    else {
        //        newState = InteractState.Jump;
        //    }

        //}

        return newState;
    }

    InteractState ManageGlide(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;

        if (param.canGlide) {
            newState = InteractState.Glide;
        }

        //NOTHING
        if (!param.canGlide) {
            newState = InteractState.Nothing;
        }
        return newState;

    }

    InteractState ManageMeleeAttack(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;

        //NOTHING
		if (param.finishedMeleeAttack) {
            newState = InteractState.Nothing;
        }
        return newState;
    }

    InteractState ManageDistantAttack(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;

        //NOTHING
		if (param.finishedDistantAttack) {
			param.finishedDistantAttack = false;
            newState = InteractState.Nothing;
        }
        return newState;
    }

    InteractState ManageSpawnLure(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;
        //NOTHING
        newState = InteractState.Nothing;
        return newState;
    }

    InteractState ManageInflate(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;
        //NOTHING
        if (param.canInflate) {
            newState = InteractState.Nothing;
        }
        return newState;
    }

    InteractState ManageResize(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;
        //NOTHING
        if (!param.canGlide) {
            newState = InteractState.Nothing;
        }
        return newState;
    }

    InteractState ManageActivate(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;
        //NOTHING
        if (!param.canGlide) {
            newState = InteractState.Nothing;
        }
        return newState;
    }
    InteractState ManageAbsorb(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;
        //NOTHING
        if (!param.yokaiStillInRange) {
            newState = InteractState.Nothing;
        }
        return newState;
    }
    InteractState ManageCarry(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;
        //NOTHING
        if (param.finishedCarry) {
            newState = InteractState.Nothing;
        }
        return newState;
    }

    InteractState ManagePush(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;
        //RUN
        //NOTHING
        if (!param.canGlide) {
            newState = InteractState.Nothing;
        }
        return newState;
    }


    //bool IsIdle(InteractStateParam param) {
    //    return param.grounded && ((Mathf.Abs(param.velocity.x) + Mathf.Abs(param.velocity.z)) < 0.1f);
    //}

    //bool IsRunning(InteractStateParam param) {
    //    return param.grounded && ((Mathf.Abs(param.velocity.x) + Mathf.Abs(param.velocity.z)) >= 0.1f);
    //}

    //bool IsGoingUp(InteractStateParam param) {
    //    return !param.grounded && param.velocity.y > 0;
    //}

    //bool IsFalling(InteractStateParam param) {
    //    return !param.grounded && param.velocity.y < 0;
    //}

}
