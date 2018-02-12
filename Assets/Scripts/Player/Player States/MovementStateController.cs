using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementState {
    Idle, Run, Jump, DoubleJump, Fall
}

public struct MovementStateParam {
	public Vector3 velocity;
	public Vector3 position;
	public Vector3 position_before_fall;
    public bool jumpRequired;
    public bool grounded;
}

public class MovementStateController {

    public MovementState GetNewState(MovementState previous, MovementStateParam param) {
		//Debug.Log ("Calling me is: " + new System.Diagnostics.StackFrame(1, true).GetMethod().Name);
		MovementState newState = previous;
        switch (previous) {
			case MovementState.Idle:
				//Debug.Log ("GetNewState mvt=Idle");
                newState = ManageIdle(previous, param);
                break;

			case MovementState.Run:
				//Debug.Log ("GetNewState mvt=Run");
                newState = ManageRun(previous, param);
                break;

			case MovementState.Jump:
				//Debug.Log ("GetNewState mvt=Jump");
                newState = ManageJump(previous, param);
                break;

			case MovementState.DoubleJump:
				//Debug.Log ("GetNewState mvt=DoubleJump");
                newState = ManageDoubleJump(previous, param);
                break;

			case MovementState.Fall:
				//Debug.Log ("GetNewState mvt=Fall");
                newState = ManageFall(previous, param);
                break;
			default:
				//Debug.Log ("GetNewState mvt=Other");
				break;
        }

        return newState;
    }

    MovementState ManageJump(MovementState previous, MovementStateParam param) {
		MovementState newState = previous;
		//Debug.Log ("Jump manage");
        //IDLE
        if (IsIdle(param)) {
            newState = MovementState.Idle;
        }

        // STILL MOVING UP
        if (IsGoingUp(param)) {
			//Debug.Log ("jump inside=" + param.jumpRequired);
			if (param.jumpRequired) {
				//Debug.Log ("Jump DoubleJump");
                newState = MovementState.DoubleJump;
            } /*else {
				Debug.Log ("Jump Jump");
                newState = MovementState.Jump;
            }*/

        }

        //FAll
        if (IsFalling(param)) {
            newState = MovementState.Fall;
        }
        return newState;
    }

    MovementState ManageFall(MovementState previous, MovementStateParam param) {
		MovementState newState = previous;
		//Debug.Log ("Fall manage");
        //IDLE
		if (IsIdle(param)) {
			//Debug.Log ("fall idle");
            newState = MovementState.Idle;
        }

		//RUN
		if (IsRunning(param)) {
			//Debug.Log ("fall run");
            newState = MovementState.Run;
        }
        return newState;

    }

    MovementState ManageDoubleJump(MovementState previous, MovementStateParam param) {
		MovementState newState = previous;
		//Debug.Log ("DoubleJump manage");
        //IDLE
		if (IsIdle(param)) {
			//Debug.Log ("DoubleJump idle");
            newState = MovementState.Idle;
        }

        //FAll
		if (IsFalling(param)) {
			//Debug.Log ("DoubleJump fall");
            newState = MovementState.Fall;
        }

		/*if (IsGoingUp(param)) {
			Debug.Log ("DoubleJump DoubleJump");
            newState = MovementState.DoubleJump;
        }*/
        return newState;
    }

    MovementState ManageRun(MovementState previous, MovementStateParam param) {
		MovementState newState = previous;
		//Debug.Log ("Run manage");
        //IDLE
		if (IsIdle(param)) {
			//Debug.Log ("Run idle");
            newState = MovementState.Idle;
        }

        //JUMP
		if (IsGoingUp(param)) {
			//Debug.Log ("Run jump");
            newState = MovementState.Jump;
        }

        //FAll
		if (IsFalling(param)) {
			//Debug.Log ("Run fall");
            newState = MovementState.Fall;
        }
        return newState;
    }

    MovementState ManageIdle(MovementState previous, MovementStateParam param) {
		MovementState newState = previous;
		//Debug.Log ("Idle manage");
        //RUN
		if (IsRunning(param)) {
			//Debug.Log ("Idle run");
            newState = MovementState.Run;
        }

        //JUMP
		if (IsGoingUp(param)) {
			//Debug.Log ("Idle jump");
            newState = MovementState.Jump;
        }

        //FAll
		if (IsFalling(param)) {
			//Debug.Log ("Idle fall");
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

	float step_velocity = 0.01f;
	float step_position = 0.5f;

	bool IsGoingUp(MovementStateParam param) {
		bool up = 
			!param.grounded && 
			param.velocity.y > step_velocity && 
			(Mathf.Abs(param.position_before_fall.y - param.position.y) > step_position || param.jumpRequired);

		return up;
	}

	bool IsFalling(MovementStateParam param) {
		bool fall = 
			!param.grounded && 
			param.velocity.y < -step_velocity &&
			(Mathf.Abs(param.position_before_fall.y - param.position.y) > step_position || param.jumpRequired);

		return fall;
    }

}
