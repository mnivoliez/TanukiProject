using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone1BossBehavior : YokaiController {

    public GameObject platform1;
    public GameObject platform2;
    public GameObject platform3;
    public GameObject platform4;

    void Start () {
        target = GameObject.FindGameObjectWithTag("Player");
        rendererMat = gameObject.GetComponent<Renderer>().material;
        Debug.Log("rendererMat: " + rendererMat);
    }

	void Update () {

    }

    public override void LooseHp(float damage){
        hp -= damage;

        if (hp <= 0) {
            isKnocked = true;
            Instantiate(knockedParticle, transform.position, Quaternion.identity).transform.parent = transform;
            rendererMat.color = new Color(150/255, 40/255, 150/255);
        }
    }

    public override void BeingHit() {
        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
        rendererMat.color = new Color(150/255, 40/255, 150/255);

    }

    public override void EndHit() {
        rendererMat.color = Color.white;
    }

    public override void Absorbed() {
        isAbsorbed = true;
        gameObject.GetComponent<Collider>().enabled = false;
    }

    public override void Die() {
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


}
