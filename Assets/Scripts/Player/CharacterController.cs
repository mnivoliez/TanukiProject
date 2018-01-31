using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    [Header("PLAYER")]
    [Space(10)]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float airControl = 6f;
    [SerializeField] private float jumpForce = 12f;
    

    //Orientation Camera player
    [Header("CAMERA")]
    [Space(10)]
    public Transform pivot;
    public float rotateSpeed;
    private Vector3 orientationMove;
    public GameObject playerModel;
    private Vector3 inputVelocityAxis;

    private Rigidbody body;
    private State currentState;
    private bool onGround;

    //Animation
    private Animator animBody;

    private void Start() {
        SetState(new IdleState(this));
        body = GetComponent<Rigidbody>();
        animBody = GetComponent<Animator>();
    }

    private void Update() {
        
        onGround = CheckGroundCollision();
        UpdateInput();
        currentState.Tick();
    }

    public void SetState(State state) {
        if (currentState != null)
            currentState.OnStateExit();

        currentState = state;

        if (currentState != null)
            currentState.OnStateEnter();
    }

    private bool CheckGroundCollision() {
        // We can use a layer mask to tell the Physics Raycast which layers we are trying to hit.
        // This will allow us to restrict which objects this applies to.
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        Bounds meshBounds = GetComponent<MeshFilter>().mesh.bounds;

        // We will use a Physics.Raycast to see if there is anything on the ground below the player.
        // We can limit the distance to make sure that we are touching the bottom of the collider.
        if (Physics.Raycast(transform.position + meshBounds.center, Vector3.down, meshBounds.extents.y, layerMask)) {
            return true;
        }
        return false;
    }

    void UpdateInput() {

        //JUMP
        if (Input.GetButtonDown("Jump") && onGround) {
            body.velocity = new Vector3(0, jumpForce, 0);
            animBody.SetBool("isJumping", true);
        }

        //Deplacement du personnage
        inputVelocityAxis = new Vector3(Input.GetAxis("Horizontal"), body.velocity.y, Input.GetAxis("Vertical"));
        //Orientation du personnage
        orientationMove = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        inputVelocityAxis = inputVelocityAxis.normalized * moveSpeed;
        inputVelocityAxis.y = body.velocity.y;

        animBody.SetFloat("Speed", Mathf.Abs(inputVelocityAxis.x) + Mathf.Abs(inputVelocityAxis.z));
        animBody.SetFloat("SpeedMultiplier", moveSpeed);


        //AIR CONTROL
        if (!onGround) {
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * new Vector3((Input.GetAxis("Horizontal") * airControl), inputVelocityAxis.y, (Input.GetAxis("Vertical") * (airControl*2)));
        }
        else {
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * inputVelocityAxis;
        }

        //Move player on direction based on camera
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            transform.rotation = Quaternion.Euler(0, pivot.rotation.eulerAngles.y, 0);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(orientationMove.x, 0f, orientationMove.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }

    }

    public bool GetOnGround() {return onGround;}
    public float GetJumpForce() { return jumpForce; }
    public Rigidbody GetBody() { return body; }
    public Animator GetAnimbody() { return animBody; }

}
