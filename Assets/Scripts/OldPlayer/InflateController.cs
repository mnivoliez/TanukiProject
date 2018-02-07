using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflateController : MonoBehaviour {

    [SerializeField]
    private GameObject inflateForm;
    [SerializeField]
    private GameObject normalForm;
    private bool isInflate;
    [SerializeField]
    private GameObject smokeSpawner;


    void Start() {
        isInflate = false;

    }

    void Update() {


        if (Input.GetButtonDown("Transformation")) {
            if (isInflate) {
                GameObject smoke = Instantiate(smokeSpawner, inflateForm.transform.position, Quaternion.identity);
                Destroy(smoke, 4);
                normalForm.transform.position = inflateForm.transform.position;
                normalForm.SetActive(true);
                inflateForm.GetComponent<Rigidbody>().velocity = Vector3.zero;
                inflateForm.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                inflateForm.transform.rotation = Quaternion.identity;
                inflateForm.SetActive(false);
                isInflate = false;
            }
            else {
                if (normalForm.GetComponent<PlayerDistantAttack>().GetLeafIsBack()) {
                    GameObject smoke = Instantiate(smokeSpawner, normalForm.transform.position, Quaternion.identity);
                    Destroy(smoke, 4);
                    inflateForm.transform.position = normalForm.transform.position;
                    inflateForm.transform.rotation = normalForm.transform.rotation;
                    inflateForm.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    inflateForm.SetActive(true);
                    normalForm.SetActive(false);
                    isInflate = true;
                }

            }


        }

    }
}
