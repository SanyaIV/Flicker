using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Wander")]
public class Wander : EnemyState
{

    private EnemyController _controller;
    //public Transform target;

    private Transform transform { get { return _controller.transform; } }
    private Vector3 velocity
    {
        get { return _controller.velocity; }
        set { _controller.velocity = value; }
    }

    public override void Initialize(EnemyStateController owner)
    {
        _controller = (EnemyController)owner;
    }

    public override void Enter()
    {
        Debug.Log("Wandering");
    }

    public override void Update()
    {
        Debug.Log("is wandering");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _controller.TransitionTo<Idle>();
        }
    }
}
