using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointDoubleLantern : MonoBehaviour {

    [SerializeField] private GameObject checkPoint;
    [SerializeField] private GameObject saveEffect;
    [SerializeField] private GameObject leftLantern;
    [SerializeField] private GameObject rightLantern;

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            Destroy(Instantiate(saveEffect, leftLantern.transform.GetChild(0).gameObject.transform.position, Quaternion.identity), 3f);
            leftLantern.transform.GetChild(0).gameObject.SetActive(true);
            Destroy(Instantiate(saveEffect, rightLantern.transform.GetChild(0).gameObject.transform.position, Quaternion.identity), 3f);
            rightLantern.transform.GetChild(0).gameObject.SetActive(true);
            other.gameObject.GetComponent<PlayerHealth>().SetRespawnPoint(checkPoint);
        }
    }
}
