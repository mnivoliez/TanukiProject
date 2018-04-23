using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct TimelineObject {
    public GameObject gameObject;
}

public class Freeze_Imput : MonoBehaviour {

    [SerializeField] private bool isfreeze;
    [SerializeField] private bool deleteTimeline;
    [SerializeField] private TimelineObject[] timelineObject;

    private InputController playerInput;

	void Start () {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<InputController>();
        if (deleteTimeline) {
            foreach (TimelineObject obj in timelineObject) {
                if (obj.gameObject != null) {
                    Destroy(obj.gameObject, 3f);
                }
            }
        }
    }
	

	void FixedUpdate () {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        playerInput.SetFreezeInput(isfreeze);
    }

}
