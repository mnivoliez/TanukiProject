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

public class CharacterController : MonoBehaviour {

    [Header("PLAYER")]
    [Space(10)]
    [SerializeField]
    private float moveSpeed = 10f;
    private float speed = 10f;
    private float coefInclination;
	[SerializeField] private float airControl = 12f;
	[SerializeField] private float jumpForce = 120f;
	[SerializeField] private float airStreamForce = 200f;
	[SerializeField] private float glideCounterForce = 150f;
    [SerializeField] private GameObject absorbRange;
    private GameObject catchableObject;
    private GameObject objectToCarry;
    private GameObject actualLure;

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
	[SerializeField] private bool permanentDoubleJumpCapacity;
	[SerializeField] private bool temporaryDoubleJumpCapacity;
    [SerializeField] private float timerCapacity;

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
        if (timerCapacity > 0) {
            timerCapacity -= Time.deltaTime;
        } else {
            StopTemporaryCapacity();
        }

        onGround = IsGrounded();
        previousMovementState = movementState;

        InputParams inputParams = inputController.RetrieveUserRequest();
        //deplacements

        previousInteractState = interactState;

		UpdateMoveStateParameters(inputParams);
		UpdateInteractStateParameters(inputParams);


        movementState = moveStateCtrl.GetNewState(movementState, moveStateParameters);
        interactState = InteractStateCtrl.GetNewState(interactState, interactStateParameter);

        MoveAccordingToInput();
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

