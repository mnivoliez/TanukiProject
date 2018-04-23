using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//================================================
//SOUNDCONTROLER
//================================================

public enum ActionLantern { Activate, Deactivate };

[System.Serializable]
struct SwitchObject {
    public GameObject gameObject;
    public ActionLantern action;
}

public class Switch1 : MonoBehaviour {

    [SerializeField] private SwitchObject[] actionOnSwitch;
    Animator butterfly;
    bool alreadyPlayed = false;

    private void Start() {
        butterfly = gameObject.transform.GetChild(0).GetComponent<Animator>();
    }

    void OnCollisionEnter(Collision collider) {
        if (!alreadyPlayed) {
            if (collider.gameObject.CompareTag("Player")) {
                //================================================
                SoundController.instance.SelectENVQuick("Switch");
                //================================================
                alreadyPlayed = true;
                StartCoroutine(FlyAway());
                butterfly.SetTrigger("isFlying");
                foreach (SwitchObject obj in actionOnSwitch) {
                    if (obj.gameObject != null) {
                        obj.gameObject.SetActive(obj.action.Equals(ActionLantern.Activate));
                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!alreadyPlayed) {
            if (other.gameObject.CompareTag("Leaf")) {
                //================================================
                SoundController.instance.SelectENVQuick("Switch");
                //================================================
                alreadyPlayed = true;
                butterfly.SetTrigger("isFlying");
                StartCoroutine(FlyAway());
                foreach (SwitchObject obj in actionOnSwitch) {
                    if (obj.gameObject != null) {
                        obj.gameObject.SetActive(obj.action.Equals(ActionLantern.Activate));
                    }
                }
            }
        }
    }

    IEnumerator FlyAway() {
        float lerpTimeAnim = 0;
        GameObject butterflyBody = butterfly.gameObject;

        while (lerpTimeAnim < 1.1f) {
            lerpTimeAnim = Mathf.Lerp(lerpTimeAnim, 1.5f, 0.01f);
            butterflyBody.transform.position = butterflyBody.transform.position + (Vector3.up * Random.Range(0f, 0.5f) + (Vector3.right * Random.Range(-0.3f, 0.3f)) + (Vector3.forward * Random.Range(-0.3f, 0.3f) ));
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(butterflyBody);

    }
}
