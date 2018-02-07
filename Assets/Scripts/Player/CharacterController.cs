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
    public bool InflateButton;
    public bool ResizeButton;
    public bool ActivateButton;
    public bool AbsorbButton;
    public bool CarryButton;
    public bool PushButton;
    public bool LeafAvailable;
    public bool activableObject;
    public bool absorbableObject;
    public bool portableObject;
}

public class LeafAlreadyTakenException : System.Exception {

}
public class Leaf {
    public class LeafLock {
        Leaf _parent;
        public LeafLock(Leaf parent) {
            _parent = parent;
        }
        public void Released() {
            _parent.leafLock = null;
            _parent = null;
        }
    }

    private LeafLock leafLock;

    public LeafLock TakeLeaf() {
        if (leafLock == null) {
            leafLock = new LeafLock(this);
            return leafLock;
        }
        else {
            throw new LeafAlreadyTakenException();
        }
    }

}
public class CharacterController : MonoBehaviour {

    [Header("PLAYER")]
    [Space(10)]
    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField] private float airControl = 6f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private GameObject absorbRange;

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
    private MovementState previousMovementState;
    private MovementState movementState;
    private MovementStateParam moveStateParameters;
    private MovementStateController moveStateCtrl;

    private InteractState previousInteractState;
    private InteractState interactState;
    private InteractStateParam interactStateParameter;
    private InteractStateController InteractStateCtrl;

    private bool onGround;

    private InputController inputController;

    // Capacity
    [SerializeField] private bool hasDoubleJumpCapacity;

    //Animation
    private AnimatorController animBody;
    private InteractBehavior interactBehaviorCtrl;

    private float timerAttack;

    private void Start() {
        movementState = MovementState.Idle;
        previousMovementState = movementState;
        interactState = InteractState.Nothing;
        previousInteractState = interactState;
        body = GetComponent<Rigidbody>();
        animBody = GetComponent<AnimatorController>();
        moveStateParameters = new MovementStateParam();
        moveStateCtrl = new MovementStateController();
        interactStateParameter = new InteractStateParam();
        InteractStateCtrl = new InteractStateController();
        inputController = GetComponent<InputController>();
        interactBehaviorCtrl = GetComponent<InteractBehavior>();
    }

    private void Update() {
        onGround = IsGrounded();
        previousMovementState = movementState;
        
        InputParams inputParams = inputController.RetrieveUserRequest();
        //deplacements
        MoveAccordingToInput(inputParams);
        InteractAccordingToInput(inputParams);
        previousInteractState = interactState;
        UpdateMoveStateParameters(inputParams);
        UpdateInteractStateParameters(inputParams);

        movementState = moveStateCtrl.GetNewState(movementState, moveStateParameters);
        interactState = InteractStateCtrl.GetNewState(interactState, interactStateParameter);

        float speed = Mathf.Sqrt(Mathf.Pow(inputParams.moveX, 2) + Mathf.Pow(inputParams.moveZ, 2));

        if (previousMovementState != movementState) {
            animBody.OnStateExit(previousMovementState);
            animBody.OnStateEnter(movementState);
        }

        if (previousInteractState != interactState) {
            animBody.OnStateExit(previousInteractState);
            animBody.OnStateEnter(interactState);
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

    void MoveAccordingToInput(InputParams inputParams) {
        //Debug.Log("STATE: " + movementState);
        bool canJump = !(movementState == MovementState.DoubleJump || movementState == MovementState.Fall || movementState == MovementState.Jump && !hasDoubleJumpCapacity)/* ou si maudit et pas en state jump / fall */;
        //Debug.Log("Here the state:" + movementState);
        //JUMP
        if (inputParams.jumpRequest && canJump) {
            body.velocity = new Vector3(0, jumpForce, 0);
        }

        inputVelocityAxis = new Vector3(inputParams.moveX, body.velocity.y, inputParams.moveZ);
        //Orientation du personnage
        orientationMove = (transform.forward * inputParams.moveZ) + (transform.right * inputParams.moveX);
        inputVelocityAxis = inputVelocityAxis.normalized * moveSpeed;
        inputVelocityAxis.y = body.velocity.y;


        //AIR CONTROL
        if (!onGround) {
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * new Vector3((inputParams.moveX * airControl), inputVelocityAxis.y, (inputParams.moveZ * (airControl * 2)));
        }
        else {
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * inputVelocityAxis;
        }

        //Move player on direction based on camera
        if (inputParams.moveX != 0 || inputParams.moveZ != 0) {
            transform.rotation = Quaternion.Euler(0, pivot.rotation.eulerAngles.y, 0);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(orientationMove.x, 0f, orientationMove.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void InteractAccordingToInput(InputParams inputParams) {

       

        switch (interactState) {

            case InteractState.Nothing:


                if (previousInteractState == InteractState.MeleeAttack) {
                    interactBehaviorCtrl.StopMeleeAttack();
                }

                if (previousInteractState == InteractState.DistantAttack) {
                    interactBehaviorCtrl.StopDistantAttack();
                }

                if (previousInteractState == InteractState.Glide) {
                    interactBehaviorCtrl.StopGlide();
                }

                if (previousInteractState == InteractState.Inflate) {
                    interactBehaviorCtrl.DoInflate(false);
                }
                break;

            case InteractState.Glide:
                body.velocity = new Vector3(body.velocity.x, -5f, body.velocity.z);
                interactBehaviorCtrl.DoGlide();
                
                break;

            case InteractState.MeleeAttack:


                if (interactStateParameter.canMeleeAttack) {
                    interactBehaviorCtrl.DoMeleeAttack();
                    
                }
                
                //if (Input.GetButtonDown("Fire1")) {
                //    timerAttack = 0;
                //}

                //if (Input.GetButton("Fire1")) {
                //    timerAttack += Time.deltaTime;

                //}

                //if (Input.GetButtonUp("Fire1")) {
                //    Debug.Log("Coucou");
                //    if (timerAttack < 0.3) {
                //        interactBehaviorCtrl.DoMeleeAttack();
                //    }
                //    else if (timerAttack >= 0.3) {
                //        interactBehaviorCtrl.DoChargedMeleeAttack();
                //    }
                //}



                break;

            
            case InteractState.DistantAttack:

                if (interactStateParameter.canDistantAttack) {
                    interactBehaviorCtrl.DoDistantAttack();
                }
                

                break;

            case InteractState.SpawnLure:
                break;

            case InteractState.Inflate:
                
                if (previousInteractState != InteractState.Inflate) {
                    Debug.Log("coucou");
                    interactBehaviorCtrl.DoInflate(true);
                }
                
                
                break;

            case InteractState.Tiny:
                break;

            case InteractState.Activate:
                break;

            case InteractState.Absorb:

                GameObject nearestObject = absorbRange.GetComponent<DetectNearInteractObject>().GetNearestObject();

                if(nearestObject == null || !nearestObject.CompareTag("Yokai")) {
                    interactStateParameter.yokaiStillInRange = false;
                }
                


                break;

            case InteractState.Carry:
                break;

            case InteractState.Push:
                break;
        }


    }


    void UpdateMoveStateParameters(InputParams inputParams) {
        moveStateParameters.velocity = body.velocity;
        moveStateParameters.jumpRequired = inputParams.jumpRequest;
        moveStateParameters.grounded = IsGrounded();
    }

    void UpdateInteractStateParameters(InputParams inputParams) {

        switch (inputParams.actionRequest) {
            case ActionRequest.Glide:
                if (movementState == MovementState.Fall) {
                    interactStateParameter.canGlide = true;
                }
                break;

            case ActionRequest.MeleeAttack:
                interactStateParameter.canMeleeAttack = true;
                break;

            case ActionRequest.DistantAttack:
                interactStateParameter.canDistantAttack = true;
                break;

            case ActionRequest.SpawnLure:
                interactStateParameter.canSpawnLure = true;
                break;

            case ActionRequest.Inflate:
                
                if (IsGrounded()) interactStateParameter.canInflate = true;
                break;

            case ActionRequest.Resize:
                if (IsGrounded()) interactStateParameter.canResize = true;
                break;

            case ActionRequest.ContextualAction:

                GameObject nearestObject = absorbRange.GetComponent<DetectNearInteractObject>().GetNearestObject();

                if(nearestObject != null) {

                    bool inFrontOfActivableObject = false;
                    bool inFrontOfAbsorbableObject = false;
                    bool inFrontOfPortableObject = false;

                    if (nearestObject.CompareTag("Yokai")) {
                        inFrontOfAbsorbableObject = true;
                        interactStateParameter.yokaiStillInRange = true;

                    }
                    else if (nearestObject.gameObject.layer == LayerMask.NameToLayer("Catchable")) {
                        inFrontOfPortableObject = true;
                    }
                    else if (nearestObject.gameObject.layer == LayerMask.NameToLayer("Activable")) {
                        inFrontOfActivableObject = true;
                    }

                    interactStateParameter.canActivate = false;
                    interactStateParameter.canAbsorb = false;
                    interactStateParameter.canCarry = false;

                    if (IsGrounded()) {
                        if (inFrontOfActivableObject) {
                            interactStateParameter.canActivate = true;
                        }
                        else if (inFrontOfAbsorbableObject) {
                            interactStateParameter.canAbsorb = true;
                        }
                        else if (inFrontOfPortableObject) {
                            interactStateParameter.canCarry = true;
                        }
                    }

                }

                
                break;

            case ActionRequest.None:
                interactStateParameter.canGlide = false;
                interactStateParameter.canMeleeAttack = false;
                interactStateParameter.canDistantAttack = false;
                interactStateParameter.canSpawnLure = false;
                interactStateParameter.canInflate = false;
                interactStateParameter.canActivate = false;
                interactStateParameter.canAbsorb = false;
                interactStateParameter.canCarry = false;
                interactStateParameter.canPush = false;
                break;
        }

    }

    

    public bool GetOnGround() { return onGround; }
    public float GetJumpForce() { return jumpForce; }

    //public bool TryTakeLeaf() {
    //    if (inputInteractParameters.LeafAvailable) {
    //        inputInteractParameters.LeafAvailable = false;
    //        return true;
    //    }
    //    return false;
    //}

    //public void ReturnLeaf() {
    //    inputInteractParameters.LeafAvailable = true;
    //}

    void OnTriggerStay(Collider collid) {


        if (interactState == InteractState.Absorb && previousInteractState != InteractState.Absorb) {
            Debug.Log("TRIGGER");
            if (interactStateParameter.canAbsorb) {
                Debug.Log("ENCORE PLUS TRIGGER");
                interactBehaviorCtrl.DoBeginAbsorption(collid.gameObject);
            }

        }else if (previousInteractState == InteractState.Absorb) {
            Capacity capacity = interactBehaviorCtrl.DoContinueAbsorption(collid.gameObject);
            if (capacity != Capacity.Nothing) {
                AddPower(capacity);
            }
        }
    }

    private void AddPower(Capacity capacity) {
        switch (capacity) {

            case Capacity.Nothing:
                break;

            case Capacity.DoubleJump:
                hasDoubleJumpCapacity = true;
                break;

            case Capacity.Glide:
                break;
        }
    }
}
