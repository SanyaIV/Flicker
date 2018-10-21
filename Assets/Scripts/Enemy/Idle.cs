using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "EnemyStates/Idle")]
public class Idle : EnemyState
{

    private EnemyController _controller;
    private bool inLineOfSight = true;
    public Transform target;
    private float minHuntDistance = 1f;
    NavMeshAgent _navMeshAgent;


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
        Debug.Log("Entering idle");

    }

    public override void Enter()
    {
        _navMeshAgent = _controller.GetComponent<NavMeshAgent>();
        if (_navMeshAgent == null)
        {
            Debug.LogError("The NavMeshAgent is not attached to " + _controller.name);
        }
        _navMeshAgent.isStopped = true;
    }

    public override void Update()
    {
        if (!CheckIfVisible())
        {
            //transform.position = Vector3.MoveTowards(transform.position, _controller.player.transform.position, _controller.speed * Time.deltaTime);
            _controller.TransitionTo<Hunt>();
            _navMeshAgent.isStopped = false;
        }
    }

    private bool CheckIfVisible()
    {
        if (_controller.rend.isVisible) //Check if Unity thinks the renderer is visible (Not perfect but works as a quick and easy out in case it's not)
        {
            if (Vector3.Dot((_controller.cam.transform.position - transform.position).normalized, _controller.cam.transform.forward) > Mathf.Lerp(-0.6f, -0.25f, Vector3.Distance(_controller.cam.transform.position, transform.position) / 10)) //Bad attempt at checking if the player is looking towards the enemy through the dot products of directions
                return _controller.visible = false; //Set visible to false and return visible (which is false)

            int taskNumber = Time.frameCount % _controller.colls.Length; //Run one box per frame

            if (taskNumber == 0)
                _controller.visible = false; //Reset visibility status at start of the "loop"
            if (_controller.visible)
                return true; //If visible is true then just return since it means the enemy has been seen for this round of the "loop"

            _controller.planes = GeometryUtility.CalculateFrustumPlanes(_controller.cam); //Get the frustum planes of the camera

            if (GeometryUtility.TestPlanesAABB(_controller.planes, _controller.colls[taskNumber].bounds)) //Test if the current boxcollider is within the camera's frustum
            {
                BoxCollider coll = _controller.colls[taskNumber]; //Get the current boxcollider
                _controller.points[0] = transform.TransformPoint(_controller.colls[taskNumber].center);
                _controller.points[1] = transform.TransformPoint(_controller.colls[taskNumber].center + new Vector3(coll.size.x, -coll.size.y, coll.size.z) * 0.5f); //One corner of the boxcollider
                _controller.points[2] = transform.TransformPoint(_controller.colls[taskNumber].center + new Vector3(coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                _controller.points[3] = transform.TransformPoint(_controller.colls[taskNumber].center + new Vector3(-coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                _controller.points[4] = transform.TransformPoint(_controller.colls[taskNumber].center + new Vector3(-coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                _controller.points[5] = transform.TransformPoint(_controller.colls[taskNumber].center + new Vector3(coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                _controller.points[6] = transform.TransformPoint(_controller.colls[taskNumber].center + new Vector3(coll.size.x, coll.size.y, -coll.size.z) * 0.5f);
                _controller.points[7] = transform.TransformPoint(_controller.colls[taskNumber].center + new Vector3(-coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                _controller.points[8] = transform.TransformPoint(_controller.colls[taskNumber].center + new Vector3(-coll.size.x, coll.size.y, -coll.size.z) * 0.5f);

                foreach (Vector3 point in _controller.points) //Loop through the points array
                    if (!Physics.Linecast(point, _controller.cam.transform.position, _controller.ignoreLayers)) //Linecast between the enemy and the camera to check if there is anything in the way
                        return _controller.visible = true; //If there is nothing in the way then the enemy is visible, so set visible to true and return it.        
            }
        }

        return _controller.visible = false; //Set visible to false and return it
    }
}
