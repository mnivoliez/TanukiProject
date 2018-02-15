using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnvironementParams
{
    public bool isGrounded;
    public bool isInAirStream;

}
public class PlayerEnvironementController : MonoBehaviour
{
    private EnvironementParams _params;
    private float coefInclination;

    private List<GameObject> _grounds = new List<GameObject>();

    void Start()
    {
        _params = new EnvironementParams();
    }

    public EnvironementParams AquireEnvironment()
    {
        _params.isGrounded = IsGrounded();
        return _params;
    }
    private bool IsGrounded()
    {
        return _grounds.Count > 0;
    }

    public bool IsGoingUp(MovementStateParam param)
    {
        bool up = !IsGrounded() && param.velocity.y > hysteresis_step;

        return up;
    }

    public bool IsFalling(MovementStateParam param)
    {
        bool fall = !param.grounded && (param.velocity.y < -hysteresis_step);

        return fall;
    }

    void OnCollisionStay(Collision coll)
    {
        GameObject gO = coll.gameObject;
        if (gO.layer == LayerMask.NameToLayer("Ground"))
        {
            ContactPoint[] contacts = coll.contacts;

            if (contacts.Length > 0)
            {
                bool found = false;
                foreach (ContactPoint c in contacts)
                {
                    if (c.normal.y >= 0.5f && c.normal.y <= 1f)
                    {
                        found = true;
                        coefInclination = Vector3.Angle(c.normal, Vector3.up);
                        break;
                    }
                }
                if (!found)
                {
                    coefInclination = 0;
                }
            }
        }
    }

    void OnCollisionExit(Collision coll)
    {
        if (IsGrounded())
        {
            GameObject gO = coll.gameObject;

            if (_grounds.Contains(gO))
            {
                _grounds.Remove(gO);
            }
        }
    }

    /*void OnTriggerStay(Collider collid)
    {
        if (collid.gameObject.CompareTag("AirStreamZone"))
        {
            moveStateParameters.inAirStream = true;
        }
        if (interactState == InteractState.Absorb && previousInteractState != InteractState.Absorb)
        {
            if (interactStateParameter.canAbsorb)
            {
                interactBehaviorCtrl.DoBeginAbsorption(collid.gameObject);
            }

        }
        else if (previousInteractState == InteractState.Absorb)
        {
            Pair<Capacity, float> pairCapacity = interactBehaviorCtrl.DoContinueAbsorption(collid.gameObject);
            if (pairCapacity.First != Capacity.Nothing)
            {
                AddCapacity(pairCapacity);
            }
        }
    }*/
}
