using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private float speed;
    public float moveSpeed;
    public float movespeedWithObject;
    public float jumpForce;
    public float gravityScale;
    public Rigidbody body;

    private Animator animBody;
    private bool _onGround = false;
    private bool isJumping;
    private bool isDoubleJumping;
    private int nbJump = 0;
    private bool canDoubleJump;
    [SerializeField]
    private GameObject ParachuteLeaf;
    [SerializeField]
    private GameObject leafHead;

    private Vector3 velocityAxis;
    private Vector3 orientationMove;
    private Vector3 airVelocity;

    public Transform pivot;
    public float rotateSpeed;
    public GameObject playerModel;


    // Use this for initialization
    void Start() {
        body = GetComponent<Rigidbody>();
        animBody = GetComponent<Animator>();
        ParachuteLeaf.SetActive(false);
        canDoubleJump = true;
    }

    // Update is called once per frame
    void Update() {

        speed = moveSpeed;

        if (GameObject.Find("Catchable Object").transform.childCount != 0) {
            speed = movespeedWithObject;
        }

        _onGround = CheckGroundCollision();

        //GLIDE ON
        if (Input.GetButton("Jump") && isJumping && !_onGround && body.velocity.y < 0 && canDoubleJump) {
            nbJump++;
            leafHead.SetActive(false);
            body.velocity = new Vector3(0, -1.5f, 0);
            animBody.SetBool("isGliding", true);
            ParachuteLeaf.SetActive(true);
        }
        //GLIDE OFF
        if (Input.GetButtonUp("Jump") && isJumping && !_onGround) {
            ParachuteLeaf.SetActive(false);
            animBody.SetBool("isGliding", false);
            leafHead.SetActive(true);
        }

        //DOUBLE JUMP
        if (Input.GetButtonDown("Jump") && isJumping && nbJump < 1 && canDoubleJump) {
            nbJump++;
            body.velocity = new Vector3(0, jumpForce, 0);
            animBody.SetBool("isDoubleJumping", true);
        }
        //JUMP
        if (Input.GetButtonDown("Jump") && _onGround) {
            airVelocity = velocityAxis;
            body.velocity = new Vector3(0, jumpForce, 0);
            isJumping = true;
            animBody.SetBool("isJumping", true);
        }
        //LAND
        if (_onGround && body.velocity.y < 0) {
            isJumping = false;
            nbJump = 0;
            ParachuteLeaf.SetActive(false);
            animBody.SetBool("isDoubleJumping", false);
            animBody.SetBool("isJumping", false);
        }

        //DEATH
        if (Input.GetKeyDown("c")) {
            animBody.SetBool("isDead", true);
        }

        if (this.animBody.GetCurrentAnimatorStateInfo(0).IsName("Dead")) {
            animBody.SetBool("isDead", false);
        }

        //Debug.Log ("sol ?" + _onGround);

        //Deplacement du personnage
        velocityAxis = new Vector3(Input.GetAxis("Horizontal"), body.velocity.y, Input.GetAxis("Vertical"));
        //Orientation du personnage
        orientationMove = (transform.forward * Input.GetAxis("Vertical")) + (transform.right * Input.GetAxis("Horizontal"));
        velocityAxis = velocityAxis.normalized * speed;
        velocityAxis.y = body.velocity.y;

        animBody.SetFloat("Speed", Mathf.Abs(velocityAxis.x) + Mathf.Abs(velocityAxis.z));
        animBody.SetFloat("SpeedMultiplier", speed);


        //AIR CONTROL
        if (!_onGround) {
            ////airVelocity = new Vector3(Input.GetAxis("Horizontal") * airVelocity.x, velocityAxis.y, Input.GetAxis("Vertical") * airVelocity.z);
            //if(airVelocity.x > 10) {

            //}
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * new Vector3((Input.GetAxis("Horizontal") * 6), velocityAxis.y, (Input.GetAxis("Vertical") * 12));
            //body.velocity = Quaternion.AngleAxis(pivot.transform.eulerAngles.y, Vector3.up) * new Vector3(velocityAxis.x*2, velocityAxis.y, velocityAxis.z*2);
        }
        else {
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * velocityAxis;
        }

        //Move player on direction based on camera
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
            transform.rotation = Quaternion.Euler(0, pivot.rotation.eulerAngles.y, 0);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(orientationMove.x, 0f, orientationMove.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }


    }



    private bool CheckGroundCollision() {
        // We can use a layer mask to tell the Physics Raycast which layers we are trying to hit.
        // This will allow us to restrict which objects this applies to.
        int layerMask = 1 << LayerMask.NameToLayer("Ground");

        // We will get the bounds of the MeshFilter (our player's sphere) so we can
        // get the coordinates of where the bottom is.
        Bounds meshBounds = GetComponent<MeshFilter>().mesh.bounds;

        // We will use a Physics.Raycast to see if there is anything on the ground below the player.
        // We can limit the distance to make sure that we are touching the bottom of the collider.
        if (Physics.Raycast(transform.position + meshBounds.center, Vector3.down, meshBounds.extents.y, layerMask)) {
            return true;
        }
        return false;
    }

    public void SetIsJumping(bool isJump) {
        isJumping = isJump;
        nbJump++;
        animBody.SetBool("isJumping", true);
    }

    public bool CanDoubleJump {
        get {
            return canDoubleJump;
        }

        set {
            canDoubleJump = value;
        }
    }

}
