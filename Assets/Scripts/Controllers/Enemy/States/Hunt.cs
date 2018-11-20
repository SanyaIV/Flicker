using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/Hunt")]
public class Hunt : EnemyState
{
    [Header("Hunt")]
    [SerializeField] private float[] _timeWithoutTargetUntilGiveUp;
    private Vector3 _lastKnownPosition;
    private float _timeWithoutTarget;

    [Header("Sanity")]
    [SerializeField] private float[] _distanceToDeplete;
    [SerializeField] private MinMaxFloat[] _depletionAmount;

    private int threat = 0;

    public override void Initialize(Controller owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        _controller.SwitchModel();
        _controller.navMeshAgent.isStopped = false;
        _controller.basicAudio.Resume();
        _timeWithoutTarget = 0f;
    }

    public override void Update()
    {
        if (_controller.IsVisible())
        {
            _controller.TransitionTo<Frozen>();
            return;
        }

        threat = _controller.GetThreatLevel();

        SetDestination();

        if (_timeWithoutTarget >= _timeWithoutTargetUntilGiveUp[threat] || Vector3.Distance(_transform.position, _lastKnownPosition) < 0.5f)
            _controller.TransitionTo<Patrol>();

        _controller.ProgressStepCycle();
    }

    private void SetDestination()
    {
        if (_controller.PlayerVisible())
        {
            SetVisibleDestination();
            DepleteSanity();
            _timeWithoutTarget = 0f;
        }
        else
        {
            SetLastKnownPositionAsDestination();
            _timeWithoutTarget += Time.deltaTime;
        }
    }

    private void SetVisibleDestination()
    {
        if (_controller.player != null)
        {
            Vector3 direction = (_controller.player.position - _transform.position).normalized;
            Vector3 _targetVector = _controller.player.position - direction;
            _controller.navMeshAgent.SetDestination(_targetVector);
            _lastKnownPosition = _controller.player.position;
        }
    }

    private void SetLastKnownPositionAsDestination()
    {
        _controller.navMeshAgent.SetDestination(_lastKnownPosition);
    }

    private void DepleteSanity()
    {
        float distance = Vector3.Distance(_controller.player.position, _transform.position);
        if (distance < _distanceToDeplete[threat])
            _controller.sanity.DepleteSanity(Mathf.Lerp(_depletionAmount[threat].Min, _depletionAmount[threat].Max, (_distanceToDeplete[threat] - distance) / _distanceToDeplete[threat]));
    }
}


