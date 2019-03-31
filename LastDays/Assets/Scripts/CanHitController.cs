using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CanHitController : MonoBehaviour
{
    protected CanHitController target;
    public abstract void GetHit(float damage);

    public abstract void Hit();

    public void setTarget(CanHitController target) {
        this.target = target;
    }
}
