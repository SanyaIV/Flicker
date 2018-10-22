using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/Hunt")]
public class Hunt : EnemyState
{
    [SerializeField]
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
        Debug.Log("Hunting");
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
            Vector3 _targetVector = _destination.transform.position;
            _navMeshAgent.SetDestination(_targetVector);
        }
    }

    public override void Update()
    {
        SetDestination();
    }
}


