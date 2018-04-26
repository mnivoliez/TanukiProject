using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour, /*IInterractState,*/ IMovementStateReceiver {

    public GameObject landingParticlePrefab;

    public Transform runParticleTransform;
    private ParticleSystem.EmissionModule emissionRun;

    public GameObject pawsProjectorPrefab;
    public GameObject waterWavePrefab;
    public Transform leftFoot;
    public Transform rightFoot;

    private Animator animBody;

    public Transform playerTransform;

    public Transform glideTransform;
    private ParticleSystem.EmissionModule emissionGlide;

    public Transform absorbTransform;
    private ParticleSystem.EmissionModule emissionAbsorb;

    private bool isRunningOnWater = false;

    private float smoothVertSpeed;

    void Start() {
        animBody = GetComponent<Animator>();
        emissionRun = runParticleTransform.gameObject.GetComponent<ParticleSystem>().emission;
        emissionGlide = glideTransform.gameObject.GetComponent<ParticleSystem>().emission;
        emissionAbsorb = absorbTransform.gameObject.GetComponent<ParticleSystem>().emission;
    }

    public void UpdateState(MovementState state, float speed, float lateralBend, float verticalSpeed) {
        animBody.SetFloat("Speed", speed);

        animBody.SetFloat("LateralBend", lateralBend);

        smoothVertSpeed = Mathf.Lerp(smoothVertSpeed, verticalSpeed, .3f);
        animBody.SetFloat("VerticalSpeed", smoothVertSpeed);
    }

    public void OnStateEnter(MovementState state) {
        switch (state) {
            case MovementState.Jump:
                animBody.SetBool("isInAir", true);
                emissionRun.enabled = false;
                break;
            case MovementState.PushUp:
                animBody.SetBool("isInAir", true);
                emissionRun.enabled = false;
                break;
            case MovementState.Fall:
                animBody.SetBool("isInAir", true);
                emissionRun.enabled = false;
                break;
            case MovementState.DoubleJump:
                animBody.SetTrigger("DoubleJump");
                emissionRun.enabled = false;
                break;
            case MovementState.Idle:
                animBody.SetBool("isInAir", false);
                emissionRun.enabled = false;
                break;
            case MovementState.Run:
                animBody.SetBool("isInAir", false);
                emissionRun.enabled = true;
                break;
        }
    }

    public void OnStateExit(MovementState state) {
        switch (state) {
            case MovementState.Jump:
                break;

            case MovementState.DoubleJump:
                break;
        }
    }

    public void OnStateEnter(InteractState state) {
        switch (state) {

            case InteractState.Glide:
                animBody.SetBool("isGliding", true);
                emissionGlide.enabled = true;
                break;

            case InteractState.MeleeAttack:
                animBody.SetTrigger("InstantAttack");
                break;

            case InteractState.DistantAttack:
                animBody.SetTrigger("DistantAttack");
                break;

            case InteractState.SpawnLure:
                //animBody.SetBool("isAttacking", true);
                break;

            case InteractState.Inflate:
                //animBody.SetBool("isAttacking", true);
                break;

            case InteractState.Tiny:
                //animBody.SetBool("isAttacking", true);
                break;

            case InteractState.Activate:
                //animBody.SetBool("isAttacking", true);
                break;

            case InteractState.Absorb:
                animBody.SetBool("IsAbsorbing", true);
                emissionAbsorb.enabled = true;
                AbsorbRotation.Toggle(true);
                break;

            case InteractState.Nothing:
                break;
        }
    }

    public void OnStateExit(InteractState state) {
        switch (state) {
            case InteractState.Glide:
                animBody.SetBool("isGliding", false);
                emissionGlide.enabled = false;
                break;

            case InteractState.MeleeAttack:
                //animBody.SetBool("isAttacking", false);
                break;

            case InteractState.DistantAttack:
                //animBody.SetBool("isDistantAttacking", false);
                break;

            case InteractState.SpawnLure:
                //animBody.SetBool("isAttacking", false);
                break;

            case InteractState.Inflate:
                //animBody.SetBool("isAttacking", false);
                break;

            case InteractState.Tiny:
                //animBody.SetBool("isAttacking", false);
                break;

            case InteractState.Activate:
                //animBody.SetBool("isAttacking", false);
                break;

            case InteractState.Absorb:
                animBody.SetBool("IsAbsorbing", false);
                emissionAbsorb.enabled = false;
                AbsorbRotation.Toggle(false);
                break;
        }
    }

	public void Landing() {
        Vector3 offsetDown = new Vector3(transform.position.x, transform.position.y+.1f, transform.position.z);
        if(Physics.Raycast(offsetDown, Vector3.down, .15f, 1<<4)) {
            GameObject go = Instantiate(waterWavePrefab, offsetDown, playerTransform.rotation) as GameObject;
            go.transform.localScale = new Vector3(3,3,3);
        } else {
		    Instantiate(landingParticlePrefab, new Vector3(transform.position.x, transform.position.y+.5f, transform.position.z), Quaternion.identity);
        }
	}

    public void FootGrounded(string footStr) {
        Transform tr = footStr.Equals("Left") ? leftFoot :  rightFoot;
        Vector3 offsetUp = new Vector3(tr.position.x, tr.position.y+.2f, tr.position.z);
        RaycastHit hit;
        if(Physics.Raycast(offsetUp, Vector3.down, out hit, .25f, 1<<4)) {
            Instantiate(waterWavePrefab, new Vector3(hit.point.x, hit.point.y+.25f, hit.point.z), playerTransform.rotation);
            emissionRun.enabled = false;
        } else if(!animBody.GetBool("isInAir")){
            Instantiate(pawsProjectorPrefab, tr.position, playerTransform.rotation);
            emissionRun.enabled = true;
        }
    }

}