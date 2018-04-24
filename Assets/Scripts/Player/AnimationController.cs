using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour, /*IInterractState,*/ IMovementStateReceiver {

    public GameObject landingParticlePrefab;

    public Transform runParticleTransform;
    private ParticleSystem.EmissionModule emissionRun;

    public GameObject pawsProjectorPrefab;
    public Transform leftFoot;
    public Transform rightFoot;

    private Animator animBody;

    public Transform playerTransform;

    public Transform glideTransform;

    private float smoothVertSpeed;

    void Start() {
        animBody = GetComponent<Animator>();
        emissionRun = runParticleTransform.gameObject.GetComponent<ParticleSystem>().emission;
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
                glideTransform.gameObject.SetActive(false);
                break;
            case MovementState.DoubleJump:
                animBody.SetTrigger("DoubleJump");
                emissionRun.enabled = false;
                break;
            case MovementState.Idle:
                animBody.SetBool("isInAir", false);
                emissionRun.enabled = true;
                break;
            case MovementState.Run:
                animBody.SetBool("isInAir", false);
                emissionRun.enabled = true;
                glideTransform.gameObject.SetActive(false);
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
                glideTransform.gameObject.SetActive(true);
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
                break;

            case InteractState.Nothing:
                break;
        }
    }

    public void OnStateExit(InteractState state) {
        switch (state) {
            case InteractState.Glide:
                animBody.SetBool("isGliding", false);
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
                break;
        }
    }

	public void Landing() {
		Instantiate(landingParticlePrefab, new Vector3(transform.position.x, transform.position.y+.5f, transform.position.z), Quaternion.identity);
	}

    public void CreatePaw(string str) {
        Transform tr = str.Equals("Left") ? leftFoot :  rightFoot;
        Instantiate(pawsProjectorPrefab, tr.position, playerTransform.rotation);
    }
}