using System.Collections;
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

    public override void Initialize(EnemyStateController owner)
    {
        _controller = (EnemyController)owner;
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
            _controller.TransitionTo<Patrol>();
        }
    }
}
