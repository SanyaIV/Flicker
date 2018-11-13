using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/Hunt")]
public class Hunt : EnemyState
{
    [Header("Sanity")]
    [SerializeField] private float _distanceToDeplete;
    [SerializeField] private float _depletionAmount;

    [Header("Settings")]
    Transform _destination;
    NavMeshAgent _navMeshAgent;

    public Transform target;

    private EnemyController _controller;

    private Transform transform { get { return _controller.transform; } }

    public override void Initialize(EnemyStateController owner)
    {
        _controller = (EnemyController)owner;
    }

    public override void Enter()
    {
        _navMeshAgent = _controller.GetComponent<NavMeshAgent>();
        _destination = _controller.player;

        if (_navMeshAgent == null)
        {
            Debug.LogError("The NavMeshAgent is not attached to " + _controller.name);
        }

        _navMeshAgent.isStopped = false;

        _controller.basicAudio.Resume();
    }

    private void SetDestination()
    {
        if (_destination != null)
        {
            Vector3 direction = (_destination.position - transform.position).normalized;
            Vector3 _targetVector = _destination.position - direction;
            _navMeshAgent.SetDestination(_targetVector);
        }
    }

    public override void Update()
    {
        SetDestination();

        if(Vector3.Distance(_controller.player.position, transform.position) < _distanceToDeplete)
            _controller.sanity.DepleteSanity(_depletionAmount);

        if (!_controller.PlayerClose())
        {
            _navMeshAgent.SetDestination(_controller.wayPoints[0].position);
            _controller.TransitionTo<Patrol>();
        }
    }
}


