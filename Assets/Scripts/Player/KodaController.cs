using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

public class LeafLock {
    public bool isUsed;
    public InteractState parent;

    public LeafLock(bool used, InteractState parentState) {
        this.isUsed = used;
        this.parent = parentState;
    }
}

//public class Leaf {
//    public class LeafLock {
//        Leaf _parent;
//        public LeafLock(Leaf parent) {
//            _parent = parent;
//        }
//        public void Release() {
//            _parent.leafLock = null;
//            _parent = null;
//        }
//    }

//    private LeafLock leafLock;

//    public LeafLock TakeLeaf() {
//        if (leafLock == null) {
//            leafLock = new LeafLock(this);
//            return leafLock;
//        }
//        else {
//            throw new LeafAlreadyTakenException();
//        }
//    }
//}

public class Pair<T, U> {
    public Pair() {
    }

    public Pair(T first, U second) {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }
}

public class KodaController : MonoBehaviour {

    [Header("PLAYER")]
    [Space(10)]
    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField] private float animationMoveSpeed = 10f;
    [SerializeField] private float airControl = 12f;
    [SerializeField] private float jumpForce = 120f;
    [SerializeField] private float airStreamForce = 200f;
    [SerializeField] private float glideCounterForce = 150f;
    [SerializeField] private float slideAngleNormal = 45f;
    [SerializeField] private float slideAngleRock = 10f;
    [SerializeField] private GameObject absorbRange;

    private Vector3 inclinationNormal;
    private GameObject catchableObject;
    private GameObject objectToCarry;
    private GameObject actualLure;
    private float speed = 10f;
    private float coefInclination;
    private List<GameObject> _grounds = new List<GameObject>();

    //Orientation Camera player
    [Header("CAMERA")]
    [Space(10)]
    //[SerializeField] private Transform pivot;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField] private Transform playerModel;
    [SerializeField] private Transform direction;

    private Vector3 orientationMove;
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

    private bool allowedToWalk;

    private InputController inputController;

    [Header("CAPACITY")]
    [Space(10)]
    // Capacity
    [SerializeField]
    private bool permanentDoubleJumpCapacity;
    private bool permanentBallCapacity;
    private bool permanentShrinkCapacity;
    private Capacity temporaryCapacity;
    [SerializeField] private float timerCapacity;

    //QTE
    private float maxPowerUpGauge = 10f;
    public GameObject canvasQTE;
    public Transform loadingBar;
    public Transform centerButton;

    // Canvas UI
    [SerializeField] private GameObject CameraMinimap;
    [SerializeField] private GameObject CanvasPrefabPause;
    [SerializeField] private GameObject SceneTransitionImage;
    [SerializeField] private GameObject DeathTransitionImage;
    [SerializeField] private GameObject VictoryTransitionImage;

    //Animation
    private AnimationController animBody;
    private InteractBehavior interactBehaviorCtrl;

    private float timerAttack;

    [Header("SOUND")]
    [Space(10)]
    [SerializeField]
    private AudioClip jumpSound;
    [SerializeField] private AudioClip footStepSound1;
    [SerializeField] private AudioClip footStepSound2;
    [SerializeField] private AudioClip footStepSound3;
    private AudioClip[] allFootStepSound;
    [SerializeField] private AudioClip fallSound;
    [SerializeField] private AudioClip glideSound;
    [SerializeField] private AudioClip pushUpSound;
    private float timerStepSound;


    private LeafLock leafLock;

    int FPS = 40;

    void Awake() {
        Instantiate(CameraMinimap).name = "MinimapCamera";
        Instantiate(CanvasPrefabPause).name = "PauseCanvas";
        Instantiate(SceneTransitionImage).name = "SceneTransitionImage";
        Instantiate(DeathTransitionImage).name = "DeathTransitionImage";
        Instantiate(VictoryTransitionImage).name = "VictoryTransitionImage";
    }

    private void Start() {
        VictorySwitch.Victory = false;
        allFootStepSound = new AudioClip[] { footStepSound1, footStepSound2, footStepSound3 };

        leafLock = new LeafLock(false, InteractState.Nothing);
        movementState = MovementState.Idle;
        previousMovementState = movementState;
        interactState = InteractState.Nothing;
        previousInteractState = interactState;
        body = GetComponent<Rigidbody>();
        animBody = GetComponent<AnimationController>();
        moveStateParameters = new MovementStateParam();
        moveStateCtrl = new MovementStateController();
        interactStateParameter = new InteractStateParam();
        InteractStateCtrl = new InteractStateController();
        inputController = GetComponent<InputController>();
        interactBehaviorCtrl = GetComponent<InteractBehavior>();

        direction = transform.Find("Direction");
    }

    /*private void OnGUI() {
		GUI.Label(new Rect(0, 50, 200, 50), new GUIContent("Frames per second: " + 1 / Time.deltaTime));
		FPS = int.Parse(GUI.TextField (new Rect (0, 100, 200, 50), FPS.ToString()));
		Application.targetFrameRate = FPS;
	}*/

    private void FixedUpdate() {
        /*}

        private void Update() {*/
        if (Pause.Paused) {
            return;
        }

        if (timerCapacity > 0) {
            timerCapacity -= Time.deltaTime;
            ProgressTimerCapacity();
        }
        else {
            StopTemporaryCapacity();
        }

        previousMovementState = movementState;

        InputParams inputParams;
        //deplacements

        previousInteractState = interactState;

        // get updated inputParams
        inputParams = inputController.RetrieveUserRequest();
        UpdateMoveStateParameters(inputParams);

        // get updated inputParams
        inputParams = inputController.RetrieveUserRequest();
        UpdateInteractStateParameters(inputParams);

        movementState = moveStateCtrl.GetNewState(movementState, moveStateParameters);
        interactState = InteractStateCtrl.GetNewState(interactState, interactStateParameter);

        MoveAccordingToInput();
        ApplyMovement();
        InteractAccordingToInput();

        speed = Mathf.Sqrt(Mathf.Pow(inputParams.moveX, 2) + Mathf.Pow(inputParams.moveZ, 2));

        if (previousMovementState != movementState) {
            animBody.OnStateExit(previousMovementState);
            animBody.OnStateEnter(movementState);
        }

        if (previousInteractState != interactState) {
            animBody.OnStateExit(previousInteractState);
            animBody.OnStateEnter(interactState);
        }

        animBody.UpdateState(movementState, speed, animationMoveSpeed);
        //ResetInteractStateParameter();
    }

    void OnCollisionEnter(Collision coll) {

        GameObject gO = coll.gameObject;

        //Debug.Log ("gO.layer enter=" + LayerMask.LayerToName(gO.layer));
        //Debug.Log ("_grounds.count enter=" + _grounds.Count);
        if (gO.layer == LayerMask.NameToLayer("Ground") || gO.layer == LayerMask.NameToLayer("Rock")) {
            ContactPoint[] contacts = coll.contacts;

            //Debug.Log ("contacts.Length=" + contacts.Length);
            if (contacts.Length > 0) {

                timerStepSound = 0.25f;
                foreach (ContactPoint c in contacts) {
                    //Debug.Log ("c.normal.y=" + c.normal.y);
                    float slideAngle = 0;
                    if (gO.layer == LayerMask.NameToLayer("Ground")) {
                        slideAngle = slideAngleNormal;
                    }
                    else if (gO.layer == LayerMask.NameToLayer("Rock")) {
                        slideAngle = slideAngleRock;
                    }
                    coefInclination = Vector3.Angle(c.normal, Vector3.up);
                    //Debug.Log ("coefInclination=" + coefInclination);
                    if (coefInclination >= 0.0f && coefInclination < slideAngle + 0.01f && !_grounds.Contains(gO)) {
                        _grounds.Add(gO);
                        break;
                    }
                }

                //Uncomment when koda's collider will be change
                //if(_grounds.Count == 1) {
                //    SoundController.instance.PlayKodaSingle(fallSound);
                //}
            }
        }
    }

    void OnCollisionStay(Collision coll) {
        GameObject gO = coll.gameObject;

        if (gO.layer == LayerMask.NameToLayer("Ground") || gO.layer == LayerMask.NameToLayer("Rock")) {
            ContactPoint[] contacts = coll.contacts;

            if (contacts.Length > 0) {
                bool found = false;
                timerStepSound -= Time.deltaTime;
                if (timerStepSound <= 0 && speed > 0) {
                    SoundController.instance.RandomizeFX(allFootStepSound);
                    timerStepSound = 0.25f;
                }
                foreach (ContactPoint c in contacts) {
                    if (c.normal != null) {
                        float slideAngle = 0;
                        if (gO.layer == LayerMask.NameToLayer("Ground")) {
                            slideAngle = slideAngleNormal;
                        }
                        else if (gO.layer == LayerMask.NameToLayer("Rock")) {
                            slideAngle = slideAngleRock;
                        }

                        //Debug.Log ("Ground=" + c.normal + " norm=" + c.normal.y + " name=" + gO.name + " " + Time.time);
                        coefInclination = Vector3.Angle(c.normal, Vector3.up);
                        if (coefInclination >= 0.0f && coefInclination < slideAngle + 0.01f) {
                            allowedToWalk = true;
                        }
                        else {
                            allowedToWalk = false;
                            inclinationNormal = c.normal;
                        }
                        //direction.rotation = Quaternion.AngleAxis (Camera.main.transform.eulerAngles.y, Vector3.up);
                        direction.RotateAround(direction.position, Vector3.right, gO.transform.rotation.eulerAngles.x);
                        direction.RotateAround(direction.position, Vector3.forward, gO.transform.rotation.eulerAngles.z);
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision coll) {
        //Debug.Log ("_grounds.count exit=" + _grounds.Count);
        if (IsGrounded()) {
            GameObject gO = coll.gameObject;
            if (gO.layer == LayerMask.NameToLayer("Ground") || gO.layer == LayerMask.NameToLayer("Rock")) {
                if (_grounds.Contains(gO)) {
                    _grounds.Remove(gO);
                }
            }
            //Debug.Log ("_grounds.count exit=" + _grounds.Count);
        }
    }

    public void StopMeleeAttackState() {
        interactStateParameter.finishedMeleeAttack = true;
    }

    public void StopDistantAttackState() {
        interactStateParameter.finishedDistantAttack = true;
    }

    private bool IsGrounded() {
        //coefInclination = 0;
        return _grounds.Count > 0;
    }

    void MoveAccordingToInput() {
        if (coefInclination > 29.99f && coefInclination < slideAngleNormal + 0.01f) {
            body.velocity = new Vector3(0, body.velocity.y, 0);
        }
        body.AddForce(Vector3.down * (220.0f / body.mass + 9.81f) * (40 * Time.deltaTime), ForceMode.Acceleration);
        /* ou si maudit et pas en state jump / fall */
        //JUMP
        //Debug.Log("moveStateParameters.jumpRequired2=" + moveStateParameters.jumpRequired);
        if (moveStateParameters.jumpRequired) {
            //Debug.Log ("IMPULSE!!!");
            // force the velocity to 0.02f (near 0) in order to reset the Y velocity (for better jump)
            transform.position += new Vector3(0, 0.1f, 0);
            body.velocity = new Vector3(body.velocity.x, 0.02f, body.velocity.z);
            try {
                SoundController.instance.PlayKodaSingle(jumpSound);
            }
            catch {
            }
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        //Debug.Log ("allowedToWalk=" + allowedToWalk);
        if (allowedToWalk) {
            //Debug.Log ("0-45");
            inputVelocityAxis =
                (
                moveStateParameters.moveX * direction.right +
                moveStateParameters.moveZ * direction.forward
            ).normalized * moveSpeed;
        }
        else {
            //Debug.Log ("45-90");
            inclinationNormal.y = 0;
            inclinationNormal.Normalize();
            //Debug.Log ("inclinationNormal=" + inclinationNormal);
            Vector3 forwardNoY = direction.forward;
            forwardNoY.y = 0;
            forwardNoY.Normalize();
            //Debug.Log ("forwardNoY=" + forwardNoY);
            Vector3 rightNoY = direction.right;
            rightNoY.y = 0;
            rightNoY.Normalize();
            //Debug.Log ("rightNoY=" + rightNoY);
            float scalar = Vector3.Dot(inclinationNormal, (forwardNoY * moveStateParameters.moveZ + rightNoY * moveStateParameters.moveX).normalized);
            float factor = 1;
            if (scalar < 0) {
                factor = 1 + scalar;
            }
            inputVelocityAxis =
                (
                moveStateParameters.moveX * rightNoY +
                moveStateParameters.moveZ * forwardNoY
            ).normalized * moveSpeed * factor;
        }
    }

    private void ApplyMovement() {
        transform.position += inputVelocityAxis * Time.deltaTime;

        //Orientation du personnage
        orientationMove = (direction.forward * moveStateParameters.moveZ) + (direction.right * moveStateParameters.moveX);
        //Move player on direction based on camera
        if (moveStateParameters.moveX != 0 || moveStateParameters.moveZ != 0) {
            transform.rotation = Quaternion.Euler(0, direction.rotation.eulerAngles.y, 0);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(orientationMove.x, 0f, orientationMove.z));
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, newRotation, rotateSpeed * Time.fixedDeltaTime);
        }

        // reset variables
        allowedToWalk = true;
        inclinationNormal = Vector3.zero;
        direction.rotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
    }

    void InteractAccordingToInput() {
        switch (interactState) {
            case InteractState.Nothing:
                if (leafLock.isUsed) {
                    if (previousInteractState == InteractState.MeleeAttack) {
                        interactBehaviorCtrl.StopMeleeAttack();
                        leafLock.isUsed = false;
                        leafLock.parent = InteractState.Nothing;
                    }

                    if (previousInteractState == InteractState.DistantAttack) {
                        interactBehaviorCtrl.StopDistantAttack();
                        leafLock.isUsed = false;
                        leafLock.parent = InteractState.Nothing;
                    }

                    if (previousInteractState == InteractState.Glide) {
                        interactBehaviorCtrl.StopGlide();
                        leafLock.isUsed = false;
                        leafLock.parent = InteractState.Nothing;
                    }

                    if (previousInteractState == InteractState.Inflate) {
                        interactBehaviorCtrl.DoInflate(false);
                        leafLock.isUsed = false;
                        leafLock.parent = InteractState.Nothing;
                    }

                    if (previousInteractState == InteractState.Tiny) {
                        interactBehaviorCtrl.DoResizeTiny(false);
                        leafLock.isUsed = false;
                        leafLock.parent = InteractState.Nothing;
                    }

                }

                if (interactStateParameter.finishedCarry && previousInteractState == InteractState.Carry) {
                    interactBehaviorCtrl.StopCarry(catchableObject);
                    leafLock.parent = InteractState.Nothing;
                }

                break;

            case InteractState.Glide:
                if (IsGrounded()) {
                    interactBehaviorCtrl.StopGlide();
                    leafLock.isUsed = false;
                    leafLock.parent = InteractState.Nothing;
                }
                else {
                    if (!leafLock.isUsed || (leafLock.isUsed && leafLock.parent == InteractState.Glide)) {
                        // add a force to counter gravity (glide effect)

                        //body.AddForce(Vector3.up * glideCounterForce, ForceMode.Force);
                        body.AddForce(Vector3.up * (glideCounterForce / body.mass) * (40 * Time.deltaTime), ForceMode.Acceleration);
                        if (body.velocity.y < -9) {
                            body.velocity = new Vector3(body.velocity.x, -9, body.velocity.z);
                        }
                        interactBehaviorCtrl.DoGlide();
                        if (interactStateParameter.canAirStream) {
                            body.AddForce(Vector3.up * (airStreamForce / body.mass) * (40 * Time.deltaTime) + (Vector3.up * Mathf.Abs(body.velocity.y)), ForceMode.Acceleration);
                            if (body.velocity.y > 8.0f) {
                                //Debug.Log ("STOP AIRSTREAM!!!");
                                // force the velocity to 0.02f (near 0) in order to reset the Y velocity (for better jump)
                                body.velocity = new Vector3(body.velocity.x, 8.0f, body.velocity.z);
                            }
                        }

                        if (previousInteractState != InteractState.Glide) {
                            SoundController.instance.PlayKodaSingle(glideSound);
                        }
                        leafLock.isUsed = true;
                        leafLock.parent = InteractState.Glide;
                    }
                }
                break;

            case InteractState.MeleeAttack:
                if (interactStateParameter.canMeleeAttack && !leafLock.isUsed) {
                    interactBehaviorCtrl.DoMeleeAttack();
                    leafLock.isUsed = true;
                }
                break;

            case InteractState.DistantAttack:
                if (interactStateParameter.canDistantAttack && !leafLock.isUsed) {
                    interactBehaviorCtrl.DoDistantAttack();
                    leafLock.isUsed = true;
                }
                break;

            case InteractState.SpawnLure:
                if (previousInteractState == InteractState.Nothing && !leafLock.isUsed) {
                    actualLure = interactBehaviorCtrl.DoSpawnLure();
                    leafLock.isUsed = true;
                    leafLock.parent = InteractState.SpawnLure;
                }
                break;

            case InteractState.DestroyLure:
                if (previousInteractState == InteractState.Nothing && interactStateParameter.canDestroyLure) {
                    interactBehaviorCtrl.DestroyLure(actualLure);
                    actualLure = null;
                    leafLock.isUsed = false;
                    leafLock.parent = InteractState.Nothing;
                }
                break;

            case InteractState.Inflate:
                if (previousInteractState != InteractState.Inflate && !leafLock.isUsed) {
                    interactBehaviorCtrl.DoInflate(true);
                    leafLock.isUsed = true;
                    leafLock.parent = InteractState.Inflate;
                }
                break;

            case InteractState.Tiny:
                if (previousInteractState == InteractState.Nothing && !leafLock.isUsed) {
                    interactBehaviorCtrl.DoResizeTiny(true);
                    leafLock.isUsed = true;
                    leafLock.parent = InteractState.Tiny;
                }
                break;

            case InteractState.Activate:
                break;

            case InteractState.Absorb:
                GameObject nearestObject = absorbRange.GetComponent<DetectNearInteractObject>().GetNearestObject();
                if (nearestObject == null || !nearestObject.CompareTag("Yokai")) {
                    interactStateParameter.yokaiStillInRange = false;
                }
                break;

            case InteractState.Carry:
                if (previousInteractState == InteractState.Nothing) {
                    interactBehaviorCtrl.DoCarry(objectToCarry);
                    catchableObject = objectToCarry;
                }
                break;

            case InteractState.Push:
                break;
        }
    }

    float hysteresis_step = 0.01f;

    public bool IsGoingUp(MovementStateParam param) {
        bool up = !param.grounded && param.velocity.y > hysteresis_step;

        return up;
    }

    public bool IsFalling(MovementStateParam param) {
        bool fall = !param.grounded && (param.velocity.y < -hysteresis_step);

        return fall;
    }

    void UpdateMoveStateParameters(InputParams inputParams) {
        if (!IsGoingUp(moveStateParameters) && !IsFalling(moveStateParameters)) {
            moveStateParameters.position_before_fall = body.position;
        }
        moveStateParameters.position = body.position;
        moveStateParameters.velocity = body.velocity;
        moveStateParameters.moveX = inputParams.moveX;
        moveStateParameters.moveZ = inputParams.moveZ;
        moveStateParameters.jumpRequired =
            inputParams.jumpRequest &&
            (movementState == MovementState.Idle ||
                movementState == MovementState.Run ||
                (movementState == MovementState.Jump && ((temporaryCapacity == Capacity.DoubleJump) || permanentDoubleJumpCapacity) && interactState != InteractState.Carry));
        moveStateParameters.grounded = IsGrounded();
        if (inputParams.jumpRequest) {
            //Debug.Log("movementState=" + movementState);
            //Debug.Log("interactState=" + interactState);
            //Debug.Log("IsGrounded=" + IsGrounded());
            //Debug.Log("moveStateParameters.jumpRequired1=" + moveStateParameters.jumpRequired);
            inputParams.jumpRequest = false;
            inputController.SetUserRequest(inputParams);
        }
    }

    void UpdateInteractStateParameters(InputParams inputParams) {
        switch (inputParams.actionRequest) {
            case ActionRequest.Glide:
                if ((movementState == MovementState.Fall || movementState == MovementState.PushUp) && (!leafLock.isUsed || leafLock.parent == InteractState.Glide)) {
                    interactStateParameter.canGlide = true;
                }
                else {
                    interactStateParameter.canGlide = false;
                }
                break;

            case ActionRequest.MeleeAttack:
                if (interactState != InteractState.Glide && !leafLock.isUsed) {
                    interactStateParameter.finishedMeleeAttack = false;
                    interactStateParameter.canMeleeAttack = true;
                }
                break;

            case ActionRequest.DistantAttack:
                if (interactState != InteractState.Glide && !leafLock.isUsed) {
                    interactStateParameter.finishedDistantAttack = false;
                    interactStateParameter.canDistantAttack = true;
                }
                break;

            case ActionRequest.SpawnLure:
                if (actualLure == null && !leafLock.isUsed) {
                    interactStateParameter.canSpawnLure = true;
                }
                else {

                    interactStateParameter.canDestroyLure = true;
                }
                break;

            case ActionRequest.Inflate:
                if (IsGrounded() && (!leafLock.isUsed || (leafLock.isUsed && leafLock.parent == InteractState.Inflate))) interactStateParameter.canInflate = true;
                break;

            case ActionRequest.Resize:
                if (IsGrounded() && (!leafLock.isUsed || (leafLock.isUsed && leafLock.parent == InteractState.Tiny))) interactStateParameter.canResize = true;
                break;

            case ActionRequest.ContextualAction:
                GameObject nearestObject = absorbRange.GetComponent<DetectNearInteractObject>().GetNearestObject();

                if (interactState == InteractState.Carry) {
                    interactStateParameter.finishedCarry = true;
                }
                else if (nearestObject != null) {
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
                            objectToCarry = nearestObject;
                            //reset action so that we cannot catch and decatch due to malsynchronization
                        }
                    }
                }
                break;

            case ActionRequest.None:
                ResetInteractStateParameter();
                nearestObject = null;
                break;
        }
        if (inputParams.actionRequest != ActionRequest.Glide) {
            inputParams.actionRequest = ActionRequest.None;
            inputController.SetUserRequest(inputParams);
        }
    }

    private void ResetInteractStateParameter() {
        interactStateParameter.canGlide = false;
        interactStateParameter.canMeleeAttack = false;
        interactStateParameter.canDistantAttack = false;
        interactStateParameter.canSpawnLure = false;
        interactStateParameter.canDestroyLure = false;
        interactStateParameter.canInflate = false;
        interactStateParameter.canResize = false;
        interactStateParameter.canActivate = false;
        interactStateParameter.canAbsorb = false;
        interactStateParameter.canCarry = false;
        interactStateParameter.canPush = false;
        interactStateParameter.finishedCarry = false;
        interactStateParameter.canAirStream = false;
    }


    public float GetJumpForce() { return jumpForce; }

    public InteractState GetInteractState() { return interactState; }

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

    void OnTriggerEnter(Collider collid) {
        if (collid.gameObject.CompareTag("AirStreamZone")) {
            //Debug.Log("AirStreamZone enter");
            moveStateParameters.inAirStream = true;
            if (interactState == InteractState.Glide) {
                SoundController.instance.PlaySingle(pushUpSound);
            }
        }
        if (collid.gameObject.CompareTag("AirStreamForce") && interactState == InteractState.Glide) {

            //Debug.Log("AirStreamForce enter");
            interactStateParameter.canAirStream = true;
            /*body.velocity = new Vector3 (body.velocity.x, 0, body.velocity.z);
			body.AddForce (Vector3.up * 80, ForceMode.Impulse);*/
        }
    }

    void OnTriggerExit(Collider collid) {
        if (collid.gameObject.CompareTag("AirStreamZone")) {
            //Debug.Log("AirStreamZone Exit");
            SoundController.instance.StopSingle();
            moveStateParameters.inAirStream = false;
        }

        if (collid.gameObject.CompareTag("AirStreamForce")) {
            //Debug.Log("AirStreamForce Exit " + body.velocity.y);

            interactStateParameter.canAirStream = false;
        }
    }

    void OnTriggerStay(Collider collid) {
        if (collid.gameObject.CompareTag("AirStreamZone")) {
            if (interactState != InteractState.Glide) { //&& previousInteractState == InteractState.Glide
                moveStateParameters.inAirStream = false;
                //Debug.Log("In Air Stream ZONE - NO GLIDE");
            }
            else if (interactState == InteractState.Glide) { //&& previousInteractState != InteractState.Glide
                moveStateParameters.inAirStream = true;
                //Debug.Log("In Air Stream ZONE - GLIDE");
            }
        }
        else if (collid.gameObject.CompareTag("AirStreamForce")) {
            if (interactState != InteractState.Glide) { //&& previousInteractState == InteractState.Glide
                interactStateParameter.canAirStream = false;
                //Debug.Log("In Air Stream FORCE - NO GLIDE");
            }
            else if (interactState == InteractState.Glide) { //&& previousInteractState != InteractState.Glide
                interactStateParameter.canAirStream = true;
                body.velocity = new Vector3(body.velocity.x, 10f, body.velocity.z);
                //Debug.Log("In Air Stream FORCE - GLIDE");
                /*body.velocity = new Vector3 (body.velocity.x, 0, body.velocity.z);
				body.AddForce (Vector3.up * 10, ForceMode.Impulse);*/
            }
        }
        else if (collid.CompareTag("Yokai")) {
            if (interactState == InteractState.Absorb && previousInteractState != InteractState.Absorb) {
                if (interactStateParameter.canAbsorb) {
                    interactBehaviorCtrl.DoBeginAbsorption(collid.gameObject);
                }

            }
            else if (previousInteractState == InteractState.Absorb) {
                Pair<Capacity, float> pairCapacity = interactBehaviorCtrl.DoContinueAbsorption(collid.gameObject, inputController);
                if (pairCapacity.First != Capacity.Nothing) {
                    AddCapacity(pairCapacity);
                }

            }
        }
    }

    //Temporary in public mode for the playtest
    public void AddCapacity(Pair<Capacity, float> pairCapacity) {
        temporaryCapacity = pairCapacity.First;
        switch (pairCapacity.First) {

            case Capacity.DoubleJump:
                canvasQTE.SetActive(true);
                break;

            case Capacity.Glide:
                break;
        }

        timerCapacity = pairCapacity.Second;
        maxPowerUpGauge = pairCapacity.Second;
    }

    private void StopTemporaryCapacity() {
        timerCapacity = 0;
        temporaryCapacity = Capacity.Nothing;
        canvasQTE.SetActive(false);

        if (temporaryCapacity == Capacity.Lure) {
            interactBehaviorCtrl.DestroyLure(actualLure);
            ResetLeafLock();
        }
    }

    private void ProgressTimerCapacity() {
        loadingBar.GetComponent<Image>().fillAmount = timerCapacity / maxPowerUpGauge;
    }

    public void ResetPlayer() {
        ResetLeafLock();
        // TODO : reset temporary capacity
    }

    public void ResetLeafLock() {
        leafLock.isUsed = false;
        leafLock.parent = InteractState.Nothing;
        interactBehaviorCtrl.ResetLeaf();
    }

    public int GetCaughtYokai() { return 0; } //Work in progress ...
    public bool GetPowerJump() { return permanentDoubleJumpCapacity; }
    public bool GetPowerBall() { return permanentBallCapacity; }
    public bool GetPowerShrink() { return permanentShrinkCapacity; }
    public Transform GetRespawnPointPosition() { return gameObject.transform; }

    public void SetCaughtYokai(int yokai_caught) { Debug.Log("ARRETEZ DE VOUS BATTEZ ! D:"); } //Work in progress ...
    public void SetPowerJump(bool has_double_jump) { permanentDoubleJumpCapacity = has_double_jump; }
    public void SetPowerBall(bool has_power_ball) { permanentBallCapacity = has_power_ball; }
    public void SetPowerShrink(bool has_power_shrink) { permanentShrinkCapacity = has_power_shrink; }
    public void SetRespawnPointPosition(float x_pos, float y_pos, float z_pos) { body.position = new Vector3(x_pos, y_pos, z_pos); }
}
