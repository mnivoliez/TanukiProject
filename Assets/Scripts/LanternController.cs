using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternController : MonoBehaviour {

    // Use this for initialization
    void Start() {
        gameObject.transform.parent.GetComponent<Renderer>().sharedMaterial.SetVector("_Center", transform.position);

    }

    // Update is called once per frame
    void Update() {
        transform.localScale = transform.localScale;
        gameObject.transform.parent.GetComponent<Renderer>().sharedMaterial.SetVector("_Center", transform.position);

        if (gameObject.transform.parent.gameObject.transform.parent == null) {
            gameObject.transform.parent.GetComponent<Renderer>().sharedMaterial.SetFloat("_Distance", 5f);
            GetComponent<Light>().intensity = 1;
        }
        else {
            gameObject.transform.parent.GetComponent<Renderer>().sharedMaterial.SetFloat("_Distance", 10f);
            GetComponent<Light>().intensity = 1.6f;
        }
    }
}
