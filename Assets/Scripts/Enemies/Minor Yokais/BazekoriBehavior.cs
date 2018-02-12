using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazekoriBehavior : YokaiController {

    [SerializeField] private float jumpForce = 300f;
    bool followPlayer = false;
    Vector3 startPosition;
    Vector3 endPosition;
    Vector3 bending = Vector3.up;
    float timeToTravel = 10f;
    float timeStamp = 0;

    void Start() {
        target = GameObject.FindGameObjectWithTag("Player");
        rendererMat = gameObject.GetComponent<Renderer>().material;
        
    }

    void Update() {
        transform.GetChild(0).transform.LookAt(target.transform);
        if (followPlayer) {
            MoveToPosition();
        }
        else {
            transform.LookAt(new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z));
        }

        if (isAbsorbed) {
            Die();
        }
    }

    private void FixedUpdate() {
        if (Random.Range(0, 100) == 5) {
            Behavior();
        }
    }

    public override void LooseHp(float damage) {
        hp -= damage;

        if (hp <= 0) {
            isKnocked = true;
            Instantiate(knockedParticle, transform.position, Quaternion.identity).transform.parent = transform;
            rendererMat.color = new Color(150f / 255f, 40f / 255f, 150f / 255f);
        }

    }

    public override void BeingHit() {
        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
        rendererMat.color = new Color(150f / 255f, 40f / 255f, 150f / 255f);
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
            Destroy(gameObject);
        }
        else {
            if (transform.localScale.x < 0 && transform.localScale.y < 0 && transform.localScale.z < 0) {
                transform.localScale = Vector3.zero;
            }
            else {
                transform.localScale -= new Vector3(5f, 5f, 5f);
            }
            speed = speed + 0.2f;
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            transform.Rotate(Vector3.right, rotationSpeed);
            transform.Rotate(Vector3.up, rotationSpeed);
            rotationSpeed += 2;
        }
    }

    public override void Behavior() {
        GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void onTriggerEnter(Collider other) { 

        startPosition = transform.position;
        endPosition = target.transform.position;
        timeStamp = 0;
        timeToTravel = 10f;
        followPlayer = true;
        
    }

    public void MoveToPosition() {

        if (0 < timeToTravel - timeStamp) {
            Vector3 currentPos = Vector3.Lerp(startPosition, endPosition, (timeStamp) / timeToTravel);
            currentPos.y += 20 * Mathf.Sin(Mathf.Clamp01((timeStamp) / timeToTravel) * Mathf.PI);
            transform.position = currentPos;
            timeStamp += 0.1f;
        }
        else {
            followPlayer = false;
        }
    }
}
