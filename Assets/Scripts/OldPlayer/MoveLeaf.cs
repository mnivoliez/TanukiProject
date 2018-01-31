using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeaf : MonoBehaviour {

    private float initialSpeed = 15f;
    private float currentSpeed;
    private float rotationSpeed = 50f;
    private float damage;
    private GameObject spawnLeaf = null;
    private Vector3 targetPosition = Vector3.zero;
    private bool arrived = false;

    void Start() {
        currentSpeed = initialSpeed;
    }

    void Update() {



        if (!arrived) {
            MoveTo();

            if (transform.position == targetPosition) {
                arrived = true;
            }
        }
        else {
            BackTo();
            currentSpeed += 0.2f;
            if (transform.position == spawnLeaf.transform.position) {
                arrived = false;
                currentSpeed = initialSpeed;
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDistantAttack>().SetLeafIsBack();
                Destroy(gameObject);
            }
        }



    }


    public void MoveTo() {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.left, rotationSpeed);
    }

    public void BackTo() {
        transform.position = Vector3.MoveTowards(transform.position, spawnLeaf.transform.position, currentSpeed * Time.deltaTime);
        transform.Rotate(Vector3.left, rotationSpeed);
    }

    public void SetSpawnPosition(GameObject spPos) {
        spawnLeaf = spPos;
    }

    public void SetTargetPosition(GameObject tPos) {
        targetPosition = new Vector3(tPos.transform.position.x, tPos.transform.position.y, tPos.transform.position.z);
    }

    public void SetDamage(float dmg) {
        damage = dmg;
    }

    public float GetDamage() {
        return damage;
    }

    void OnTriggerEnter(Collider collid) {

        if (collid.gameObject.CompareTag("Yokai") && !collid.gameObject.GetComponent<YokaiController>().GetIsKnocked()) {
            collid.gameObject.GetComponent<YokaiController>().BeingHit();
            collid.gameObject.GetComponent<YokaiController>().LooseHp(damage);
            targetPosition = collid.gameObject.transform.position;
        }

    }

    void OnTriggerExit(Collider collid) {

        if (collid.gameObject.CompareTag("Yokai") && !collid.gameObject.GetComponent<YokaiController>().GetIsKnocked()) {
            collid.gameObject.GetComponent<YokaiController>().EndHit();
        }

    }

}
