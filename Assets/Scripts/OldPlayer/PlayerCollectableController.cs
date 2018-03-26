using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollectableController : MonoBehaviour {

    private int nbYokai = 0;
    // Use this for initialization
    void Start() {
       
    }

    // Update is called once per frame
    void Update() {
       
    }

    public void AddYokai() {
        nbYokai++;
        //Debug.Log("+1yokai !");
    }

    public int GetnbYokai () {
        return nbYokai;
    }

    public void SetnbYokai(int nbYokai_from_load) {
        nbYokai = nbYokai_from_load;
    }
}
