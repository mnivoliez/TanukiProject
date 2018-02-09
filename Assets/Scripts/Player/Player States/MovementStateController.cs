using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementState {
    Idle, Run, Jump, DoubleJump, Fall
}

public struct MovementStateParam {
	public Vector3 velocity;
	public Vector3 velocity_previous;
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
			Debug.Log ("fall idle");
            newState = MovementState.Idle;
        }

		if (IsRunning(param)) {
			Debug.Log ("fall run");
            newState = MovementState.Run;
        }
        return newState;

    }

    MovementState ManageDoubleJump(MovementState previous, MovementStateParam param) {
        MovementState newState = previous;
        //IDLE
		if (IsIdle(param)) {
			Debug.Log ("DoubleJump idle");
            newState = MovementState.Idle;
        }

        //FAll
		if (IsFalling(param)) {
			Debug.Log ("DoubleJump fall");
            newState = MovementState.Fall;
        }

		if (IsGoingUp(param)) {
			Debug.Log ("DoubleJump DoubleJump");
            newState = MovementState.DoubleJump;
        }
        return newState;
    }

    MovementState ManageRun(MovementState previous, MovementStateParam param) {
        MovementState newState = previous;
        //IDLE
		if (IsIdle(param)) {
			Debug.Log ("Run idle");
            newState = MovementState.Idle;
        }

        //JUMP
		if (IsGoingUp(param)) {
			Debug.Log ("Run jump");
            newState = MovementState.Jump;
        }

        //FAll
		if (IsFalling(param)) {
			Debug.Log ("Run fall");
            newState = MovementState.Fall;
        }
        return newState;
    }

    MovementState ManageIdle(MovementState previous, MovementStateParam param) {
        MovementState newState = previous;
        //RUN
		if (IsRunning(param)) {
			Debug.Log ("Idle run");
            newState = MovementState.Run;
        }

        //JUMP
		if (IsGoingUp(param)) {
			Debug.Log ("Idle jump");
            newState = MovementState.Jump;
        }

        //FAll
		if (IsFalling(param)) {
			Debug.Log ("Idle fall");
			newState = MovementState.Fall;
        }

        return newState;
    }

    bool IsIdle(MovementStateParam param) {
		return param.grounded && ((Mathf.Abs(param.velocity.x) + Mathf.Abs(param.velocity.z) + Mathf.Abs(param.velocity.y)) < 0.01f);
    }

    bool IsRunning(MovementStateParam param) {
        return param.grounded && ((Mathf.Abs(param.velocity.x) + Mathf.Abs(param.velocity.z)) >= 0.1f);
    }

	float hysteresis_step = 5f;

    bool IsGoingUp(MovementStateParam param) {
		bool fall = !param.grounded && param.velocity.y > hysteresis_step;

		return fall;
	}

	bool IsFalling(MovementStateParam param) {
		bool fall = !param.grounded && (param.velocity.y < -hysteresis_step);

		return fall;
    }

}
