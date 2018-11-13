﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/Idle")]
public class Idle : EnemyState
{
    private EnemyController _controller;
    //private bool _inLineOfSight = true;
    //private float _minHuntDistance = 1f;
    public Transform target;
    NavMeshAgent _navMeshAgent;

    private Transform transform { get { return _controller.transform; } }
    private Vector3 velocity
    {
        get { return _controller.velocity; }
        set { _controller.velocity = value; }
    }

    public override void Initialize(EnemyStateController owner)
    {
        _controller = (EnemyController)owner;
        _controller.velocity = new Vector3(0, 0);
    }

    public override void Enter()
    {
        _navMeshAgent = _controller.GetComponent<NavMeshAgent>();
        if (_navMeshAgent == null)
        {
            Debug.LogError("The NavMeshAgent is not attached to " + _controller.name);
        }
        _navMeshAgent.isStopped = true;

        _controller.basicAudio.Pause();
    }

    public override void Update()
    {
        if (!(_controller.CheckIfEnemyIsVisible()))
        {
            Debug.Log("Hidden");
            Debug.Log("Transitioning from idle to patrol");
            _controller.TransitionTo<Patrol>();
        }
    }
}
