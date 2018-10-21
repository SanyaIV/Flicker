using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : EnemyStateController
{
    public Plane[] planes;
    public Camera cam;
    public Renderer rend;
    public Vector3[] points = new Vector3[9];
    private float maxDistance = 30f;

    //private List<RaycastHit> hits;
    public BoxCollider[] colls;

    public LayerMask ignoreLayers;
    public bool visible = false;
    public Transform player;
    public float speed;
    public float detectDistance = 20f;

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

        detectPlayer();
    }

    private bool CheckIfVisible()
    {
        if (rend.isVisible)
        {
            Debug.Log("Can see me!");
            TransitionTo<Idle>();
            Debug.Log("Transitioning to idle");
        }
        else
        {
            TransitionTo<Hunt>();
            Debug.Log("Transitioning to hunt");
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

    public void detectPlayer()
    {
        if(player.transform.position.magnitude - transform.position.magnitude < detectDistance)
        {
            Debug.Log("player detected");
        }
    }

}










