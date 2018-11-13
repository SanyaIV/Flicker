using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/Patrol")]
public class Patrol : EnemyState
{
    [Header("Settings")]
    Transform _destination;
    NavMeshAgent _navMeshAgent;

    public Transform target;

    private EnemyController _controller;
    private int _destPoint = 0;
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

        _navMeshAgent.isStopped = false;
        _controller.basicAudio.Resume();

    }

    private void Start()
    {
        if (_controller.wayPoints[0] == null)
            return;
        _destination = _controller.wayPoints[0];
        _navMeshAgent.autoBraking = false;
        GoToNextPoint();

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
        if (_controller.PlayerClose())
        {
            _controller.TransitionTo<Hunt>();
        }
        else
        {
            SetDestination();

            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < 0.5f)
            {
                GoToNextPoint();
            }
        }
      
    }

    private void GoToNextPoint()
    {
        if (_controller.wayPoints.Length == 0 || _controller.wayPoints[0] == null)
            return;

        _navMeshAgent.destination = _controller.wayPoints[_destPoint].position;
        _destPoint = (_destPoint + 1) % _controller.wayPoints.Length;
    }
}



