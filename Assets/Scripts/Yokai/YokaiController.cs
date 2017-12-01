using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiController : MonoBehaviour {


    private bool isAbsorbed = false;
    private float speed = 5f;
    private float rotationSpeed = 10f;
    private GameObject target;


    void Start () {
        target = GameObject.FindGameObjectWithTag("Player");

    }
	
	void Update () {

        if (isAbsorbed) {
            Die();
        }

	}

    public void Absorbed() {
        isAbsorbed = true;
        gameObject.GetComponent<Collider>().enabled = false;
    }

    void Die() {
        if (transform.position == target.transform.position) {
            target.GetComponent<Animator>().SetBool("isAbsorbing", false);
            target.GetComponent<PlayerAbsorption>().sakePot.SetActive(false);
            Destroy(gameObject);
        }
        else {
            if (transform.localScale.x < 0 && transform.localScale.y < 0 && transform.localScale.z < 0 ) {
                transform.localScale = Vector3.zero;
            }
            else {
                transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
            }
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            transform.Rotate(Vector3.right, rotationSpeed);
            transform.Rotate(Vector3.up, rotationSpeed);
            rotationSpeed += 2;
        }
    }
}
