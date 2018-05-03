using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
//================================================
//SOUNDCONTROLER
//================================================

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
    [SerializeField] private float airControl = 12f;
    [SerializeField] private float jumpForce = 120f;
    [SerializeField] private float airStreamForce = 200f;
    [SerializeField] private float glideCounterForce = 150f;
    [SerializeField] private float slideAngleNormal = 45f;
    [SerializeField] private float slideAngleRock = 10f;
    [SerializeField] private GameObject interactArea;
    [SerializeField] private float fieldOfView = 45f;

    private PlayerHealth playerHealth;
    private DetectNearInteractObject detectNearInteractObject;
    private List<Vector3> inclinationNormals;
    private GameObject catchableObject;
    private GameObject objectToCarry;
    private GameObject actualLure;
    private float speed = 10f;
    private float coefInclination;
    private List<GameObject> _grounds = new List<GameObject>();
    private float ratio;

    //Orientation Camera player
    [Header("CAMERA")]
    [Space(10)]
    //[SerializeField] private Transform pivot;
    [SerializeField]
    private float rotateSpeed;
    [SerializeField] private Transform playerModel;
    [SerializeField] private Transform direction;

    private float angle;

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
    private Vector3 normal_contact;

    private InputController inputController;

    [Header("CAPACITY")]
    [Space(10)]
    // Capacity
    [SerializeField]
    private bool hasPermanentDoubleJumpCapacity;
    [SerializeField] private bool hasPermanentLureCapacity;
    private bool hasPermanentBallCapacity;
    private bool hasPermanentShrinkCapacity;
    private Capacity temporaryCapacity;
    private float timerCapacity;

    //QTE
    private float maxPowerUpGauge = 10f;
    [SerializeField] private GameObject canvasQTE;
    Image loadingBar;
    [SerializeField] private GameObject doubleJump_Logo;
    [SerializeField] private GameObject spawnLure_Logo;

    // Canvas UI
    [SerializeField] private GameObject CameraMinimap;
    [SerializeField] private GameObject CanvasPrefabPause;
    [SerializeField] private GameObject SceneTransitionImage;
    [SerializeField] private GameObject DeathTransitionImage;
    [SerializeField] private GameObject VictoryTransitionImage;

    // PowerUp
    [Tooltip("0 : Body & 1 : Accessorizes")]
    [SerializeField] private Material[] mats = new Material[2];
    [Tooltip("0 : Double Jump, 1 : Lure")]
    [SerializeField] private Texture[] textures = new Texture[2];
    [Tooltip("0 : Double Jump, 1 : Lure")]
    [SerializeField] private Color[] colors = new Color[2];
    [SerializeField] private float lerpBtwColor = .1f;


    //Animation
    private AnimationController animBody;
    private InteractBehavior interactBehaviorCtrl;

    private float timerAttack;
    private float timerStepSound = 0.25f;
    private LeafLock leafLock;

    int FPS = 40;

    [Header("LANTERN")]
    [Space(10)]
    private bool runOnWater = false;
    private GameObject[] lanterns;
    private GameObject lanternNearest = null;
    private bool playerStop = false;

    InputParams inputParams;

    private bool levelIsLight = false;


    void Awake() {
        Instantiate(CameraMinimap).name = "MinimapCamera";
        //Instantiate(CanvasPrefabPause).name = "PauseCanvas";
        Instantiate(SceneTransitionImage).name = "SceneTransitionImage";
        //Instantiate(LoadingScreen).name = "LoadingScreen";
        Instantiate(DeathTransitionImage).name = "DeathTransitionImage";
        Instantiate(VictoryTransitionImage).name = "VictoryTransitionImage";
    }

    private void Start() {
        if(Game.playerData.lightBoss1 && (SceneManager.GetActiveScene().name == "Z1-P1-complete" || SceneManager.GetActiveScene().name == "Z1-P2-complete" || SceneManager.GetActiveScene().name == "Z1-P3-complete" || SceneManager.GetActiveScene().name == "Boss1")) { levelIsLight = true; }
        if (Game.playerData.lightBoss2 && (SceneManager.GetActiveScene().name == "Z2-P1-complete" || SceneManager.GetActiveScene().name == "Z2-P2-complete" || SceneManager.GetActiveScene().name == "Z2-P3-complete" || SceneManager.GetActiveScene().name == "Boss2")) { levelIsLight = true; }

        VictorySwitch.Victory = false;

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
        playerHealth = GetComponent<PlayerHealth>();
        detectNearInteractObject = interactArea.GetComponent<DetectNearInteractObject>();
        loadingBar = canvasQTE.transform.GetChild(0).gameObject.GetComponent<Image>();

        direction = transform.Find("Direction");

        lanterns = GameObject.FindGameObjectsWithTag("Lantern");
        inclinationNormals = new List<Vector3>();
    }

    /*private void OnGUI() {
        GUI.Label(new Rect(0, 50, 200, 50), new GUIContent("Frames per second: " + 1 / Time.deltaTime));
        FPS = int.Parse(GUI.TextField (new Rect (0, 100, 200, 50), FPS.ToString()));
        Application.targetFrameRate = FPS;
    }*/

    private void FixedUpdate() {
        //GC.Collect();
        //Resources.UnloadUnusedAssets();
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
		List<int> groundsToDelete = null;
		for (int i = 0 ; i < _grounds.Count ; i++) {
			if (_grounds[i] == null) {
				if (groundsToDelete == null) {
					groundsToDelete = new List<int> ();
				}
				groundsToDelete.Add (i);
			}
		}

		if (groundsToDelete != null) {
			foreach (int groundToDelete in groundsToDelete) {
				_grounds.Remove (_grounds[groundToDelete]);
			}
		}

        ratio = (40 * Time.deltaTime);

        if (timerCapacity > 0) {
            timerCapacity -= Time.deltaTime;
            ProgressTimerCapacity();
        }
        else {
            StopTemporaryCapacity();
        }

        previousMovementState = movementState;

        //InputParams inputParams;
        //deplacements

        previousInteractState = interactState;

        // get updated inputParams
        inputParams = inputController.RetrieveUserRequest();
        if (IsGrounded()) {
            inputParams.hasDoubleJumped = false;
        }
        else if (movementState == MovementState.DoubleJump) {
            inputParams.hasDoubleJumped = true;
        }

        //if(inputParams.jumpRequest) Debug.Log ("inputParams.jumpRequest 1 = " + inputParams.jumpRequest + " " + Time.time);
        inputController.SetUserRequest(inputParams);
        UpdateMoveStateParameters(inputParams);

        //if(inputParams.jumpRequest) Debug.Log ("inputParams.jumpRequest 2 = " + inputParams.jumpRequest + " " + Time.time);
        // get updated inputParams
        inputParams = inputController.RetrieveUserRequest();
        UpdateInteractStateParameters(inputParams);

        //if(inputParams.jumpRequest) Debug.Log ("inputParams.jumpRequest 3 = " + inputParams.jumpRequest + " " + Time.time);
        movementState = moveStateCtrl.GetNewState(movementState, moveStateParameters);
        //if(inputParams.jumpRequest) Debug.Log ("inputParams.jumpRequest 4 = " + inputParams.jumpRequest + " " + Time.time);
        interactState = InteractStateCtrl.GetNewState(interactState, interactStateParameter);

        // get updated inputParams
        inputParams = inputController.RetrieveUserRequest();
        //if(inputParams.jumpRequest) Debug.Log ("inputParams.jumpRequest 5 = " + inputParams.jumpRequest + " " + Time.time);
        MoveAccordingToInput();
        // get updated inputParams
        inputParams = inputController.RetrieveUserRequest();
        //if(inputParams.jumpRequest) Debug.Log ("inputParams.jumpRequest 6 = " + inputParams.jumpRequest + " " + Time.time);
        ApplyMovement();
        // get updated inputParams
        inputParams = inputController.RetrieveUserRequest();
        //if(inputParams.jumpRequest) Debug.Log ("inputParams.jumpRequest 7 = " + inputParams.jumpRequest + " " + Time.time);
        InteractAccordingToInput();

        speed = Mathf.Sqrt(Mathf.Pow(inputParams.moveX, 2) + Mathf.Pow(inputParams.moveZ, 2));

        //================================================
        if (previousMovementState == MovementState.Fall && (movementState == MovementState.Idle || movementState == MovementState.Run)) {
            if (!runOnWater) {
                SoundController.instance.SelectKODA("FallGround");
            }
            if (runOnWater) {
                SoundController.instance.SelectKODA("FallWater");
            }
        }
        //================================================

        if (previousMovementState != movementState) {
            animBody.OnStateExit(previousMovementState);
            animBody.OnStateEnter(movementState);
        }

        if (previousInteractState != interactState) {
            animBody.OnStateExit(previousInteractState);
            animBody.OnStateEnter(interactState);
        }

        animBody.UpdateState(movementState, speed, angle / 67.5f, body.velocity.y / 10);
        angle = Mathf.Lerp(angle, 0, 0.2f);
        //ResetInteractStateParameter();

        if (runOnWater) {
            //Debug.Log("on water");
            if ((!Game.playerData.lightBoss1 && SceneManager.GetActiveScene().name != "Boss1") || (!Game.playerData.lightBoss2 && SceneManager.GetActiveScene().name != "Boss2")) { 
                float distance = 0f;
                if (lanterns.Length > 0) {
                    lanternNearest = lanterns[0];
                    distance = Vector3.Distance(lanternNearest.transform.position, transform.position);
                }
                else {
                    lanternNearest = null;
                }
                foreach (GameObject lantern in lanterns) {
                    float dis = Vector3.Distance(lantern.transform.position, transform.position);
                    if (dis < distance) {
                        lanternNearest = lantern;
                        distance = dis;
                    }
                }
                if (!levelIsLight) {
                    if (lanternNearest != null) {
                        if (distance > lanternNearest.GetComponent<LanternController>().GetRadiusEffect()) {
                            //Debug.Log("die distance");
                            playerHealth.PlayerDie();
                            runOnWater = false;
                        }
                    }
                    else {
                        playerHealth.PlayerDie();
                        runOnWater = false;
                    }
                }
            }
            else {
                runOnWater = true;
            }
        }
    }

    void OnCollisionEnter(Collision coll) {
        GameObject gO = coll.gameObject;

        //Debug.Log ("gO.layer enter=" + LayerMask.LayerToName(gO.layer));
        //Debug.Log ("_grounds.count enter=" + _grounds.Count);
        if (gO.layer == LayerMask.NameToLayer("Ground") || gO.layer == LayerMask.NameToLayer("Rock") || gO.layer == LayerMask.NameToLayer("Water")) {
            if (gO.layer == LayerMask.NameToLayer("Water")) {
                runOnWater = true;
            }
            ContactPoint[] contacts = coll.contacts;

            //Debug.Log ("contacts.Length=" + contacts.Length);
            if (contacts.Length > 0) {

                timerStepSound = 0.25f;
                foreach (ContactPoint c in contacts) {
                    //Debug.Log ("c.normal.y=" + c.normal.y);
                    // no need to give water a slide angle since its angle is always 0 (default value of slideAngle)
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

        if (gO.layer == LayerMask.NameToLayer("Ground") || gO.layer == LayerMask.NameToLayer("Rock") || gO.layer == LayerMask.NameToLayer("Water")) {
            ContactPoint[] contacts = coll.contacts;

            if (contacts.Length > 0) {
                bool found = false;
                timerStepSound -= Time.deltaTime;
                if (timerStepSound <= 0 && speed > 0 && (gO.layer == LayerMask.NameToLayer("Ground") || gO.layer == LayerMask.NameToLayer("Rock"))) {
                    //================================================
                    SoundController.instance.SelectKODA("FootStepGround");
                    timerStepSound = 0.25f;
                    //================================================
                }
                if (timerStepSound <= 0 && speed > 0 && (gO.layer == LayerMask.NameToLayer("Water"))) {
                    //================================================
                    SoundController.instance.SelectKODA("FootStepWater");
                    timerStepSound = 0.25f;
                    //================================================
                }
                foreach (ContactPoint c in contacts) {
                    if (c.normal != null) {
                        float slideAngle = 0;
                        // no need to give water a slide angle since its angle is always 0 (default value of slideAngle)
                        if (gO.layer == LayerMask.NameToLayer("Ground")) {
                            slideAngle = slideAngleNormal;
                        }
                        else if (gO.layer == LayerMask.NameToLayer("Rock")) {
                            slideAngle = slideAngleRock;
                        }

                        coefInclination = Vector3.Angle(c.normal, Vector3.up);
                        //Debug.Log ("Ground=" + c.normal + " norm=" + c.normal.y + " angle=" + coefInclination + " name=" + gO.name + " " + Time.time);
                        if (coefInclination >= 0.0f && coefInclination < slideAngle + 0.01f /*|| Vector3.Distance(c.point, transform.position) < 0.5f*/) {
                            allowedToWalk = true;
                            normal_contact = c.normal;
                        }
                        else {
                            inclinationNormals.Add(c.normal);
                            allowedToWalk = false;
                            //inclinationNormal = c.normal;
                        }


                        // this condition is taken from OnCollisionEnter to avoid the Fall state
                        // when we jump directly to the platform without passing over it
                        // IF IT MAKES ISSUES REMOVE IT
                        if (coefInclination >= 0.0f && coefInclination < slideAngle + 0.01f && !_grounds.Contains(gO)) {
                            _grounds.Add(gO);
                        }

                        break;
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision coll) {
        //Debug.Log ("_grounds.count exit=" + _grounds.Count);
        if (IsGrounded()) {
            GameObject gO = coll.gameObject;
            if (gO.layer == LayerMask.NameToLayer("Ground") || gO.layer == LayerMask.NameToLayer("Rock") || gO.layer == LayerMask.NameToLayer("Water")) {
                if (gO.layer == LayerMask.NameToLayer("Water")) {
                    runOnWater = false;
                }
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
        body.AddForce(Vector3.down * (220.0f / body.mass + 9.81f) * ratio, ForceMode.Acceleration);
        /* ou si maudit et pas en state jump / fall */
        //JUMP
        //Debug.Log("moveStateParameters.jumpRequired2=" + moveStateParameters.jumpRequired);
        if (moveStateParameters.jumpRequired) {
            //Debug.Log ("IMPULSE!!!");
            // force the velocity to 0.02f (near 0) in order to reset the Y velocity (for better jump)
            transform.position += new Vector3(0, 0.1f, 0);
            body.velocity = new Vector3(body.velocity.x, 0.02f, body.velocity.z);
            try {
                //================================================
                SoundController.instance.SelectKODA("Jump");
                //================================================
            }
            catch {
            }
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        //Debug.Log ("allowedToWalk=" + allowedToWalk);
        if (allowedToWalk) {
            //Debug.Log ("0-45");
            inputVelocityAxis = (moveStateParameters.moveX * direction.right +
                                 moveStateParameters.moveZ * direction.forward);
            if (inputVelocityAxis.magnitude > 1)
                inputVelocityAxis.Normalize();
            inputVelocityAxis *= moveSpeed;
        }
        else {
            //Debug.Log ("45-90");
            float scalar_forward = 0;
            float scalar_right = 0;

            Vector3 forwardNoY = direction.forward;
            forwardNoY.y = 0;
            forwardNoY = forwardNoY.normalized * moveStateParameters.moveZ;
            //Debug.Log ("forwardNoY=" + forwardNoY);
            Vector3 rightNoY = direction.right;
            rightNoY.y = 0;
            rightNoY = rightNoY.normalized * moveStateParameters.moveX;

            float factor_forward = 1;
            float factor_right = 1;

            if (inclinationNormals.Count > 1) {
                Vector3 inclinationNormalAdd = Vector3.zero;
                Vector3 inclinationTengeantAddForward = Vector3.zero;
                Vector3 inclinationTengeantAddRight = Vector3.zero;
                List<Vector3> inclinationTengeantForward = new List<Vector3>();
                List<Vector3> inclinationTengeantRight = new List<Vector3>();

                //Debug.Log ("inclinationNormals tot=" + inclinationNormals.Count);
                foreach (Vector3 IN in inclinationNormals) {
                    Vector3 inclinationNormal = IN;
                    inclinationNormal.y = 0;
                    inclinationNormal.Normalize();

                    Vector3 forwardProj = forwardNoY - Vector3.Project(forwardNoY, inclinationNormal);
                    Vector3 rightProj = rightNoY - Vector3.Project(rightNoY, inclinationNormal);

                    inclinationTengeantForward.Add(forwardProj);
                    inclinationTengeantRight.Add(rightProj);

                    inclinationNormalAdd += inclinationNormal;
                    inclinationTengeantAddForward += forwardProj;
                    inclinationTengeantAddRight += rightProj;
                }
                inclinationNormalAdd.Normalize();
                inclinationTengeantAddForward.Normalize();
                inclinationTengeantAddRight.Normalize();
                //Debug.Log ("inclinationNormalAdd tot=" + inclinationNormalAdd);
                //Debug.Log ("inclinationTengeantAddForward tot=" + inclinationTengeantAddForward);
                //Debug.Log ("inclinationTengeantAddRight tot=" + inclinationTengeantAddRight);

                scalar_forward += Vector3.Dot(inclinationNormalAdd, inclinationTengeantAddForward);
                scalar_right += Vector3.Dot(inclinationNormalAdd, inclinationTengeantAddRight);
                //Debug.Log("scalar1 tot=" + scalar_forward);
                //Debug.Log ("scalar2 tot=" + scalar_right);

                if (scalar_forward < -0.5f) {
                    //factor_forward = Mathf.Round ((1 + scalar_forward) * 100) / 100.0f;
                    factor_forward = 0;
                }

                if (scalar_right < -0.5f) {
                    //factor_right = Mathf.Round((1 + scalar_right) * 100) / 100.0f;
                    factor_right = 0;
                }

                //factor_forward = Mathf.Round ((1 - Mathf.Abs(scalar_forward)) * 100) / 100.0f;
                //factor_right = Mathf.Round((1 - Mathf.Abs(scalar_right)) * 100) / 100.0f;
            }
            else if (inclinationNormals.Count == 1) {
                scalar_forward = Vector3.Dot(inclinationNormals[0], forwardNoY.normalized);
                scalar_right = Vector3.Dot(inclinationNormals[0], rightNoY.normalized);
                //Debug.Log("scalar3 tot=" + scalar_forward);
                //Debug.Log ("scalar4 tot=" + scalar_right);

                if (scalar_forward < 0) {
                    factor_forward = Mathf.Round((1 + scalar_forward) * 100) / 100.0f;
                }
                if (scalar_right < 0) {
                    factor_right = Mathf.Round((1 + scalar_right) * 100) / 100.0f;
                }
                //Debug.Log ("factor_forward tot=" + factor_forward);
                //Debug.Log ("factor_right tot=" + factor_right);
            }

            //Debug.Log ("forwardNoY * factor_forward tot1=" + forwardNoY * factor_forward);
            //Debug.Log ("rightNoY * factor_right tot1=" + rightNoY * factor_right);

            forwardNoY = new Vector3(
                Mathf.Round(forwardNoY.x * factor_forward * 10) / 10.0f,
                Mathf.Round(forwardNoY.y * factor_forward * 10) / 10.0f,
                Mathf.Round(forwardNoY.z * factor_forward * 10) / 10.0f
            );
            rightNoY = new Vector3(
                Mathf.Round(rightNoY.x * factor_right * 10) / 10.0f,
                Mathf.Round(rightNoY.y * factor_right * 10) / 10.0f,
                Mathf.Round(rightNoY.z * factor_right * 10) / 10.0f
            );
            //Debug.Log ("forwardNoY * factor_forward tot2=" + forwardNoY);
            //Debug.Log ("rightNoY * factor_right tot2=" + rightNoY);

            inputVelocityAxis =
            (
                rightNoY +
                forwardNoY
            ) * moveSpeed;
            //Debug.Log ("direction.forward=" + direction.forward);
            //Debug.Log ("forward=" + forwardNoY);
            //Debug.Log("inputVelocityAxis=" + inputVelocityAxis);
        }
    }

    private void ApplyMovement() {
        //transform.position += inputVelocityAxis * Time.deltaTime * ratio;
        transform.position += inputVelocityAxis * Time.fixedDeltaTime;

        //Orientation du personnage
        orientationMove = (direction.forward * moveStateParameters.moveZ) + (direction.right * moveStateParameters.moveX);
        //Move player on direction based on camera
        if (moveStateParameters.moveX != 0 || moveStateParameters.moveZ != 0) {
            transform.rotation = Quaternion.Euler(0, direction.rotation.eulerAngles.y, 0);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(orientationMove.x, 0f, orientationMove.z));

            angle = newRotation.eulerAngles.y - playerModel.rotation.eulerAngles.y;
            if (angle > 180)
                angle -= 360;
            else if (angle < -180)
                angle += 360;

            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, newRotation, rotateSpeed * Time.fixedDeltaTime);
        }
        //Debug.Log ("inclinationNormals.Count=" + inclinationNormals.Count + " " + Time.time);
        direction.up = Vector3.up;
        if (Camera.main != null) {
            direction.rotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
        }
        if (allowedToWalk) {
            /*float angleX = Mathf.Abs(gO.transform.rotation.eulerAngles.x);
            if (angleX > 180)
                angleX = angleX % 360 - 360;
            float angleZ = Mathf.Abs(gO.transform.rotation.eulerAngles.z);
            if (angleZ > 180)
                angleZ = angleZ % 360 - 360;
            direction.RotateAround(direction.position, Vector3.right, angleX % 90);
            direction.RotateAround(direction.position, Vector3.forward, angleZ % 90);*/

            direction.forward = Vector3.ProjectOnPlane(direction.forward, normal_contact).normalized;
            float angle_normal = Vector3.SignedAngle(direction.up, normal_contact, direction.forward);
            direction.RotateAround(direction.position, direction.forward, angle_normal);
        }

        // reset variables
        if (IsGrounded()) {
            allowedToWalk = false;
        }
        else {
            allowedToWalk = true;
        }
        //inclinationNormal = Vector3.zero;
        inclinationNormals.Clear();
        //direction.rotation = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up);
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
                        //================================================
                        SoundController.instance.StopKoda();
                        //================================================
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
                if (previousInteractState == InteractState.Absorb) {
                    interactBehaviorCtrl.StopAbsorption();
                }
                if (interactStateParameter.finishedCarry && previousInteractState == InteractState.Carry) {
                    interactBehaviorCtrl.StopCarry(catchableObject, inputVelocityAxis);
                    leafLock.parent = InteractState.Nothing;
                }

                break;

            case InteractState.Glide:
                if (IsGrounded()) {
                    interactBehaviorCtrl.StopGlide();
                    leafLock.isUsed = false;
                    leafLock.parent = InteractState.Nothing;
                    //================================================
                    SoundController.instance.StopKoda();
                    //================================================
                }
                else {
                    if (!leafLock.isUsed || (leafLock.isUsed && leafLock.parent == InteractState.Glide)) {
                        // add a force to counter gravity (glide effect)

                        //body.AddForce(Vector3.up * glideCounterForce, ForceMode.Force);
                        body.AddForce(Vector3.up * (glideCounterForce / body.mass) * ratio, ForceMode.Acceleration);
                        if (body.velocity.y < -9) {
                            body.velocity = new Vector3(body.velocity.x, -9, body.velocity.z);
                        }
                        interactBehaviorCtrl.DoGlide();
                        if (interactStateParameter.canAirStream) {
                            body.AddForce((Vector3.up * (airStreamForce / body.mass) + (Vector3.up * Mathf.Abs(body.velocity.y)) * ratio), ForceMode.Acceleration);
                            if (body.velocity.y > 8.0f) {
                                //Debug.Log ("STOP AIRSTREAM!!!");
                                // force the velocity to 0.02f (near 0) in order to reset the Y velocity (for better jump)
                                body.velocity = new Vector3(body.velocity.x, 8.0f, body.velocity.z);
                            }
                        }

                        if (previousInteractState != InteractState.Glide) {
                            //================================================
                            SoundController.instance.SelectLEAF("UnfoldGlide");
                            SoundController.instance.SelectKODA("Glide");
                            //================================================
                        }
                        leafLock.isUsed = true;
                        leafLock.parent = InteractState.Glide;
                    }
                }
                break;

            case InteractState.MeleeAttack:
                if (interactStateParameter.canMeleeAttack && !leafLock.isUsed) {
                    interactBehaviorCtrl.DoMeleeAttack();
                    //================================================
                    SoundController.instance.SelectLEAF("CloseCombat");
                    //================================================
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
                    _grounds.Remove(actualLure);
                    interactBehaviorCtrl.DestroyLure(actualLure);
                    _grounds.Remove(actualLure);
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
                GameObject nearestObject = detectNearInteractObject.GetNearestObject();
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
        //if(inputParams.jumpRequest) Debug.Log ("inputParams.jumpRequest 8 = " + inputParams.jumpRequest + " " + Time.time);
        moveStateParameters.jumpRequired =
            inputParams.jumpRequest &&
            (
                movementState == MovementState.Idle ||
                movementState == MovementState.Run ||
                (
                    !inputParams.hasDoubleJumped &&
                    (
                        temporaryCapacity == Capacity.DoubleJump ||
                        hasPermanentDoubleJumpCapacity
                    ) &&
                    interactState != InteractState.Carry
                )
            );
        moveStateParameters.grounded = IsGrounded();
        //if(inputParams.jumpRequest) Debug.Log ("inputParams.jumpRequest 9 = " + inputParams.jumpRequest + " " + Time.time);
        if (inputParams.jumpRequest) {
            //Debug.Log("movementState=" + movementState);
            //Debug.Log("interactState=" + interactState);
            //Debug.Log("IsGrounded=" + IsGrounded());
            //Debug.Log("moveStateParameters.jumpRequired1=" + moveStateParameters.jumpRequired);
            inputParams.jumpRequest = false;
            inputController.SetUserRequest(inputParams);
        }
        //if(inputParams.jumpRequest) Debug.Log ("inputParams.jumpRequest 10 = " + inputParams.jumpRequest + " " + Time.time);
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
                if (actualLure == null && !leafLock.isUsed && (hasPermanentLureCapacity || temporaryCapacity == Capacity.Lure)) {
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
                GameObject nearestObject = detectNearInteractObject.GetNearestObject();

                if (interactState == InteractState.Carry) {
                    interactStateParameter.finishedCarry = true;
                }
                else if (nearestObject != null) {
                    bool inFrontOfActivableObject = false;
                    bool inFrontOfAbsorbableObject = false;
                    bool inFrontOfPortableObject = false;

                    if (nearestObject.CompareTag("Yokai") && nearestObject.GetComponent<YokaiController>().GetIsKnocked()) {
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
                        else if (inFrontOfAbsorbableObject && !inputParams.absorptionInterrupted) {
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
            inputParams.absorptionInterrupted = false;
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
                //================================================
                SoundController.instance.SelectENVQuick("AirStream");
                //================================================
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
            //================================================
            SoundController.instance.StopEnvironnementEffectLong();
            //================================================
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
        else if (collid.CompareTag("Yokai") && interactState == InteractState.Absorb && interactStateParameter.canAbsorb) {
            if (previousInteractState != InteractState.Absorb) {

                interactBehaviorCtrl.DoBeginAbsorption(collid.gameObject);

            }
            else {
                Pair<Capacity, float> pairCapacity = interactBehaviorCtrl.DoContinueAbsorption(collid.gameObject, inputController);
                if (pairCapacity.First != Capacity.Nothing) {
                    AddCapacity(pairCapacity);
                }
            }
        }
        else if (collid.CompareTag("LoveHotel")) {
            InputParams inputParams = inputController.RetrieveUserRequest();
            if (inputParams.contextualButtonPressed && interactState == InteractState.Activate) {
                LanternStandController stand = collid.GetComponent<LanternStandController>();
                stand.RecallLantern();
                inputParams.contextualButtonPressed = false;
                inputController.SetUserRequest(inputParams);
            }
        }
    }

    //Temporary in public mode for the playtest
    public void AddCapacity(Pair<Capacity, float> pairCapacity) {
        if (pairCapacity.First != Capacity.Nothing && pairCapacity.Second == 0) {
            switch (pairCapacity.First) {

                case Capacity.DoubleJump:
                    hasPermanentDoubleJumpCapacity = true;
                    break;

                case Capacity.Lure:
                    hasPermanentLureCapacity = true;
                    break;
            }
        }
        else {
            temporaryCapacity = pairCapacity.First;
            canvasQTE.SetActive(true);
            switch (pairCapacity.First) {

                case Capacity.DoubleJump:
                    doubleJump_Logo.SetActive(true);
                    SetPowerUpMaterials(1);
                    break;

                case Capacity.Lure:
                    spawnLure_Logo.SetActive(true);
                    SetPowerUpMaterials(2);
                    break;
            }
        }


        timerCapacity = pairCapacity.Second;
        maxPowerUpGauge = pairCapacity.Second;
    }

    private void SetPowerUpMaterials(int texId) {
        float intensity = (texId == 0) ? 0 : 1;
        if(texId > 0) {
            mats[0].SetColor("_EmissiveColor", colors[texId-1]);
            mats[0].SetTexture("_FirstTexture", textures[texId-1]);
        }
        StartCoroutine(UpdateEmissiveMaterial(mats[0], intensity));
    }

    IEnumerator UpdateEmissiveMaterial(Material mat, float target) {
        while((target == 0 ? -1 : 1)*mat.GetFloat("_EmissiveIntensity") < target) {
            float value = mat.GetFloat("_EmissiveIntensity") +lerpBtwColor * (target == 0 ? -1 : 1);
            mat.SetFloat("_EmissiveIntensity" , Mathf.Clamp01(value));
            yield return new WaitForSeconds(lerpBtwColor);
        }
    }

    IEnumerator UpdateInvincibilityMaterial(Material mat, float target) {
        while((target == 0 ? -1 : 1)*mat.GetColor("_InvincibilityColor").a < target) {
            Color color = mat.GetColor("_InvincibilityColor");
            color.a = Mathf.Clamp01(color.a + lerpBtwColor * (target == 0 ? -1 :1));
            mat.SetColor("_InvincibilityColor" , color);
            yield return new WaitForSeconds(lerpBtwColor);
        }
    }

    public void SetInvincibility(bool toggle) {
        for (int i = 0; i < mats.Length; i++) {
            StartCoroutine(UpdateInvincibilityMaterial(mats[i], toggle ? 1 : 0));
        }
    }

    private void StopTemporaryCapacity() {
        timerCapacity = 0;

        SetPowerUpMaterials(0);
        if (temporaryCapacity == Capacity.DoubleJump) { doubleJump_Logo.SetActive(false); }
        if (temporaryCapacity == Capacity.Lure) {
            interactBehaviorCtrl.DestroyLure(actualLure);
            ResetLeafLock();
            spawnLure_Logo.SetActive(false);
        }

        temporaryCapacity = Capacity.Nothing;
        canvasQTE.SetActive(false);
    }

    private void ProgressTimerCapacity() {
        loadingBar.fillAmount = timerCapacity / maxPowerUpGauge;
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

    //Use when AirStreamLantern is Destroy and player is actualy gliding in.
    public void PlayerOutAirstream() {
        //================================================
        //SoundController.instance.StopEnvironnementEffectLong();
        //================================================
        moveStateParameters.inAirStream = false;
        interactStateParameter.canAirStream = false;
    }

    public bool GetPowerJump() { return hasPermanentDoubleJumpCapacity; }
    public bool GetPowerLure() { return hasPermanentLureCapacity; }
    public bool GetPowerBall() { return hasPermanentBallCapacity; }
    public bool GetPowerShrink() { return hasPermanentShrinkCapacity; }
    public Transform GetRespawnPointPosition() { return gameObject.transform; }

    public void SetPowerJump(bool has_double_jump) { hasPermanentDoubleJumpCapacity = has_double_jump; }
    public void SetPowerLure(bool has_lure) { hasPermanentLureCapacity = has_lure; }
    public void SetPowerBall(bool has_power_ball) { hasPermanentBallCapacity = has_power_ball; }
    public void SetPowerShrink(bool has_power_shrink) { hasPermanentShrinkCapacity = has_power_shrink; }
    public void SetRespawnPointPosition(float x_pos, float y_pos, float z_pos) { body.position = new Vector3(x_pos, y_pos, z_pos); }
}
