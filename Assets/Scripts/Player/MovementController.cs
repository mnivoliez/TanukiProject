using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementState
{
    Idle, Run, PreJump, Jump, PreDoubleJump, DoubleJump, Fall
}
public class MovementController : MonoBehaviour
{
    private MovementState _state;
    private Rigidbody _body;

    private float _moveSpeed = 10f;
    private float _speed = 10f;
    [SerializeField] private float _airControl = 6f;
    [SerializeField] private float _jumpForce = 120f;
    private const float HYSTERESIS_STEP = 0.01f;

    private float _moveX;
    private float _moveZ;

    // Use this for initialization
    void Start()
    {
        _state = MovementState.Idle;
        _moveX = 0f;
        _moveZ = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocityAxis = new Vector3(_moveX, _body.velocity.y, _moveZ);
        //Orientation du personnage
        Vector3 orientationMove = (transform.forward * _moveZ) + (transform.right * _moveX);

        switch (_state)
        {
            case MovementState.Idle:
                break;
            case MovementState.Run:
                break;
            case MovementState.PreJump:
                _body.velocity = new Vector3(_body.velocity.x, 0.02f, _body.velocity.z);
                _body.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
                break;
            case MovementState.Jump:
                break;
            case MovementState.PreDoubleJump:
                break;
            case MovementState.DoubleJump:
                break;
            case MovementState.Fall:
                break;
        }
    }

    public void Step(InputParams input, EnvironementParams env)
    {
        _moveX = input.moveX;
        _moveZ = input.moveZ;

        switch (_state)
        {
            case MovementState.Idle:
                UpdateIdleState(input, env);
                break;
            case MovementState.Run:
                UpdateRunState(input, env);
                break;
            case MovementState.PreJump:
                // should be manage by this controller, will do nothing, at least for now
                break;
            case MovementState.Jump:
                UpdateJumpState(input, env);
                break;
            case MovementState.PreDoubleJump:
                // should be manage by this controller, will do nothing, at least for now
                break;
            case MovementState.DoubleJump:
                UpdateDoubleJumpState(input, env);
                break;
            case MovementState.Fall:
                UpdateFallState(input, env);
                break;
        }
    }

    private void UpdateIdleState(InputParams input, EnvironementParams env)
    {
        if (env.isGrounded && input.jumpRequest)
        {
            _state = MovementState.PreJump;
        }
    }
    private void UpdateRunState(InputParams input, EnvironementParams env)
    {
        if (env.isGrounded && input.jumpRequest)
        {
            _state = MovementState.PreJump;
        }
    }

    private void UpdatePreJumpState()
    {

    }
    private void UpdateJumpState(InputParams input, EnvironementParams env)
    {

    }
    private void UpdatePreDoubleJumpState()
    {

    }

    private void UpdateDoubleJumpState(InputParams input, EnvironementParams env)
    {

    }
    private void UpdateFallState(InputParams input, EnvironementParams env)
    {

    }

    public bool IsGoingUp(EnvironementParams env)
    {
        bool up = !env.isGrounded && _body.velocity.y > HYSTERESIS_STEP;

        return up;
    }

    public bool IsFalling(EnvironementParams env)
    {
        bool fall = !env.isGrounded && (_body.velocity.y < -HYSTERESIS_STEP);

        return fall;
    }
}
