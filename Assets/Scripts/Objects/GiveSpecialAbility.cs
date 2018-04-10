using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveSpecialAbility : MonoBehaviour {

    [SerializeField] protected Capacity capacity;
    [SerializeField] protected float timerCapacity = 0;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        //===========================
        if (Pause.Paused) {
            return;
        }
        //===========================
        transform.Rotate(new Vector3(0, 2, 0));
    }


    void OnTriggerEnter(Collider collid) {
        if (collid.gameObject.CompareTag("Player")) {
            Pair<Capacity, float> pairCapacity = new Pair<Capacity, float>(capacity, timerCapacity);
            collid.gameObject.GetComponent<KodaController>().AddCapacity(pairCapacity);
            Destroy(gameObject);
        }

    }

    public void SetTimerCapacity(float timerPowerUp) {
        timerCapacity = timerPowerUp;
    }

}
