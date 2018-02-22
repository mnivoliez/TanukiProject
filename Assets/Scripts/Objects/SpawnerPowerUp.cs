using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPowerUp : MonoBehaviour {

    [SerializeField] private GameObject prefabPowerUp;
    [SerializeField] private GameObject prefabSmokeSpawn;
    [SerializeField] private float timerPowerUp;
    private float cooldownSpawner = 0;
    private GameObject instancePowerUp;


    void Start() {
        SpawnPowerUp();
    }

    void FixedUpdate() {
        if (instancePowerUp == null) {
            cooldownSpawner += 0.02f;
            if (cooldownSpawner > timerPowerUp) {
                SpawnPowerUp();
            }

        }
    }

    void SpawnPowerUp() {

        Vector3 newPosSpawn = Random.insideUnitSphere * 3;
        newPosSpawn.y = 0;
        Instantiate(prefabSmokeSpawn, transform.position + newPosSpawn, Quaternion.identity);
        instancePowerUp = Instantiate(prefabPowerUp, transform.position + newPosSpawn, Quaternion.identity);
        instancePowerUp.GetComponent<GiveSpecialAbility>().SetTimerCapacity(timerPowerUp);
        instancePowerUp.GetComponent<Rigidbody>().AddForce(Vector3.up * 300 * Time.deltaTime, ForceMode.Impulse);
        cooldownSpawner = 0;

    }
}
