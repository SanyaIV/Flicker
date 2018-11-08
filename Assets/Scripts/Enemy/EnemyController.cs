using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : EnemyStateController
{
    public Plane[] planes;

    [SerializeField]
    public float _distanceToDepleteSanity;
    public float depletionAmount;

    //Array för fiendens modeller
    //public string[] posePlaceHolders;
    
    public Camera cam;
    public Renderer rend;
    public Vector3[] points = new Vector3[9];
    private float _maxDistance = 30f;
    public NavMeshAgent _navMeshAgent;

    public float detectDistance;

    public LayerMask doorLayer;

    //private List<RaycastHit> hits;
    public BoxCollider[] colls;

    public LayerMask ignoreLayers;
    public bool visible = false;
    public Transform player;
    public Sanity sanity;
    public float speed;

    private Vector3 _startPos;
    private Quaternion _startRot;
    private int _aggression;
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

    [Header("Footstep")]
    [SerializeField] private float _stepInterval;
    [SerializeField] [Range(0f, 1f)] private float _runStepLengthen;
    private float _stepCycle;
    private float _nextStep;

    [Header("Audio")]
    [SerializeField] public BasicAudio basicAudio;
    public AudioClip[] footstepSounds;
    public AudioClip doorPound;

    public AudioSource audioSource;

    public bool hittingDoor = false;
    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        _stepCycle = 0f;
        _nextStep = _stepCycle / 2f;

        rend = GetComponent<Renderer>();
        cam = Camera.main;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.acceleration = 60f;
        _charCtrl = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        sanity = GameObject.FindGameObjectWithTag("Player").GetComponent<Sanity>();

        AudioSource audioSource = GetComponent<AudioSource>();

        _startPos = transform.position;
        _startRot = transform.rotation;

        _aggression = 0;

        TransitionTo<Patrol>();
    }

    public override void Update()
    {
        base.Update();
        if (hittingDoor) return;

        if (!CheckIfEnemyIsVisible())
        {
            Debug.Log("is hidden");
            UpdateIfHidden();
            
        }
        else
        {
            Debug.Log("Is visible");
            audioSource.Stop();
            UpdateIfVisible();
        }
    }

    private void UpdateIfVisible()
    {
        _navMeshAgent.isStopped = true;

        if (!(CurrentState is Idle))
            Debug.Log("Transitioning to idle");
        TransitionTo<Idle>();
   
        sanity.DepleteSanity();
    }

    private void UpdateIfHidden()
    {
        _navMeshAgent.isStopped = false;

        if (HitDoor() && !hittingDoor)
        {
            StartCoroutine(PoundOnDoor());
        }

        ProgressStepCycle();
        UpdateAggression();
    }

    private bool HitDoor()
    {
        RaycastHit hit = new RaycastHit();
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.cyan);
        return (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), 0.5f, LayerMask.GetMask("Door")));   
    }

    public bool CheckIfEnemyIsVisible()
    {
        if (rend.isVisible) //Check if Unity thinks the renderer is visible (Not perfect but works as a quick and easy out in case it's not)
        {
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

    //Stop enemy and play pound on door audio
    public IEnumerator PoundOnDoor()
    {
        hittingDoor = true;
        _navMeshAgent.isStopped = true;
        audioSource.PlayOneShot(doorPound);
        Debug.Log("Play sound");

        yield return new WaitForSeconds(5);
        TransitionTo<Patrol>();
        _navMeshAgent.isStopped = false;
        yield return new WaitForSeconds(6);
        hittingDoor = false;
    }

    private void ProgressStepCycle()
    {
        if (_navMeshAgent.velocity.magnitude > 1f)
            _stepCycle += (velocity.magnitude + (speed * 1f)) * Time.fixedDeltaTime;

        if (!(_stepCycle > _nextStep))
            return;

        _nextStep = _stepCycle + _stepInterval;

        PlayAudio(ref footstepSounds);
       
    }

    private void PlayAudio(ref AudioClip[] audio)
    {
        if (audio.Length > 1)
        {
            int n = Random.Range(1, audio.Length);
            audioSource.clip = audio[n];
            audioSource.PlayOneShot(audioSource.clip);
            audio[n] = audio[0];
            audio[0] = audioSource.clip;
        }
        else if (audio.Length == 1)
            audioSource.PlayOneShot(audio[0]);
        else
            return;
    }

    public bool PlayerClose()
    {
        return (Vector3.Distance(player.position, transform.position) < detectDistance);
    }

    private void UpdateAggression()
    {
        if (Time.time > 250)
        {
            _aggression = 2;
        }
            
        if (Time.time > 500)
        {
            _aggression = 3;
        }
            
    }

    /*private void ChangePose()
    {
        string currentPose = posePlaceHolders[0];
        for(int i = 0; i < posePlaceHolders.Length; i++)
        {
            currentPose = posePlaceHolders[0 + i];
            Debug.Log(currentPose);
        }
    }*/

}










