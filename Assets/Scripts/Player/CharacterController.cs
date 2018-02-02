using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct InputMoveParams {
    public bool requestJump;
    public float moveX;
    public float moveZ;

}

struct InputInteractParams {
    public bool GlideButton;
    public bool MeleeAttackButton;
    public bool DistantAttackButton;
    public bool SpawnLureButton;
    public bool TransformationButton;
    public bool ResizeButton;
    public bool ActivateButton;
    public bool AbsorbButton;
    public bool CarryButton;
    public bool PushButton;

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
    InputMoveParams inputMoveParameters;


    InputInteractParams inputInteractParameters;



    private bool onGround;

    //Animation
    private AnimatorController animBody;

    private void Start() {
        movementState = MovementState.Idle;
        body = GetComponent<Rigidbody>();
        animBody = GetComponent<AnimatorController>();
        moveStateParameters = new MovementStateParam();
        moveStateCtrl = new MovementStateController();
        inputMoveParameters = new InputMoveParams();
        inputInteractParameters = new InputInteractParams();
    }

    private void Update() {
        
        onGround = IsGrounded();
        UpdateMoveInput();
        UpdateInteractInput();
        previousState = movementState;

        //deplacements
        ApplyInput();

        UpdateMoveStateParameters();
        UpdateInteractStateParameters();

        movementState = moveStateCtrl.GetNewState(movementState, moveStateParameters);

        float speed = Mathf.Sqrt(Mathf.Pow(inputMoveParameters.moveX, 2) + Mathf.Pow(inputMoveParameters.moveZ, 2));

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


    InputMoveParams UpdateMoveInput() {

        inputMoveParameters.requestJump = Input.GetButtonDown("Jump");
        inputMoveParameters.moveX = Input.GetAxis("Horizontal");
        inputMoveParameters.moveZ = Input.GetAxis("Vertical");

        return inputMoveParameters;

    }

    InputInteractParams UpdateInteractInput() {

        inputInteractParameters.GlideButton = Input.GetButton("Jump");
        inputInteractParameters.MeleeAttackButton = Input.GetButton("Fire1");
        inputInteractParameters.DistantAttackButton = Input.GetButton("Fire2");
        inputInteractParameters.SpawnLureButton = Input.GetButton("Lure");
        inputInteractParameters.TransformationButton = Input.GetButton("Transformation");
        inputInteractParameters.ResizeButton = Input.GetButton("Resize");
        inputInteractParameters.ActivateButton = Input.GetButton("Fire3");
        inputInteractParameters.AbsorbButton = Input.GetButton("Fire3");
        inputInteractParameters.CarryButton = Input.GetButton("Fire3");
        //inputInteractParameters.PushButton = Input.GetButton("Fire3");

        return inputInteractParameters;

    }

    void ApplyInput() {
        //Debug.Log("STATE: " + movementState);
        bool canJump = !(movementState == MovementState.DoubleJump || movementState == MovementState.Fall)/* ou si maudit et pas en state jump / fall */;
        Debug.Log("Here the state:" + movementState);
        //JUMP
        if (inputMoveParameters.requestJump && canJump) {
            body.velocity = new Vector3(0, jumpForce, 0);
        }
        
        inputVelocityAxis = new Vector3(inputMoveParameters.moveX, body.velocity.y, inputMoveParameters.moveZ);
        //Orientation du personnage
        orientationMove = (transform.forward * inputMoveParameters.moveZ) + (transform.right * inputMoveParameters.moveX);
        inputVelocityAxis = inputVelocityAxis.normalized * moveSpeed;
        inputVelocityAxis.y = body.velocity.y;


        //AIR CONTROL
        if (!onGround) {
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * new Vector3((inputMoveParameters.moveX * airControl), inputVelocityAxis.y, (inputMoveParameters.moveZ * (airControl * 2)));
        }
        else {
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * inputVelocityAxis;
        }

        //Move player on direction based on camera
        if (inputMoveParameters.moveX != 0 || inputMoveParameters.moveZ != 0) {
            transform.rotation = Quaternion.Euler(0, pivot.rotation.eulerAngles.y, 0);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(orientationMove.x, 0f, orientationMove.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void UpdateMoveStateParameters() {
        moveStateParameters.velocity = body.velocity;
        moveStateParameters.jumpRequired = inputMoveParameters.requestJump;
        moveStateParameters.grounded = IsGrounded();
    }

    void UpdateInteractStateParameters() {
        //A TERMINER !
    }


    public bool GetOnGround() {return onGround;}
    public float GetJumpForce() { return jumpForce; }
    
    

}
