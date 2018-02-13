using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class YokaiController : MonoBehaviour {
    //    private bool isAbsorbed = false;
    //    private bool isKnocked = false;
    //    [SerializeField]
    //    private float speed = 8f;
    //    [SerializeField]
    //    private float hp = 3f;
    //    private float rotationSpeed = 10f;
    //    private GameObject target;
    //    [SerializeField]
    //    private Material knockedTexture;
    //    [SerializeField]
    //    private Material normalTexture;
    //    [SerializeField]
    //    private GameObject hitParticle;
    //    [SerializeField]
    //    private GameObject knockedParticle;



    //    void Start() {
    //        target = GameObject.FindGameObjectWithTag("Player");

    //    }

    //    void Update() {

    //        if (isAbsorbed) {
    //            Die();
    //        }

    //    }

    //    public void Absorbed() {
    //        isAbsorbed = true;
    //        gameObject.GetComponent<Collider>().enabled = false;
    //    }

    //    void Die() {
    //        if (transform.position == target.transform.position) {
    //            target.GetComponent<Animator>().SetBool("isAbsorbing", false);
    //            //target.GetComponent<PlayerAbsorption>().sakePot.SetActive(false);
    //            Destroy(gameObject);
    //        }
    //        else {
    //            if (transform.localScale.x < 0 && transform.localScale.y < 0 && transform.localScale.z < 0) {
    //                transform.localScale = Vector3.zero;
    //            }
    //            else {
    //                transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
    //            }
    //            speed = speed + 0.2f;
    //            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    //            transform.Rotate(Vector3.right, rotationSpeed);
    //            transform.Rotate(Vector3.up, rotationSpeed);
    //            rotationSpeed += 2;
    //        }
    //    }

    //    public bool GetIsKnocked() {
    //        return isKnocked;
    //    }

    //    public void LooseHp(float damage) {
    //        hp -= damage;

    //        if (hp <= 0) {
    //            isKnocked = true;
    //            Instantiate(knockedParticle, transform.position, Quaternion.identity).transform.parent = transform;
    //            transform.GetChild(0).GetComponent<Renderer>().material = knockedTexture;
    //            transform.GetChild(1).GetComponent<Renderer>().material = knockedTexture;
    //        }
    //    }

    //    public void BeingHit() {
    //        Destroy(Instantiate(hitParticle, transform.position, Quaternion.identity), 1);
    //        transform.GetChild(0).GetComponent<Renderer>().material = knockedTexture;
    //        transform.GetChild(1).GetComponent<Renderer>().material = knockedTexture;

    //    }

    //    public void EndHit() {
    //        transform.GetChild(0).GetComponent<Renderer>().material = normalTexture;
    //        transform.GetChild(1).GetComponent<Renderer>().material = normalTexture;
    //    }

    //    public void SetHp(float healPoint) {
    //        hp = healPoint;
    //    }

    //    public float GetHp() {
    //        return hp;
    //    }
    //}

    protected bool isAbsorbed = false;
    protected bool isKnocked = false;
    [SerializeField] protected float speed = 8f;
    [SerializeField] protected float hp = 10f;
    [SerializeField] protected GameObject hitParticle;
    [SerializeField] protected GameObject knockedParticle;
    protected Material rendererMat;
    protected float rotationSpeed = 10f;
    protected GameObject target;

    public virtual void LooseHp(float damage) { }

    public virtual void BeingHit() { }

    public virtual void EndHit() { }

    public virtual void Absorbed() { }

    public virtual void Die() { }

    public virtual void Behavior() { }

    public bool GetIsKnocked() { return isKnocked; }

}

