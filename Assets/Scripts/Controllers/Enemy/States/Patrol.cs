using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/Patrol")]
public class Patrol : EnemyState
{
    [Header("Settings")]
    private Transform _destination;

    public override void Initialize(Controller owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        _controller.SwitchModel();
        _controller.navMeshAgent.isStopped = false;
        _controller.basicAudio.Resume();
        GoToNextPoint();
    }

    public override void Update()
    {
        if (_controller.IsVisible())
        {
            _controller.TransitionTo<Frozen>();
            return;
        }

        if (_controller.PlayerClose() && _controller.PlayerVisible())
        {
            _controller.TransitionTo<Hunt>();
            return;
        }
        else if (!_controller.navMeshAgent.pathPending && _controller.navMeshAgent.remainingDistance < 0.5f)
            GoToNextPoint();

        if (_controller.navMeshAgent.isOnOffMeshLink)
        {
            _controller.navMeshAgent.velocity = Vector3.zero;
            GoToNextPoint();
            _controller.navMeshAgent.Warp(_transform.position);
        }

        _controller.ProgressStepCycle();
    }

    private void GoToNextPoint()
    {
        if(AreaTracker.GetCurrentPlayerArea() != null && AreaTracker.GetCurrentEnemyArea() != null)
            if (_controller.GetThreatLevel() > 0 && AreaTracker.GetCurrentEnemyArea() != AreaTracker.GetCurrentPlayerArea())
                _controller.navMeshAgent.Warp(_controller.GetSpawnPointInArea(_controller.GetTargetArea()).position);

        _controller.navMeshAgent.SetDestination(_controller.GetWayPointInArea(_controller.GetTargetArea()).position);
    }
}



