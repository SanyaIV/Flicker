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

        if (_controller.PlayerClose())
            _controller.TransitionTo<Hunt>();
        else if (!_controller.navMeshAgent.pathPending && _controller.navMeshAgent.remainingDistance < 0.5f)
            GoToNextPoint();

        _controller.ProgressStepCycle();
    }

    private void GoToNextPoint()
    {
        Debug.LogWarning("Still haven't finished dynamic waypoints!");
        _controller.navMeshAgent.SetDestination(_controller.GetWayPointInArea("Dandelion").position);
    }
}



