using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionRequest
{
    Glide, MeleeAttack, DistantAttack, SpawnLure, Inflate, Resize, ContextualAction, None
}

public struct InputParams
{
    public ActionRequest actionRequest;
    public bool contextualButtonPressed;
    public bool jumpRequest;
    public bool hasDoubleJumped;
    public float moveX;
    public float moveZ;

    public void Reset()
    {
        actionRequest = ActionRequest.None;
        contextualButtonPressed = false;
        jumpRequest = false;
        moveX = 0f;
        moveZ = 0f;
    }
}

public class InputController : MonoBehaviour
{

    private InputParams inputParams;
    private bool triggerAxisInUse = false;

    // Use this for initialization
    void Start()
    {
        inputParams.actionRequest = ActionRequest.None;
        inputParams = new InputParams();
        inputParams.hasDoubleJumped = false;
    }

    // Update is called once per frame
    void Update()
    {
        //inputParams.Reset();
        UpdateMoveInput();
        UpdateInteractInput();
    }

    void UpdateInteractInput()
    {
        int actionRequested = 0;
        if (Input.GetButton("Glide") || Input.GetAxisRaw("Glide") == -1)
        {
            //Debug.Log("input controller GLIDE!!!");
            actionRequested++;
            inputParams.actionRequest = ActionRequest.Glide;
        }
        else
        {
            if (inputParams.actionRequest == ActionRequest.Glide)
            {
                inputParams.actionRequest = ActionRequest.None;
            }

            //Reactiver apres le playtest !!
            if (Input.GetButtonDown("Fire1"))
            {
                actionRequested++;
                inputParams.actionRequest = ActionRequest.MeleeAttack;
            }
            if (Input.GetButtonDown("Fire2"))
            {
                actionRequested++;
                inputParams.actionRequest = ActionRequest.DistantAttack;
            }

            if (Input.GetAxisRaw("Lure") != 1) { triggerAxisInUse = false; }

            if (Input.GetButtonDown("Lure") || (Input.GetAxisRaw("Lure") == 1))
            {
                if (!triggerAxisInUse)
                {
                    triggerAxisInUse = true;
                    actionRequested++;
                    inputParams.actionRequest = ActionRequest.SpawnLure;
                }
            }

            //if (Input.GetButtonDown("Transformation")) {
            //    actionRequested++;
            //    inputParams.actionRequest = ActionRequest.Inflate;
            //}
            //if (Input.GetButtonDown("Resize")) {
            //    actionRequested++;
            //    inputParams.actionRequest = ActionRequest.Resize;
            //}
            if (Input.GetButtonDown("Fire3"))
            {
                actionRequested++;
                inputParams.actionRequest = ActionRequest.ContextualAction;
                inputParams.contextualButtonPressed = true;
            }
        }

        if (actionRequested > 1)
        {
            inputParams.actionRequest = ActionRequest.None;
        }


        //Debug.Log(inputParams.actionRequest);
    }

    void UpdateMoveInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            //Debug.Log("JUMP!!!");
            inputParams.jumpRequest = true;
        }
        //inputParams.jumpRequest = Input.GetButtonDown("Jump");
        inputParams.moveX = Input.GetAxis("Horizontal");
        inputParams.moveZ = Input.GetAxis("Vertical");
    }

    public InputParams RetrieveUserRequest()
    {
        return inputParams;
    }

    public void SetUserRequest(InputParams inputParamsNew)
    {
        inputParams = inputParamsNew;
    }
}
