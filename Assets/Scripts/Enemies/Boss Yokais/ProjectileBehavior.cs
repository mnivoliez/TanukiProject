using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {

    [SerializeField] private float speed = 30;
    [SerializeField] private GameObject hitParticle;
    private float damage = 1f;

    void Start() {
        Destroy(gameObject, 10);
    }


    void Update() {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    public void SetDamage(float dmg) {
        damage = dmg;
    }

    void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Player")) {

            other.gameObject.GetComponent<PlayerHealth>().LooseHP(damage);
            Destroy(Instantiate(hitParticle, other.gameObject.transform.position, Quaternion.identity), 1);
            Destroy(gameObject);
        }

    }
}
