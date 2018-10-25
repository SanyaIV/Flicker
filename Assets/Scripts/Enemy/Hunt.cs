using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/Hunt")]
public class Hunt : EnemyState
{
    [Header("Settings")]
    [SerializeField] public float _distanceToDepleteSanity;
    Transform _destination;
    NavMeshAgent _navMeshAgent;

    private EnemyController _controller;
    public Transform target;

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
        _navMeshAgent = _controller.GetComponent<NavMeshAgent>();
        _destination = _controller.player;

        if (_navMeshAgent == null)
        {
            Debug.LogError("The NavMeshAgent is not attached to " + _controller.name);
        }

        _navMeshAgent.isStopped = false;
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

        if(Vector3.Distance(_controller.player.position, transform.position) < _distanceToDepleteSanity)
        {
            _controller.sanity.DepleteSanity(2f);
        }
    }
}


