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

    public override void Initialize(Controller owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        _controller.SwitchModel();
        _controller.navMeshAgent.isStopped = false;
        _controller.basicAudio.Resume();
    }

    public override void Update()
    {
        if (_controller.IsVisible())
        {
            _controller.TransitionTo<Frozen>();
            return;
        }

        SetDestination();
        float distance = Vector3.Distance(_controller.player.position, _transform.position);
        if (distance < _distanceToDeplete)
            _controller.sanity.DepleteSanity(Mathf.Lerp(_depletionAmount.Min, _depletionAmount.Max, (_distanceToDeplete - distance) / _distanceToDeplete));

        if (Vector3.Distance(_controller.player.position, _controller.transform.position) > _maxFollowDistance)
            _controller.TransitionTo<Patrol>();

        _controller.ProgressStepCycle();
    }

    private void SetDestination()
    {
        if (_controller.player != null)
        {
            Vector3 direction = (_controller.player.position - _transform.position).normalized;
            Vector3 _targetVector = _controller.player.position - direction;
            _controller.navMeshAgent.SetDestination(_targetVector);
        }
    }
}


