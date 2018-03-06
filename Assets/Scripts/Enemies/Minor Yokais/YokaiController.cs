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
}

