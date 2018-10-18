using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState : ScriptableObject
{
    public virtual void Initialize(EnemyStateController owner) { }
    public virtual void LateInitialize() { }
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}
