using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum InteractState {
    Nothing, Glide, MeleeAttack, DistantAttack, SpawnLure, Transformation, Activate, Absorb, Carry, Push
}

public struct InteractStateParam {
    public bool grounded;
    public bool GlideButton;
    public bool MeleeAttackButton;
    public bool DistantAttackButton;
    public bool SpawnLureButton;
    public bool TransformationButton;
    public bool ActivateButton;
    public bool AbsorbButton;
    public bool CarryButton;
    public bool PushButton;
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

            case InteractState.Transformation:
                newState = ManageTransformation(previous, param);
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

    InteractState ManageNothing(InteractState previous, InteractState param) {
        InteractState newState = previous;
        //IDLE
        if (IsIdle(param)) {
            newState = InteractState.Idle;
        }

        // STILL MOVING UP
        if (IsGoingUp(param)) {
            if (param.jumpRequired) {
                newState = InteractState.DoubleJump;
            }
            else {
                newState = InteractState.Jump;
            }

        }

        return newState;
    }

    InteractState ManageGlide(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;
        //IDLE
        if (IsIdle(param)) {
            newState = InteractState.Idle;
        }

        return newState;

    }

    InteractState MeleeAttack(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;
        //IDLE
        if (IsIdle(param)) {
            newState = InteractState.Idle;
        }

        return newState;
    }

    InteractState DistantAttack(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;

        //FAll
        if (IsFalling(param)) {
            newState = InteractState.Fall;
        }
        return newState;
    }

    InteractState SpawnLure(InteractState previous, InteractStateParam param) {
        InteractState newState = previous;
        //RUN
        if (IsRunning(param)) {
            newState = InteractState.Run;
        }


        return newState;
    }

    bool IsIdle(InteractStateParam param) {
        return param.grounded && ((Mathf.Abs(param.velocity.x) + Mathf.Abs(param.velocity.z)) < 0.1f);
    }

    bool IsRunning(InteractStateParam param) {
        return param.grounded && ((Mathf.Abs(param.velocity.x) + Mathf.Abs(param.velocity.z)) >= 0.1f);
    }

    bool IsGoingUp(InteractStateParam param) {
        return !param.grounded && param.velocity.y > 0;
    }

    bool IsFalling(InteractStateParam param) {
        return !param.grounded && param.velocity.y < 0;
    }

}
