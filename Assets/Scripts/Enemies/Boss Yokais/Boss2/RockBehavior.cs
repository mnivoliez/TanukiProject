using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehavior : MonoBehaviour {

    [SerializeField] protected GameObject hitParticle;

    void Start() {
        GameObject[] yokais = GameObject.FindGameObjectsWithTag("Yokai");
        Collider myCollider = GetComponent<Collider>();
        foreach (GameObject yokai in yokais) {
            Physics.IgnoreCollision(myCollider, yokai.GetComponent<Collider>());
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            collision.gameObject.GetComponent<PlayerHealth>().LooseHP(1);
            Destroy(Instantiate(hitParticle, collision.gameObject.transform.position, Quaternion.identity), 1);
        }
        Destroy(gameObject);
    }
}
