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
        Debug.Log("Entering idle");
    }

    public override void Update()
    {
        fightMode();
    }

    public bool fightMode()
    {

        Debug.Log("In fight mode");
        Debug.Log(Vector3.Distance(_controller.cam.transform.position, transform.position) / 10 + " : " + Vector3.Dot((_controller.cam.transform.position - transform.position).normalized, _controller.cam.transform.forward));
        if (Vector3.Dot((_controller.cam.transform.position - transform.position).normalized, _controller.cam.transform.forward) > Mathf.Lerp(-0.6f, -0.25f, Vector3.Distance(_controller.cam.transform.position, transform.position) / 10))
            return false;

        _controller.planes = GeometryUtility.CalculateFrustumPlanes(_controller.cam);

        foreach (BoxCollider coll in _controller.colls)
        {
            if (GeometryUtility.TestPlanesAABB(_controller.planes, coll.bounds))
            {
                _controller.points[0] = transform.TransformPoint(coll.center);
                _controller.points[1] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                _controller.points[2] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                _controller.points[3] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                _controller.points[4] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                _controller.points[5] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                _controller.points[6] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, coll.size.y, -coll.size.z) * 0.5f);
                _controller.points[7] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                _controller.points[8] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, coll.size.y, -coll.size.z) * 0.5f);

                foreach (Vector3 point in _controller.points)
                    if (!Physics.Linecast(point, _controller.cam.transform.position, _controller.ignoreLayers))
                        return true;
            }
        }
        return true;
    }
}
