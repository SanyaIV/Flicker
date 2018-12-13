using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/Frozen")]
public class Frozen : EnemyState
{
    [Header("Sanity")]
    [SerializeField] private bool _depleteSanity = false;
    [SerializeField] private float _depletionAmount;

    public override void Initialize(Controller owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        //_controller.navMeshAgent.velocity = Vector3.zero;
        _controller.navMeshAgent.isStopped = true;
        _controller.StopAudio();
    }

    public override void Update()
    {
        if (!_controller.IsVisible())
        {
            _controller.TransitionTo<Hunt>();
            return;
        }

        _controller.sanity.ProgressBlink();

        if(_depleteSanity)
            _controller.sanity.DepleteSanity(_depletionAmount);
    }
}
