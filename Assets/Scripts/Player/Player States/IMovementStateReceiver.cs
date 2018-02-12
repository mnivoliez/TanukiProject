using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementStateReceiver {

	void OnStateEnter(MovementState state);
    void OnStateExit(MovementState state);
}

public interface IInteractStateReceiver {

    void Enter(InteractState state);
    void Exit(InteractState state);
}
