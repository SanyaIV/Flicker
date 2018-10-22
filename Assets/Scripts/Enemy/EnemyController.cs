using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : EnemyStateController
{
    public Plane[] planes;
    public Camera cam;
    public Renderer rend;
    public Vector3[] points = new Vector3[9];
    private float maxDistance = 30f;
    public NavMeshAgent _navMeshAgent;

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

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        rend = GetComponent<Renderer>();
        cam = Camera.main;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _charCtrl = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _startPos = transform.position;
        _startRot = transform.rotation;

    }

    public override void Update()
    {
        base.Update();

        if (!CheckIfVisible())
        {
            //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            TransitionTo<Hunt>();
        }

        detectPlayer();
    }

    private bool CheckIfVisible()
    {
        if (rend.isVisible) //Check if Unity thinks the renderer is visible (Not perfect but works as a quick and easy out in case it's not)
        {
            _navMeshAgent.isStopped = true;
           
            if (Vector3.Dot((cam.transform.position - transform.position).normalized, cam.transform.forward) > Mathf.Lerp(-0.6f, -0.25f, Vector3.Distance(cam.transform.position, transform.position) / 10)) //Bad attempt at checking if the player is looking towards the enemy through the dot products of directions
                return visible = false; //Set visible to false and return visible (which is false)

            int taskNumber = Time.frameCount % colls.Length; //Run one box per frame

            if (taskNumber == 0)
                visible = false; //Reset visibility status at start of the "loop"
            if (visible)
                return true; //If visible is true then just return since it means the enemy has been seen for this round of the "loop"

            planes = GeometryUtility.CalculateFrustumPlanes(cam); //Get the frustum planes of the camera

            if (GeometryUtility.TestPlanesAABB(planes, colls[taskNumber].bounds)) //Test if the current boxcollider is within the camera's frustum
            {
                BoxCollider coll = colls[taskNumber]; //Get the current boxcollider
                points[0] = transform.TransformPoint(colls[taskNumber].center);
                points[1] = transform.TransformPoint(colls[taskNumber].center + new Vector3(coll.size.x, -coll.size.y, coll.size.z) * 0.5f); //One corner of the boxcollider
                points[2] = transform.TransformPoint(colls[taskNumber].center + new Vector3(coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                points[3] = transform.TransformPoint(colls[taskNumber].center + new Vector3(-coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                points[4] = transform.TransformPoint(colls[taskNumber].center + new Vector3(-coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                points[5] = transform.TransformPoint(colls[taskNumber].center + new Vector3(coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                points[6] = transform.TransformPoint(colls[taskNumber].center + new Vector3(coll.size.x, coll.size.y, -coll.size.z) * 0.5f);
                points[7] = transform.TransformPoint(colls[taskNumber].center + new Vector3(-coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                points[8] = transform.TransformPoint(colls[taskNumber].center + new Vector3(-coll.size.x, coll.size.y, -coll.size.z) * 0.5f);

                foreach (Vector3 point in points) //Loop through the points array
                    if (!Physics.Linecast(point, cam.transform.position, ignoreLayers)) //Linecast between the enemy and the camera to check if there is anything in the way
                        return visible = true; //If there is nothing in the way then the enemy is visible, so set visible to true and return it.        
            }
        }

        return visible = false; //Set visible to false and return it
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










