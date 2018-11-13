using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : EnemyStateController
{
    [Header("Target")]
    public Transform player;
    public Sanity sanity;

    [Header("Visibility Check")]
    [SerializeField] private BoxCollider[] _colls;
    [SerializeField] private LayerMask _blockVisibilityLayers;
    private Camera _cam;
    private Renderer _rend;
    private Plane[] _planes;
    private Vector3[] _points = new Vector3[9];
    private bool _visible = false;

    [Header("Detection")]
    public float detectDistance;

    [Header("Sanity")]
    [SerializeField] private float _depletionAmount;

    [Header("Nav Mesh Agent")]
    [SerializeField] private NavMeshAgent _navMeshAgent;
    public Transform[] wayPoints;

    [Header("Doors")]
    [SerializeField] private LayerMask _doorLayer;
    [SerializeField] private bool _hittingDoor = false;

    [Header("Footsteps")]
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _velocity;
    [SerializeField] private float _stepInterval;
    [SerializeField] [Range(0f, 1f)] private float _runStepLengthen;
    private float _stepCycle;
    private float _nextStep;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _footstepSounds;
    [SerializeField] private AudioClip[] _doorPound;
    public BasicAudio basicAudio;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        _stepCycle = 0f;
        _nextStep = _stepCycle / 2f;

        _rend = GetComponent<Renderer>();
        _cam = Camera.main;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.acceleration = 60f;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        sanity = GameObject.FindGameObjectWithTag("Player").GetComponent<Sanity>();

        _audioSource = GetComponent<AudioSource>();

        TransitionTo<Patrol>();
    }

    public override void Update()
    {
        base.Update();
        //if (hittingDoor) return;

        if (!CheckIfEnemyIsVisible())
        {
            UpdateIfHidden();
        }
        else
        {
            _audioSource.Stop();
            UpdateIfVisible();
        }
    }

    private void UpdateIfVisible()
    {
        _navMeshAgent.isStopped = true;

        if (!(CurrentState is Idle))
            TransitionTo<Idle>();
   
        sanity.DepleteSanity(_depletionAmount);
    }

    private void UpdateIfHidden()
    {
        _navMeshAgent.isStopped = false;

        if (HitDoor() && !_hittingDoor)
        {
            StartCoroutine(PoundOnDoor());
        }

        ProgressStepCycle();
    }

    private bool HitDoor()
    {
        RaycastHit hit = new RaycastHit();
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.cyan);
        return (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 0.5f, _doorLayer));
    }

    public bool CheckIfEnemyIsVisible()
    {
        if (_rend.isVisible) //Check if Unity thinks the renderer is visible (Not perfect but works as a quick and easy out in case it's not)
        {
            if (Vector3.Dot((_cam.transform.position - transform.position).normalized, _cam.transform.forward) > Mathf.Lerp(-0.6f, -0.25f, Vector3.Distance(_cam.transform.position, transform.position) / 10)) //Bad attempt at checking if the player is looking towards the enemy through the dot products of directions
                return _visible = false; //Set visible to false and return visible (which is false)

            int taskNumber = Time.frameCount % _colls.Length; //Run one box per frame

            if (taskNumber == 0)
                _visible = false; //Reset visibility status at start of the "loop"
            if (_visible)
                return true; //If visible is true then just return since it means the enemy has been seen for this round of the "loop"

            _planes = GeometryUtility.CalculateFrustumPlanes(_cam); //Get the frustum planes of the camera

            if (GeometryUtility.TestPlanesAABB(_planes, _colls[taskNumber].bounds)) //Test if the current boxcollider is within the camera's frustum
            {
                BoxCollider coll = _colls[taskNumber]; //Get the current boxcollider
                _points[0] = transform.TransformPoint(_colls[taskNumber].center);
                _points[1] = transform.TransformPoint(_colls[taskNumber].center + new Vector3(coll.size.x, -coll.size.y, coll.size.z) * 0.5f); //One corner of the boxcollider
                _points[2] = transform.TransformPoint(_colls[taskNumber].center + new Vector3(coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                _points[3] = transform.TransformPoint(_colls[taskNumber].center + new Vector3(-coll.size.x, -coll.size.y, coll.size.z) * 0.5f);
                _points[4] = transform.TransformPoint(_colls[taskNumber].center + new Vector3(-coll.size.x, -coll.size.y, -coll.size.z) * 0.5f);
                _points[5] = transform.TransformPoint(_colls[taskNumber].center + new Vector3(coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                _points[6] = transform.TransformPoint(_colls[taskNumber].center + new Vector3(coll.size.x, coll.size.y, -coll.size.z) * 0.5f);
                _points[7] = transform.TransformPoint(_colls[taskNumber].center + new Vector3(-coll.size.x, coll.size.y, coll.size.z) * 0.5f);
                _points[8] = transform.TransformPoint(_colls[taskNumber].center + new Vector3(-coll.size.x, coll.size.y, -coll.size.z) * 0.5f);

                foreach (Vector3 point in _points) //Loop through the points array
                    if (!Physics.Linecast(point, _cam.transform.position, _blockVisibilityLayers)) //Linecast between the enemy and the camera to check if there is anything in the way
                        return _visible = true; //If there is nothing in the way then the enemy is visible, so set visible to true and return it.        
            }
        }

        return _visible = false; //Set visible to false and return it
    }

    //Stop enemy and play pound on door audio
    public IEnumerator PoundOnDoor()
    {
        _hittingDoor = true;
        _navMeshAgent.isStopped = true;
        _audioSource.PlayOneShot(_doorPound[0]);

        yield return new WaitForSeconds(5);
        TransitionTo<Patrol>();
        _navMeshAgent.isStopped = false;
        yield return new WaitForSeconds(6);
        _hittingDoor = false;
    }

    private void ProgressStepCycle()
    {
        if (_navMeshAgent.velocity.magnitude > 1f)
            _stepCycle += (_velocity.magnitude + (_speed * 1f)) * Time.deltaTime;

        if (!(_stepCycle > _nextStep))
            return;

        _nextStep = _stepCycle + _stepInterval;

        PlayAudio(ref _footstepSounds);
    }

    private void PlayAudio(ref AudioClip[] audio)
    {
        if (audio.Length > 1)
        {
            int n = Random.Range(1, audio.Length);
            _audioSource.clip = audio[n];
            _audioSource.PlayOneShot(_audioSource.clip);
            audio[n] = audio[0];
            audio[0] = _audioSource.clip;
        }
        else if (audio.Length == 1)
            _audioSource.PlayOneShot(audio[0]);
        else
            return;
    }

    public bool PlayerClose()
    {
        return (Vector3.Distance(player.position, transform.position) < detectDistance);
    }
}










