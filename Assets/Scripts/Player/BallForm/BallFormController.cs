using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFormController : MonoBehaviour {

    public float movespeed;
    public float jumpForce;
    public float maxSpeed;
    public float gravityScale;
    public Rigidbody body;


    private bool _onGround = false;
    private bool isJumping;
    private int nbJump = 0;


    // Use this for initialization
    void Start() {
        body = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update() {

        _onGround = CheckGroundCollision();

        //JUMP
        if (Input.GetButtonDown("Jump") && _onGround) {
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            //animBody.SetBool("isJumping", true);
        }
        //LAND
        if (_onGround && body.velocity.y < 0) {
            isJumping = false;
            nbJump = 0;
            //animBody.SetBool("isDoubleJumping", false);
            //animBody.SetBool("isJumping", false);
        }

        Vector3 velocityAxis;
        velocityAxis = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0) {
            body.AddForce(-body.velocity / 2, ForceMode.Impulse);
        }



        if (body.velocity.magnitude < maxSpeed) {
            body.AddForce(velocityAxis * movespeed, ForceMode.Impulse);
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
        if (Physics.Raycast(transform.position + meshBounds.center, Vector3.down, meshBounds.extents.y + 0.5f, layerMask)) {
            return true;
        }

        return false;
    }

}
