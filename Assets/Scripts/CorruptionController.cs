using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionController : MonoBehaviour
{

    [SerializeField] private bool _corruptionHideObject;
    private LanternController _lantern;

    private BoxCollider _bbox;

    void Start()
    {
        foreach (BoxCollider box in GetComponents<BoxCollider>())
        {
            if (!box.isTrigger) _bbox = box;
        };
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Lantern"))
        {
            _lantern = other.gameObject.GetComponent<LanternController>();
        }
    }



    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Lantern"))
        {
            _lantern = null;
        }
        else
        {
            ManageCollision(other);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Lantern")) ManageCollision(other);

    }

    void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Lantern")) ManageCollision(collision.collider);

    }

    private void ManageCollision(Collider other)
    {
        bool isAffectedByLantern = _lantern != null && _lantern.isInEffectArea(other.ClosestPoint(_lantern.transform.position));
        Physics.IgnoreCollision(other, _bbox, _corruptionHideObject ? !isAffectedByLantern : isAffectedByLantern);
    }
}
