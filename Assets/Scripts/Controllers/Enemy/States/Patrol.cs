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
        string targetArea;
        bool dandelion = false;
        foreach(Area area in AreaTracker.GetVisitedAreas())
            if (area.GetName() == "Dandelion")
                dandelion = true;

        if(AreaTracker.GetCurrentPlayerArea() != null && AreaTracker.GetCurrentEnemyArea() != null)
        {
            if (dandelion && AreaTracker.GetCurrentPlayerArea().GetName() != "Escape Pod")
                targetArea = AreaTracker.GetCurrentPlayerArea().GetName();
            else
                targetArea = "Indigo";

            if (dandelion && AreaTracker.GetCurrentEnemyArea() != AreaTracker.GetCurrentPlayerArea())
                _controller.navMeshAgent.Warp(_controller.GetSpawnPointInArea(targetArea).position);

            _controller.navMeshAgent.SetDestination(_controller.GetWayPointInArea(targetArea).position);

            return;
        }

        _controller.navMeshAgent.SetDestination(_controller.GetWayPointInArea("Dandelion").position);
    }
}



