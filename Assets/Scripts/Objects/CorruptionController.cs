using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum LanternEffect {
    Appear,
    Disappear
}
public class CorruptionController : MonoBehaviour {

    [SerializeField] private LanternEffect _lanternEffect;
    private List<LanternController> _lanterns;

    private Collider _bbox;

    void Start() {
        _lanterns = new List<LanternController>();
        foreach (Collider box in GetComponents<Collider>()) {
            if (!box.isTrigger) _bbox = box;
        };
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Lantern")) {
            _lanterns.Add(other.gameObject.GetComponent<LanternController>());
        }
    }



    void OnTriggerExit(Collider other) {

        if (other.CompareTag("Lantern")) {
            _lanterns.Remove(other.gameObject.GetComponent<LanternController>());
        }
        else {
            ManageCollision(other);
        }

    }

    void OnTriggerStay(Collider other) {

        if (!other.CompareTag("Lantern")) {
            ManageCollision(other);
        }
    }

    void OnCollisionStay(Collision collision) {
        //if (!collision.collider.CompareTag("Lantern")) {
        ManageCollision(collision.collider);
        //}
    }

    private void ManageCollision(Collider other) {
        if (Game.playerData.lightSandbox) {
            switch (_lanternEffect) {
                case LanternEffect.Appear:
                    MakeAppearForCollider(other);
                    break;
                case LanternEffect.Disappear:
                    MakeDisappearForCollider(other);
                    break;
            }
        } else {
            bool isInLanternRange = CheckIfInLanternRange(other);
            switch (_lanternEffect) {
                case LanternEffect.Appear:
                    if (isInLanternRange) {
                        //if (this.name == "P_PlateformTree_Logs_02") { Debug.Log("appear collider" + this.name); }                    
                        MakeAppearForCollider(other);
                    }
                    else {
                        //if (this.name == "P_PlateformTree_Logs_02") { Debug.Log("disappear collider" + this.name); }
                        MakeDisappearForCollider(other);
                    }
                    break;
                case LanternEffect.Disappear:
                    if (isInLanternRange) {
                        MakeDisappearForCollider(other);
                    }
                    else {
                        MakeAppearForCollider(other);
                    }
                    break;
            }
        }
           

    }

    private bool CheckIfInLanternRange(Collider other) {
        bool isInLanternRange = false;
        var iter = _lanterns.GetEnumerator();
        while (iter.MoveNext() && !isInLanternRange) {
            isInLanternRange = iter.Current.isInEffectArea(other.ClosestPoint(iter.Current.transform.position));
        }

        return isInLanternRange;
    }

    private void MakeDisappearForCollider(Collider other) {
        Physics.IgnoreCollision(other, _bbox, true);
    }

    private void MakeAppearForCollider(Collider other) {
        Physics.IgnoreCollision(other, _bbox, false);
    }
}
