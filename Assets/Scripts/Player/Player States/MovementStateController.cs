using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementState {
    Idle, Run, Jump, DoubleJump, Fall, PushUp
}

public struct MovementStateParam {
	public Vector3 velocity;
	public Vector3 position;
	public Vector3 position_before_fall;
    public bool jumpRequired;
    public bool inAirStream;
	public bool grounded;
	public float moveX;
	public float moveZ;
}

public class MovementStateController {

	private bool debugEnabled = false;

	public void DebugLogLocal(string message) {
		if (debugEnabled)
		{
			Debug.Log (message);
		}
	}

    public MovementState GetNewState(MovementState previous, MovementStateParam param) {
		//DebugLogLocal ("Calling me is: " + new System.Diagnostics.StackFrame(1, true).GetMethod().Name);
		MovementState newState = previous;
        switch (previous) {
			case MovementState.Idle:
				//DebugLogLocal ("GetNewState mvt=Idle");
                newState = ManageIdle(previous, param);
                break;

			case MovementState.Run:
				//DebugLogLocal ("GetNewState mvt=Run");
                newState = ManageRun(previous, param);
                break;

			case MovementState.Jump:
				//DebugLogLocal ("GetNewState mvt=Jump");
                newState = ManageJump(previous, param);
                break;

			case MovementState.DoubleJump:
				//DebugLogLocal ("GetNewState mvt=DoubleJump");
                newState = ManageDoubleJump(previous, param);
                break;

			case MovementState.Fall:
				//DebugLogLocal ("GetNewState mvt=Fall");
                newState = ManageFall(previous, param);
                break;

			case MovementState.PushUp:
				//DebugLogLocal ("GetNewState mvt=Fall");
                newState = ManagePushUp(previous, param);
                break;
			default:
				//DebugLogLocal ("GetNewState mvt=Other");
				break;
        }

        return newState;
    }

    MovementState ManageJump(MovementState previous, MovementStateParam param) {
		MovementState newState = previous;
		//DebugLogLocal ("Jump manage");
        //IDLE
        if (IsIdle(param)) {
            newState = MovementState.Idle;
        }

        // STILL MOVING UP
		if (param.jumpRequired) {
			//DebugLogLocal ("jump inside=" + param.jumpRequired);
			if (param.jumpRequired) {
				DebugLogLocal ("Jump DoubleJump");
                newState = MovementState.DoubleJump;
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
		DebugLogLocal ("Fall manage");
        //IDLE
		if (IsIdle(param)) {
			DebugLogLocal ("fall idle");
            newState = MovementState.Idle;
        }

		//RUN
		if (IsRunning(param)) {
			DebugLogLocal ("fall run");
            newState = MovementState.Run;
        }
        
        //DOUBLEJUMP
        if (IsGoingUp(param) && !param.inAirStream) {
            DebugLogLocal ("fall run");
            newState = MovementState.DoubleJump;
        }

        //PUSHUP
        if (IsGoingUp(param) && param.inAirStream) {
            DebugLogLocal ("fall run");
            newState = MovementState.PushUp;
        }

        return newState;

    }

    MovementState ManagePushUp(MovementState previous, MovementStateParam param) {
		MovementState newState = previous;
		//DebugLogLocal ("Fall manage");
        //IDLE
		if (IsIdle(param)) {
			DebugLogLocal ("fall idle");
            newState = MovementState.Idle;
        }

		//RUN
		if (IsRunning(param)) {
			DebugLogLocal ("fall run");
            newState = MovementState.Run;
        }

        //FALL
        if (IsFalling(param)) {
            DebugLogLocal ("fall run");
            newState = MovementState.Fall;
        }

        return newState;

    }

    MovementState ManageDoubleJump(MovementState previous, MovementStateParam param) {
		MovementState newState = previous;
		//DebugLogLocal ("DoubleJump manage");
        //IDLE
		if (IsIdle(param)) {
			DebugLogLocal ("DoubleJump idle");
            newState = MovementState.Idle;
        }

        //FAll
		if (IsFalling(param)) {
			DebugLogLocal ("DoubleJump fall");
            newState = MovementState.Fall;
        }
        return newState;
    }

    MovementState ManageRun(MovementState previous, MovementStateParam param) {
		MovementState newState = previous;
		//DebugLogLocal ("Run manage");
        //IDLE
		if (IsIdle(param)) {
			DebugLogLocal ("Run idle");
            newState = MovementState.Idle;
        }

        //JUMP
		if (param.jumpRequired) {
			DebugLogLocal ("Run jump");
            newState = MovementState.Jump;
        }

        //FAll
		if (IsFalling(param)) {
			DebugLogLocal ("Run fall");
            newState = MovementState.Fall;
        }
        return newState;
    }

    MovementState ManageIdle(MovementState previous, MovementStateParam param) {
		MovementState newState = previous;
		//DebugLogLocal ("Idle manage");
        //RUN
		if (IsRunning(param)) {
			DebugLogLocal ("Idle run");
            newState = MovementState.Run;
        }

        //JUMP
		if (param.jumpRequired) {
			DebugLogLocal ("Idle jump");
            newState = MovementState.Jump;
        }

        //FAll
		if (IsFalling(param)) {
			DebugLogLocal ("Idle fall");
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
