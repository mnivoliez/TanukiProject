using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartCutScene : MonoBehaviour {

    [SerializeField] private GameObject timelineObject;
    private bool neverPlay = true;

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player") && neverPlay) {
            Debug.Log("CUTSCENE !");
            timelineObject.SetActive(true);
            neverPlay = false;
            timelineObject.GetComponent<PlayableDirector>().Play();
        }

    }
}
