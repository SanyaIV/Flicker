using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState : State
{
    protected EnemyController _controller;
    protected Transform _transform { get { return _controller.transform; } }

    public override void Initialize(Controller owner) {
        _controller = (EnemyController)owner;
    }
}
