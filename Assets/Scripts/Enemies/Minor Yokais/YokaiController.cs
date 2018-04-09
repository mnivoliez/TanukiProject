using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Capacity {
    Nothing, DoubleJump, Glide, Lure, BallForm
}

public abstract class YokaiController : MonoBehaviour {
    

    protected bool isAbsorbed = false;
    protected bool isKnocked = false;
    [SerializeField] protected float speed = 8f;
    [SerializeField] protected float hp = 10f;
    [SerializeField] protected GameObject hitParticle;
    [SerializeField] protected GameObject knockedParticle;
    protected Material rendererMat;
    protected float rotationSpeed = 10f;
    [SerializeField]
    protected GameObject target;
    [SerializeField] protected Capacity capacity;
    [SerializeField] protected float timerCapacity = 0;
    [SerializeField] protected float damage = 1f;
    protected Vector3 positionOrigin;
    [SerializeField] protected float distanceLimit;
    [SerializeField] protected AudioClip absorbed;
    [SerializeField] protected AudioClip yokaiScream;
    [SerializeField] protected AudioClip yokaiHurt;

    public virtual void LooseHp(float damage) { }

    public virtual void BeingHit() { }

    public virtual void EndHit() { }

    public virtual void Absorbed() { }

    public virtual void Die() { }

    public virtual void Behavior() { }

    public Capacity GetCapacity() { return capacity; }

    public float GetTimerCapacity() { return timerCapacity; }

    public bool GetIsKnocked() { return isKnocked; }

    public GameObject GetTarget() { return target; }

    public void SetTarget(GameObject target) { this.target = target; }

    public void SetTimerCapacity(float timer) { timerCapacity = timer; }

    public bool TooFarAway() {
        bool tooFarAway = false;
        if (Vector3.Distance(transform.position, positionOrigin) > distanceLimit) {
            tooFarAway = true;
        }
        return tooFarAway;
    }
    
    public bool TooFarAway(Vector3 position) {
        bool tooFarAway = false;
        if (Vector3.Distance(position, positionOrigin) > distanceLimit) {
            tooFarAway = true;
        }
        return tooFarAway;
    }
}

