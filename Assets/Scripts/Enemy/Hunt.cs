using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/Hunt")]
public class Hunt : EnemyState
{
    [Header("Hunt")]
    [SerializeField] private float _maxFollowDistance = 10f;

    [Header("Sanity")]
    [SerializeField] private float _distanceToDeplete;
    [SerializeField] private MinMaxFloat _depletionAmount;

    [Header("Settings")]
    private Transform _target;
    private NavMeshAgent _navMeshAgent;

    public override void Initialize(Controller owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        _navMeshAgent = _controller.GetComponent<NavMeshAgent>();
        _target = _controller.player;

        if (_navMeshAgent == null)
        {
            Debug.LogError("The NavMeshAgent is not attached to " + _controller.name);
        }

        _navMeshAgent.isStopped = false;

        _controller.basicAudio.Resume();
    }

    private void SetDestination()
    {
        if (_target != null)
        {
            Vector3 direction = (_target.position - _transform.position).normalized;
            Vector3 _targetVector = _target.position - direction;
            _navMeshAgent.SetDestination(_targetVector);
        }
    }

    public override void Update()
    {
        SetDestination();
        float distance = Vector3.Distance(_controller.player.position, _transform.position);
        if (distance < _distanceToDeplete)
            _controller.sanity.DepleteSanity(Mathf.Lerp(_depletionAmount.Min, _depletionAmount.Max, (_distanceToDeplete - distance) / _distanceToDeplete));

        if (Vector3.Distance(_target.position, _controller.transform.position) > _maxFollowDistance)
        {
            _navMeshAgent.SetDestination(_controller.wayPoints[0].position);
            _controller.TransitionTo<Patrol>();
        }
    }
}


