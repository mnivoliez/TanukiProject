using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	
	public float movespeed;
	public float jumpForce;
	public float gravityScale;
	public Rigidbody body;

	private Animator animBody;
	private bool _onGround = false;
    private bool isJumping;
    private bool isDoubleJumping;
    private int nbJump = 0;
    [SerializeField]
    private GameObject ParachuteLeaf;

    private Vector3 velocityAxis;
    private Vector3 airVelocity;

    // Use this for initialization
    void Start () {
		body = GetComponent<Rigidbody>();
		animBody = GetComponent <Animator> ();
        ParachuteLeaf.SetActive(false);
        
    }
	
	// Update is called once per frame
	void Update () {

		_onGround = CheckGroundCollision();

        //GLIDE
        if (Input.GetButton("Jump") && isJumping && !_onGround && body.velocity.y < 0) {
            
            body.velocity = new Vector3(0, -1.5f, 0);
            animBody.SetBool("isGliding", true);
            ParachuteLeaf.SetActive(true);
        }
        //Glide
        if (Input.GetButtonUp("Jump") && isJumping && !_onGround) {
            ParachuteLeaf.SetActive(false);
            animBody.SetBool("isGliding", false);
        }

        //DOUBLE JUMP
        if (Input.GetButtonDown("Jump") && isJumping && nbJump < 1) {
            nbJump++;
            body.velocity = new Vector3(0, jumpForce, 0);
            animBody.SetBool("isDoubleJumping", true);
        }
        //JUMP
        if (Input.GetButtonDown("Jump") && _onGround){
            airVelocity = velocityAxis;
            body.velocity = new Vector3 (0, jumpForce, 0);
            isJumping = true;
            animBody.SetBool("isJumping", true);
		}
        //LAND
        if (_onGround && body.velocity.y < 0){
            isJumping = false;
            nbJump = 0;
            ParachuteLeaf.SetActive(false);
            animBody.SetBool("isDoubleJumping", false);
            animBody.SetBool("isJumping", false);
		}

        //DEATH
        if (Input.GetKeyDown("c"))
        {
            animBody.SetBool("isDead", true);
        }

        if (this.animBody.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            animBody.SetBool("isDead", false);
        }

        //Debug.Log ("sol ?" + _onGround);
        velocityAxis = new Vector3 (Input.GetAxis("Horizontal") * movespeed, body.velocity.y, Input.GetAxis("Vertical") * movespeed);
		animBody.SetFloat("Speed", Mathf.Abs(velocityAxis.x)+ Mathf.Abs(velocityAxis.z));
        if ((Mathf.Abs(velocityAxis.x) + Mathf.Abs(velocityAxis.z)) < movespeed) {
            animBody.SetFloat("SpeedMultiplier", Mathf.Abs(velocityAxis.x) + Mathf.Abs(velocityAxis.z));
        }
        else {
            animBody.SetFloat("SpeedMultiplier", movespeed);
        }
        

        if (this.animBody.GetCurrentAnimatorStateInfo(0).IsName("Run")) {
           // this.animBody.GetCurrentAnimatorStateInfo(0).speed = (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical") / 2);
        }

        //		// Rotate the player's model to show direction
        //		if (velocityAxis.magnitude > 0.2f) {
        //			transform.rotation = Quaternion.LookRotation (velocityAxis);
        //		}

        if (!_onGround) {
            //airVelocity = new Vector3(Input.GetAxis("Horizontal") * airVelocity.x, velocityAxis.y, Input.GetAxis("Vertical") * airVelocity.z);
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * new Vector3((Input.GetAxis("Horizontal")*4) + airVelocity.x, velocityAxis.y, (Input.GetAxis("Vertical")*4) + airVelocity.z);
        }
        else {
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * velocityAxis;
        }
	}
		

    
	private bool CheckGroundCollision(){
		// We can use a layer mask to tell the Physics Raycast which layers we are trying to hit.
		// This will allow us to restrict which objects this applies to.
		int layerMask = 1 << LayerMask.NameToLayer("Ground");

		// We will get the bounds of the MeshFilter (our player's sphere) so we can
		// get the coordinates of where the bottom is.
		Bounds meshBounds = GetComponent<MeshFilter>().mesh.bounds;

		// We will use a Physics.Raycast to see if there is anything on the ground below the player.
		// We can limit the distance to make sure that we are touching the bottom of the collider.
		if (Physics.Raycast(transform.position+meshBounds.center,Vector3.down,meshBounds.extents.y,layerMask)){
			return true;
		}
		return false;
	}
}
