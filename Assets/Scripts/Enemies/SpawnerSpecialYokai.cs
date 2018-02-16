using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSpecialYokai : MonoBehaviour {

    [SerializeField] private GameObject prefabYokai;
    [SerializeField] private GameObject prefabSmokeSpawn;
    [SerializeField] private float timerPowerUp;
    private float cooldownSpawner = 0;
    private GameObject instanceYokai;


	void Start () {
        SpawnYokai();
    }
	
	void FixedUpdate () {
		if(instanceYokai == null) {
            cooldownSpawner += 0.02f;
            if (cooldownSpawner > timerPowerUp) {
                SpawnYokai();
            }

        }
	}

    void SpawnYokai() {

        Vector3 newPosSpawn = Random.insideUnitSphere * 5;
        newPosSpawn.y = 0;
        Instantiate(prefabSmokeSpawn, transform.position + newPosSpawn, Quaternion.identity);
        instanceYokai = Instantiate(prefabYokai, transform.position + newPosSpawn, Quaternion.identity);
        instanceYokai.GetComponent<YokaiController>().SetTimerCapacity(timerPowerUp);
        instanceYokai.GetComponent<Rigidbody>().AddForce(Vector3.up * 300, ForceMode.Impulse);
        cooldownSpawner = 0;

    }
}
