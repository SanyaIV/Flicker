using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : EnemyStateController
{
    private Plane[] planes;
    public Camera cam;
    private Renderer rend;
    private Vector3[] points = new Vector3[9];
    private float maxDistance = 30f;

    //private List<RaycastHit> hits;
    public BoxCollider[] colls;

    public LayerMask ignoreLayers;
    public bool visible = false;
    public Transform player;
    public float speed;

    private Vector3 _startPos;
    private Quaternion _startRot;
    public Vector3 velocity;

    public Transform[] wayPoints;

    [Header("Movement")]
    public float MaxSpeed = 10f;
    public float Gravity = 50f;
    public Vector3 MoveDir = Vector3.zero;

    [Header("Collision")]
    public LayerMask CollisionLayers;
    public float GroundCheckDistance = 0.15f;
    [HideInInspector] public CharacterController _charCtrl;
    [HideInInspector] public CollisionFlags _collision;

    //COLLISION
    public float rayRadius;
    [Range(0, 360)]
    public float rayAngle;
    public float numOfRays;
    public LayerMask obstacleMask;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        rend = GetComponent<Renderer>();

        _charCtrl = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _startPos = transform.position;
        _startRot = transform.rotation;

    }

    public override void Update()
    {
        base.Update();

        visible = CheckIfVisible();

        if (!visible)
        {
            TransitionTo<Hunt>();
        }
    }

    private bool CheckIfVisible()
    {
        if (rend.isVisible)
        {
            TransitionTo<Idle>();

            Debug.Log(Vector3.Distance(cam.transform.position, transform.position) / 10 + " : " + Vector3.Dot((cam.transform.position - transform.position).normalized, cam.transform.forward));
            if (Vector3.Dot((cam.transform.position - transform.position).normalized, cam.transform.forward) > Mathf.Lerp(-0.6f, -0.25f, Vector3.Distance(cam.transform.position, transform.position) / 10))
                return false;

            planes = GeometryUtility.CalculateFrustumPlanes(cam);

            foreach (BoxCollider coll in colls)
            {
                if (GeometryUtility.TestPlanesAABB(planes, coll.bounds))
                {
                    points[0] = transform.TransformPoint(coll.center);
                    points[1] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                    points[2] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                    points[3] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                    points[4] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                    points[5] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                    points[6] = transform.TransformPoint(coll.center + new Vector3(coll.size.x, coll.size.y, -coll.size.z) * 0.5f);
                    points[7] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                    points[8] = transform.TransformPoint(coll.center + new Vector3(-coll.size.x, coll.size.y, -coll.size.z) * 0.5f);

                    foreach (Vector3 point in points)
                        if (!Physics.Linecast(point, cam.transform.position, ignoreLayers))
                            return true;
                }
            }
        }

        return false;
    }

    public void UpdateCollisions()
    {
        if (Physics.BoxCast(colls[0].center, colls[0].size / 2, velocity, Quaternion.identity, maxDistance))
        {
            //om boxcasten ger en träff
            //kan bara färdas raycasthit.distance

            //Normalkraft:
            //Vector3.Dot(velocity, normal) * normal

        }

    }

    public void getNormalForce()
    {

    }

    public bool IsGrounded()
    {
        RaycastHit hitInfo;
        bool hit = Physics.SphereCast(transform.position, _charCtrl.radius, Vector3.down, out hitInfo, GroundCheckDistance, CollisionLayers, QueryTriggerInteraction.Ignore);

        return ((!hit || hitInfo.point.magnitude < MathHelper.FloatEpsilon) ? false : true);
    }

    public RaycastHit GroundCheck()
    {
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, _charCtrl.radius, Vector3.down, out hitInfo, GroundCheckDistance, CollisionLayers, QueryTriggerInteraction.Ignore);

        Debug.Log(hitInfo);
        return hitInfo;
    }

    public void ResetTransform()
    {
        transform.position = _startPos;
        transform.rotation = _startRot;
    }

}