        animBody.UpdateState(movementState, speed, moveSpeed);
    }

    void OnCollisionEnter(Collision coll) {
        GameObject gO = coll.gameObject;

		//Debug.Log ("gO.layer enter=" + LayerMask.LayerToName(gO.layer));
		//Debug.Log ("_grounds.count enter=" + _grounds.Count);
        if (gO.layer == LayerMask.NameToLayer("Ground")) {
            ContactPoint[] contacts = coll.contacts;

			//Debug.Log ("contacts.Length=" + contacts.Length);
            if (contacts.Length > 0) {
                foreach (ContactPoint c in contacts) {
                    // c.normal.y = 0 => Vertical
                    // c.normal.y = 0.5 => 45°
					// c.normal.y = 1 => Horizontal
					//Debug.Log ("c.normal.y=" + c.normal.y);
					if (c.normal.y >= 0.50f && c.normal.y < 1.01f && !_grounds.Contains (gO)) {
                        _grounds.Add(gO);
                        break;
                    }
                }
            }
        }
    }

    void OnCollisionStay(Collision coll) {
        GameObject gO = coll.gameObject;
        if (gO.layer == LayerMask.NameToLayer("Ground")) {
            ContactPoint[] contacts = coll.contacts;

            if (contacts.Length > 0) {
                //transform.rotation = Quaternion.Euler(Vector3.Angle(contacts[0].normal, Vector3.up), Vector3.Angle(contacts[0].normal, Vector3.up), Vector3.Angle(contacts[0].normal, Vector3.up));
                //Debug.Log("Angle:"+Vector3.Angle(contacts[0].normal, Vector3.up));
                //coefInclination = Vector3.Angle(contacts[0].normal, Vector3.up);
				bool found = false;
                foreach (ContactPoint c in contacts) {
                    // c.normal.y = 0 => Vertical
                    // c.normal.y = 0.5 => 45°
                    // c.normal.y = 1 => Horizontal
					if ((c.normal.y >= 0.5f && c.normal.y <= 1f) || !(c.normal == null))
					{
						//_grounds.Add(gO);
						found = true;
						coefInclination = Vector3.Angle (c.normal, Vector3.up);
						break;
					}
                }
				if (!found) {
					coefInclination = 0;
				}
            }
		}
    }

    void OnCollisionExit(Collision coll) {
		if (IsGrounded()) {
			GameObject gO = coll.gameObject;
			//Debug.Log ("gO.layer exit=" + LayerMask.LayerToName(gO.layer));
			if (gO.layer == LayerMask.NameToLayer ("Ground"))
			{
				if (_grounds.Contains (gO))
				{
					_grounds.Remove (gO);
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
        return _grounds.Count > 0;
    }

    void MoveAccordingToInput() {
        /* ou si maudit et pas en state jump / fall */
		//JUMP
		if (moveStateParameters.jumpRequired) {
			Debug.Log ("IMPULSE!!!");
			// force the velocity to 0.02f (near 0) in order to reset the Y velocity (for better jump)
			body.velocity = new Vector3(body.velocity.x, 0.02f, body.velocity.z);
			body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}

		inputVelocityAxis = new Vector3(moveStateParameters.moveX, body.velocity.y, moveStateParameters.moveZ);
        //Orientation du personnage
		orientationMove = (transform.forward * moveStateParameters.moveZ) + (transform.right * moveStateParameters.moveX);

		//Manage Inclination Ground
        if (coefInclination <= 45) {
            //old_version
            //inputVelocityAxis = inputVelocityAxis.normalized * moveSpeed + ((inputVelocityAxis.normalized * moveSpeed) * (1 - Mathf.Cos(coefInclination * Mathf.Deg2Rad)));
            //Attention, je ne suis pas tres fiers de ce calcul. C'est tres arbitraire et pifo-metrique. Mais camarche plutot bien. Meh.
            int angle_speed = 2; // Valeur qui va influencer la vitesse engeandree par l'angle d'inclinaison de la plateforme. Plus l'angle est faible (plateforme faiblement inclinee) plus ca ira vite de base. Donc il faut que angle_speed ne soit pas trop eleve.
            int vertical_speed = 4; // Valeur qui va influencer la vitesse ascendante ou descendante reportee sur le plan horistontal en mode barbare. C'est sans doute la valeur la plus intéressante a tweaker a condition que la vitesse ascendante reste interessante.
            //Formule gros merdieren approche. Bon courage. PS : inputVelocityAxis.normalized permet de garder la logique d'orientation de la vitesse à appliquer. C'est un vecteur important.
            inputVelocityAxis = inputVelocityAxis.normalized * moveSpeed + angle_speed * moveSpeed * Mathf.Sin(coefInclination * Mathf.Deg2Rad) * inputVelocityAxis.normalized + vertical_speed * inputVelocityAxis.normalized * Mathf.Abs(body.velocity.y);
            inputVelocityAxis.y = body.velocity.y;

            //Debug.Log(" Z Speed " + inputVelocityAxis.z + " Y Speed " + inputVelocityAxis.y);
            //Debug.Log(" Normalized Normal " + inputVelocityAxis.normalized);
            //Debug.Log(" Angle " + coefInclination + " Cos Deg2Rad " + Mathf.Cos(coefInclination * Mathf.Deg2Rad));
        }
        else {
            //Fall if Ground Inclination > 45 deg
            inputVelocityAxis.y = -5f;
        }

        //AIR CONTROL
        if (!onGround) {
            //body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * new Vector3((moveStateParameters.moveX * airControl), inputVelocityAxis.y, (moveStateParameters.moveZ * (airControl)));
            //float temp_angle = Vector3.Angle(Vector3.right, new Vector3(moveStateParameters.moveX, 0f, moveStateParameters.moveZ));
            float temp_speed = Mathf.Sqrt(Mathf.Pow(moveStateParameters.moveX, 2) + Mathf.Pow(moveStateParameters.moveZ, 2));
            Vector3 temp_vect = new Vector3(moveStateParameters.moveX, 0f, moveStateParameters.moveZ);
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * temp_vect.normalized * temp_speed * airControl + new Vector3(0f, inputVelocityAxis.y, 0f);
        }
        else {
            body.velocity = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * inputVelocityAxis;
        }

        //Move player on direction based on camera
		if (moveStateParameters.moveX != 0 || moveStateParameters.moveZ != 0) {
            transform.rotation = Quaternion.Euler(0, pivot.rotation.eulerAngles.y, 0);
            Quaternion newRotation = Quaternion.LookRotation(new Vector3(orientationMove.x, 0f, orientationMove.z));
            playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, newRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void InteractAccordingToInput() {
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

                /*if (interactStateParameter.canDestroyLure) {
                    interactBehaviorCtrl.DestroyLure(actualLure);
                    actualLure = null;
                }*/
                
                if (interactStateParameter.finishedCarry && previousInteractState == InteractState.Carry) {
                    interactBehaviorCtrl.StopCarry(catchableObject);
                }
                break;

            case InteractState.Glide:
                if (IsGrounded()) {
                    interactBehaviorCtrl.StopGlide();
                } else {
                    // add a force to counter gravity (glide effect)
                                       
					body.AddForce(Vector3.up * glideCounterForce, ForceMode.Force);
                    if (body.velocity.y < -10) {
                        body.velocity = new Vector3(body.velocity.x, -10, body.velocity.z);
                    }
                    interactBehaviorCtrl.DoGlide();
					if (interactStateParameter.canAirStream) {
						body.AddForce (Vector3.up * airStreamForce + (Vector3.up * Mathf.Abs (body.velocity.y)), ForceMode.Force);
						if (body.velocity.y > 8.0f) {
							//Debug.Log ("STOP AIRSTREAM!!!");
							// force the velocity to 0.02f (near 0) in order to reset the Y velocity (for better jump)
							body.velocity = new Vector3(body.velocity.x, 8.0f, body.velocity.z);
						}
					}
                }
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
                if (previousInteractState == InteractState.Nothing) {
                    actualLure = interactBehaviorCtrl.DoSpawnLure();
                }
                break;

            case InteractState.DestroyLure:
                if (previousInteractState == InteractState.Nothing) {
                    interactBehaviorCtrl.DestroyLure(actualLure);
                    actualLure = null;
                }
                break;

            case InteractState.Inflate:
                if (previousInteractState != InteractState.Inflate) {
                    interactBehaviorCtrl.DoInflate(true);
                }
                break;

            case InteractState.Tiny:
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
		if (!IsGoingUp (moveStateParameters) && !IsFalling (moveStateParameters)) {
			moveStateParameters.position_before_fall = body.position;
			if (inputParams.jumpRequest)
			{
				Debug.Log ("NO JUMP!!! YOU ARE MOVING!");
			}
		}
		moveStateParameters.position = body.position;
		moveStateParameters.velocity = body.velocity;
		moveStateParameters.moveX = inputParams.moveX;
		moveStateParameters.moveZ = inputParams.moveZ;
		moveStateParameters.jumpRequired = 
			inputParams.jumpRequest && 
			(	movementState == MovementState.Idle ||
				movementState == MovementState.Run ||
				(movementState == MovementState.Jump && (temporaryDoubleJumpCapacity || permanentDoubleJumpCapacity) && interactState != InteractState.Carry));
		moveStateParameters.grounded = IsGrounded();
		if (inputParams.jumpRequest) {
			Debug.Log ("movementState=" + movementState);
			Debug.Log ("interactState=" + interactState);
			Debug.Log ("IsGrounded=" + IsGrounded());
			Debug.Log ("moveStateParameters.jumpRequired=" + moveStateParameters.jumpRequired);
			inputParams.jumpRequest = false;
			inputController.SetUserRequest (inputParams);
		}
    }

    void UpdateInteractStateParameters(InputParams inputParams) {
        switch (inputParams.actionRequest) {
			case ActionRequest.Glide:
                if (movementState == MovementState.Fall || movementState == MovementState.PushUp) {
                    interactStateParameter.canGlide = true;
                } else {
                    interactStateParameter.canGlide = false;
                }
                break;

            case ActionRequest.MeleeAttack:
                if (interactState != InteractState.Glide) {
                    interactStateParameter.finishedMeleeAttack = false;
                    interactStateParameter.canMeleeAttack = true;
                }
                break;

            case ActionRequest.DistantAttack:
                if (interactState != InteractState.Glide) {
                    interactStateParameter.finishedDistantAttack = false;
                    interactStateParameter.canDistantAttack = true;
                }
                break;

            case ActionRequest.SpawnLure:
                if (actualLure == null) {
                    interactStateParameter.canSpawnLure = true;
                } else if (interactState != InteractState.SpawnLure) {
                    interactStateParameter.canDestroyLure = true;
                }
                break;

            case ActionRequest.Inflate:
                if (IsGrounded()) interactStateParameter.canInflate = true;
                break;

            case ActionRequest.Resize:
                if (IsGrounded()) interactStateParameter.canResize = true;
                break;

            case ActionRequest.ContextualAction:
                GameObject nearestObject = absorbRange.GetComponent<DetectNearInteractObject>().GetNearestObject();

                if(interactState == InteractState.Carry) {
                    interactStateParameter.finishedCarry = true;
                } else if (nearestObject != null) {
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
				interactStateParameter.canGlide = false;
				interactStateParameter.canMeleeAttack = false;
				interactStateParameter.canDistantAttack = false;
				interactStateParameter.canSpawnLure = false;
				interactStateParameter.canDestroyLure = false;
				interactStateParameter.canInflate = false;
				interactStateParameter.canActivate = false;
				interactStateParameter.canAbsorb = false;
				interactStateParameter.canCarry = false;
				interactStateParameter.canPush = false;
				interactStateParameter.finishedCarry = false;
				interactStateParameter.canAirStream = false;
                nearestObject = null;
                break;
        }


        if (inputParams.actionRequest != ActionRequest.Glide) {
            inputParams.actionRequest = ActionRequest.None;
            inputController.SetUserRequest(inputParams);
        }
    }
    

    public bool GetOnGround() { return onGround; }
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
		if (collid.gameObject.CompareTag ("AirStreamZone")) {
			Debug.Log ("AirStreamZone enter");
			moveStateParameters.inAirStream = true;
		}
		if (collid.gameObject.CompareTag ("AirStreamForce") && interactState == InteractState.Glide)
		{
			Debug.Log ("AirStreamForce enter");
			interactStateParameter.canAirStream = true;
			/*body.velocity = new Vector3 (body.velocity.x, 0, body.velocity.z);
			body.AddForce (Vector3.up * 80, ForceMode.Impulse);*/
		}
	}

	void OnTriggerExit(Collider collid) {
		if (collid.gameObject.CompareTag("AirStreamZone")) {
			Debug.Log ("AirStreamZone Exit");
			moveStateParameters.inAirStream = false;
		}

		if (collid.gameObject.CompareTag ("AirStreamForce")) {
			Debug.Log ("AirStreamForce Exit " + body.velocity.y);
			interactStateParameter.canAirStream = false;
		}
	}

    void OnTriggerStay(Collider collid) {
        if (collid.gameObject.CompareTag("AirStreamZone")) {
			if (interactState != InteractState.Glide && previousInteractState == InteractState.Glide)
			{
				moveStateParameters.inAirStream = false;
			}
			else if (interactState == InteractState.Glide && previousInteractState != InteractState.Glide)
			{
				moveStateParameters.inAirStream = true;
			}
		}
		if (collid.gameObject.CompareTag("AirStreamForce")) {
			if (interactState != InteractState.Glide && previousInteractState == InteractState.Glide)
			{
				interactStateParameter.canAirStream = false;
			}
			else if (interactState == InteractState.Glide && previousInteractState != InteractState.Glide)
			{
				interactStateParameter.canAirStream = true;
				/*body.velocity = new Vector3 (body.velocity.x, 0, body.velocity.z);
				body.AddForce (Vector3.up * 10, ForceMode.Impulse);*/
			}
		}
        if (interactState == InteractState.Absorb && previousInteractState != InteractState.Absorb) {
            if (interactStateParameter.canAbsorb) {
                interactBehaviorCtrl.DoBeginAbsorption(collid.gameObject);
            }

        } else if (previousInteractState == InteractState.Absorb) {
            Pair<Capacity, float> pairCapacity = interactBehaviorCtrl.DoContinueAbsorption(collid.gameObject);
            if (pairCapacity.First != Capacity.Nothing) {
                AddCapacity(pairCapacity);
            }
        }
    }

    private void AddCapacity(Pair<Capacity, float> pairCapacity) {
        switch (pairCapacity.First) {

            case Capacity.DoubleJump:
                temporaryDoubleJumpCapacity = true;
                
                break;

            case Capacity.Glide:
                break;
        }

        timerCapacity = pairCapacity.Second;
    }

    private void StopTemporaryCapacity() {
        timerCapacity = 0;
        temporaryDoubleJumpCapacity = false;
    }
}
