using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStates/Idle")]
public class Idle : EnemyState
{

    private EnemyController _controller;
    private bool inLineOfSight = true;
    public Transform target;
    private float minHuntDistance = 1f;


    private Transform transform { get { return _controller.transform; } }
    private Vector3 velocity
    {
        get { return _controller.velocity; }
        set { _controller.velocity = value; }
    }

    public override void Initialize(EnemyStateController owner)
    {
        _controller = (EnemyController)owner;
        _controller.velocity = new Vector3(0, 0);
    }

    public override void Update()
    {
        Debug.Log("is idle");
        
    }
}
