using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionController : MonoBehaviour
{

    [SerializeField] private bool _hide;
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
        Physics.IgnoreCollision(other, _bbox, _hide);
        if (other.CompareTag("Lantern"))
        {
            _lantern = null;
        }
    }

    void OnTriggerStay(Collider other)
    {
        bool is_affected_by_lantern = _lantern != null && _lantern.isInEffectArea(other.ClosestPoint(_lantern.transform.position));
        bool shouldNotCollide = is_affected_by_lantern ? !_hide : _hide;
        Physics.IgnoreCollision(other, _bbox, shouldNotCollide);

    }

    void OnCollisionStay(Collision collision)
    {
        bool is_affected_by_lantern = _lantern != null && _lantern.isInEffectArea(collision.contacts[0].point);
        bool shouldNotCollide = is_affected_by_lantern ? !_hide : _hide;
        Physics.IgnoreCollision(collision.collider, _bbox, true);
    }
}
