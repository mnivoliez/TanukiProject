using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementState {
    Idle, Run, Jump, DoubleJump, Fall,
}

public struct MovementStateParam {
    public Vector3 velocity;
    public bool jumpRequired;
    public bool grounded;
}

public class MovementStateController {

    public MovementState GetNewState(MovementState previous, MovementStateParam param) {
        MovementState newState = previous;
        switch (previous) {
            case MovementState.Idle:
                newState = ManageIdle(previous, param);
                break;
            case MovementState.Run:
                newState = ManageRun(previous, param);
                break;

            case MovementState.Jump:
                newState = ManageJump(previous, param);
                break;


            case MovementState.DoubleJump:
                newState = ManageDoubleJump(previous, param);
                break;


            case MovementState.Fall:
                newState = ManageFall(previous, param);
                break;

        }

        return newState;
    }

    MovementState ManageJump(MovementState previous, MovementStateParam param) {
        MovementState newState = previous;
        //IDLE
        if (IsIdle(param)) {
            newState = MovementState.Idle;
        }

        // STILL MOVING UP
        if (IsGoingUp(param)) {
            if (param.jumpRequired) {
                newState = MovementState.DoubleJump;
            }
            else {
                newState = MovementState.Jump;
            }

        }

        //FAll
        if (IsFalling(param)) {
            newState = MovementState.Fall;
        }
        return newState;
    }

    MovementState ManageFall(MovementState previous, MovementStateParam param) {
        MovementState newState = previous;
        //IDLE
        if (IsIdle(param)) {
            newState = MovementState.Idle;
        }

        if (IsRunning(param)) {
            newState = MovementState.Run;
        }
        return newState;

    }

    MovementState ManageDoubleJump(MovementState previous, MovementStateParam param) {
        MovementState newState = previous;
        //IDLE
        if (IsIdle(param)) {
            newState = MovementState.Idle;
        }

        //FAll
        if (IsFalling(param)) {
            newState = MovementState.Fall;
        }

        if (IsGoingUp(param)) {
            newState = MovementState.DoubleJump;
        }
        return newState;
    }

    MovementState ManageRun(MovementState previous, MovementStateParam param) {
        MovementState newState = previous;
        //IDLE
        if (IsIdle(param)) {
            newState = MovementState.Idle;
        }

        //JUMP
        if (IsGoingUp(param)) {
            newState = MovementState.Jump;
        }

        //FAll
        if (IsFalling(param)) {
            newState = MovementState.Fall;
        }
        return newState;
    }

    MovementState ManageIdle(MovementState previous, MovementStateParam param) {
        MovementState newState = previous;
        //RUN
        if (IsRunning(param)) {
            newState = MovementState.Run;
        }

        //JUMP
        if (IsGoingUp(param)) {
            newState = MovementState.Jump;
        }

        //FAll
        if (IsFalling(param)) {
            newState = MovementState.Fall;
        }

        return newState;
    }

    bool IsIdle(MovementStateParam param) {
        return param.grounded && ((Mathf.Abs(param.velocity.x) + Mathf.Abs(param.velocity.z)) < 0.1f);
    }

    bool IsRunning(MovementStateParam param) {
        return param.grounded && ((Mathf.Abs(param.velocity.x) + Mathf.Abs(param.velocity.z)) >= 0.1f);
    }

    bool IsGoingUp(MovementStateParam param) {
        return !param.grounded && param.velocity.y > 0;
    }

    bool IsFalling(MovementStateParam param) {
        return !param.grounded && param.velocity.y < 0;
    }

}
