using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour {

    [SerializeField]
    private float timeDelay;

    private void Start() {
        Destroy(gameObject, timeDelay);
    }
}
