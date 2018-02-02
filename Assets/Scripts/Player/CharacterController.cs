using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct InputParams {
    public bool requestJump;
    public float moveX;
    public float moveZ;

}
public class CharacterController : MonoBehaviour {

    [Header("PLAYER")]
    [Space(10)]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float airControl = 6f;
    [SerializeField] private float jumpForce = 12f;

    private List<GameObject> _grounds = new List<GameObject>();


    //Orientation Camera player
    [Header("CAMERA")]
    [Space(10)]
    public Transform pivot;
    public float rotateSpeed;
    private Vector3 orientationMove;
    public GameObject playerModel;
    private Vector3 inputVelocityAxis;

    private Rigidbody body;
    private MovementState previousState;
    private MovementState movementState;
    private MovementStateParam moveStateParameters;
    private MovementStateController moveStateCtrl;
    InputParams inputParameters;



    private bool onGround;

    //Animation
    private AnimatorController animBody;

    private void Start() {
        movementState = MovementState.Idle;
        body = GetComponent<Rigidbody>();
        animBody = GetComponent<AnimatorController>();
        moveStateParameters = new MovementStateParam();
        moveStateCtrl = new MovementStateController();
        inputParameters = new InputParams();
    }

    private void Update() {
        
        onGround = IsGrounded();
        UpdateInput();
        previousState = movementState;

        //deplacements
        ApplyInput();

        UpdateMoveStateParameters();

        movementState = moveStateCtrl.GetNewState(movementState, moveStateParameters);

        float speed = Mathf.Sqrt(Mathf.Pow(inputParameters.moveX, 2) + Mathf.Pow(inputParameters.moveZ, 2));

        if (previousState != movementState) {
            animBody.OnStateExit(previousState);
            animBody.OnStateEnter(movementState);
        }
        animBody.UpdateState(movementState, speed, moveSpeed);
        
    }

    void OnCollisionEnter(Collision coll) {
        GameObject gO = coll.gameObject;

        if (gO.layer == LayerMask.NameToLayer("Ground")) {
            ContactPoint[] contacts = coll.contacts;

            if (contacts.Length > 0) {
                foreach (ContactPoint c in contacts) {
                    // c.normal.y = 0 => Vertical
                    // c.normal.y = 0.5 => 45°
                    // c.normal.y = 1 => Horizontal
                    if (c.normal.y >= 0.5f && c.normal.y <= 1f) {
                        _grounds.Add(gO);
                        break;
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision coll) {
        if (IsGrounded()) {
            GameObject gO = coll.gameObject;

            if (_grounds.Contains(gO)) {
                _grounds.Remove(gO);
            }
        }
    }

    private bool IsGrounded() {
        return _grounds.Count > 0;
    }

    private float AbsoluteVal(float nb) {
        if (nb < 0) {
            nb *= -1;
        }
        return nb;
    }


    InputParams UpdateInput() {

        inputParameters.requestJump = Input.GetButtonDown("Jump");
        inputParameters.moveX = Input.GetAxis("Horizontal");
        inputParameters.moveZ = Input.GetAxis("Vertical");

        return inputParameters;

    }

    void ApplyInput() {
        //Debug.Log("STATE: " + movementState);
        bool canJump = !(movementState == MovementState.DoubleJump || movementState == MovementState.Fall)/* ou si maudit et pas en state jump / fall */;
        Debug.Log("Here the state:" + movementState);
        //JUMP
        if (inputParameters.requestJump && canJump) {
            body.velocity = new Vector3(0, jumpForce, 0);
        }
        
        inputVelocityAxis = new Vector3(inputParameters.moveX, body.velocity.y, inputParameters.moveZ);
        //Orientation du personnage
        orientationMove = (transform.forward * inputParameters.moveZ) + (transform.right * inputParameters.moveX);
        inputVelocityAxis = inputVelocityAxis.normalized * moveSpeed;
        inputVelocityAxis.y = body.velocity.y;


        //AIR CONTROL
        if (!onGround) {
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * new Vector3((inputParameters.moveX * airControl), inputVelocityAxis.y, (inputParameters.moveZ * (airControl * 2)));
        }
        else {
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * inputVelocityAxis;
        }

        //Move player on direction based on camera
        if (inputParameters.moveX != 0 || inputParameters.moveZ != 0) {
            transform.rotation = Quaternion.Euler(0, pivot.rotation.eulerAngles.y, 0);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(orientationMove.x, 0f, orientationMove.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void UpdateMoveStateParameters() {
        moveStateParameters.velocity = body.velocity;
        moveStateParameters.jumpRequired = inputParameters.requestJump;
        moveStateParameters.grounded = IsGrounded();
    }

    public bool GetOnGround() {return onGround;}
    public float GetJumpForce() { return jumpForce; }
    
    

}
