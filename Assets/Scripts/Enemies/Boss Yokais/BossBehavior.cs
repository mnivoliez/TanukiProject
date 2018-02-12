using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour {

    private bool isAbsorbed = false;
    private bool isKnocked = false;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float hp = 10f;
    [SerializeField] private GameObject hitParticle;
    [SerializeField] private GameObject knockedParticle;
    private Material rendererMat;
    private float rotationSpeed = 10f;
    private GameObject target;
    

    void Start () {
        target = GameObject.FindGameObjectWithTag("Player");
        rendererMat = gameObject.GetComponent<Renderer>().material;
    }

	void Update () {

        if (isAbsorbed) {
            Die();
        }

    }

    public void LooseHp(float damage) {
        hp -= damage;

        if (hp <= 0) {
            isKnocked = true;
            Instantiate(knockedParticle, transform.position, Quaternion.identity).transform.parent = transform;
            rendererMat.color = new Color(150, 40, 150);
        }
    }

    public void BeingHit() {
        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
        rendererMat.color = new Color(150, 40, 150);

    }

    public void EndHit() {
        rendererMat.color = Color.white;
    }

    public void Absorbed() {
        isAbsorbed = true;
        gameObject.GetComponent<Collider>().enabled = false;
    }

    void Die() {
        if (transform.position == target.transform.position) {
            target.GetComponent<Animator>().SetBool("isAbsorbing", false);
            //target.GetComponent<PlayerAbsorption>().sakePot.SetActive(false);
            Destroy(gameObject);
        }
        else {
            if (transform.localScale.x < 0 && transform.localScale.y < 0 && transform.localScale.z < 0) {
                transform.localScale = Vector3.zero;
            }
            else {
                transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
            }
            speed = speed + 0.2f;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            transform.Rotate(Vector3.right, rotationSpeed);
            transform.Rotate(Vector3.up, rotationSpeed);
            rotationSpeed += 2;
        }
    }

    public bool GetIsKnocked() {
        return isKnocked;
    }


}
