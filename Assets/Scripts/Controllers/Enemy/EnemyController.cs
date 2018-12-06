using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Controller
{
    [Header("Target")]
    [HideInInspector] public Transform player;
    [HideInInspector] public Sanity sanity;

    [Header("Visibility Check")]
    [SerializeField] private LayerMask _blockVisibilityLayers;
    private List<GameObject> _models = new List<GameObject>();
    private List<BoxCollider> _colls = new List<BoxCollider>();
    private MeshRenderer _rend;
    private Vector3[] _points = new Vector3[9];
    private Camera _cam;
    private Plane[] _planes;
    private bool _visible = false;

    [Header("Detection")]
    public float detectDistance;

    [Header("Nav Mesh Agent")]
    [HideInInspector] public Dictionary<string, List<Transform>> wayPoints = new Dictionary<string, List<Transform>>();
    public NavMeshAgent navMeshAgent;

    [Header("Doors")]
    [SerializeField] private float _doorCheckDistance = 0.5f;
    [SerializeField] private LayerMask _doorLayer;

    [Header("Footsteps")]
    [SerializeField] private float _speed;
    [SerializeField] private float _stepInterval;
    private float _stepCycle;
    private float _nextStep;

    [Header("Audio")]
    [SerializeField] private BasicAudio _footstepBasicAudio;
    public BasicAudio basicAudio;

    public override void Awake()
    {
        base.Awake();

        AddWayPoints();
        AddModels();
        DisableAllModels();
        SwitchModel();

        _cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        sanity = player.GetComponent<Sanity>();
    }

    public override void Start()
    {
        base.Start();

        _stepCycle = 0f;
        _nextStep = _stepCycle / 2f;

        navMeshAgent.acceleration = 60f;

        TransitionTo<Patrol>();

        GameManager.AddSaveEvent(SaveOrReload);
        GameManager.AddLateReloadEvent(SaveOrReload);
    }

    public override void Update()
    {
        CheckIfVisible();

        if (HitDoor() && !(currentState is Frozen) && !(currentState is DoorBlock) && !GetState<DoorBlock>().IsCoolingDown())
            TransitionTo<DoorBlock>();

        base.Update();
    }

    public bool HitDoor()
    {
        RaycastHit hit = new RaycastHit();
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * _doorCheckDistance, Color.cyan);

        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _doorCheckDistance, _doorLayer))
            if (hit.transform.CompareTag("Door"))
                return true;

        return false;
    }

    public bool CheckIfVisible()
    {
        if (_rend.isVisible) //Check if Unity thinks the renderer is visible (Not perfect but works as a quick and easy out in case it's not)
        {
            if (Vector3.Dot((_cam.transform.position - transform.position).normalized, _cam.transform.forward) > Mathf.Lerp(-0.6f, -0.25f, Vector3.Distance(_cam.transform.position, transform.position) / 10)) //Bad attempt at checking if the player is looking towards the enemy through the dot products of directions
                return _visible = false; //Set visible to false and return visible (which is false)

            int taskNumber = Time.frameCount % _colls.Count(); //Run one box per frame
            
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

    public void ProgressStepCycle()
    {
        if (navMeshAgent.velocity.magnitude > 1f)
            _stepCycle += (navMeshAgent.velocity.magnitude + _speed) * Time.deltaTime;

        if (!(_stepCycle > _nextStep))
            return;

        _nextStep = _stepCycle + _stepInterval;

        _footstepBasicAudio.PlayAudio(true);
    }

    public bool PlayerClose()
    {
        return (Vector3.Distance(player.position, transform.position) < detectDistance);
    }

    public bool PlayerVisible()
    {
        if(!Physics.Linecast(transform.position, player.position, _blockVisibilityLayers))
            return true;
        else
            return false;
    }

    private void AddWayPoints()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Spawn Point"))
        {
            SpawnPoint sp = go.GetComponent<SpawnPoint>();
            if (sp != null)
            {
                if (!wayPoints.ContainsKey(sp.GetArea()))
                    wayPoints.Add(sp.GetArea(), new List<Transform>());

                wayPoints[sp.GetArea()].Add(go.transform);
            }
        }
    }

    public Transform GetWayPointInArea(string area)
    {
        if (wayPoints.ContainsKey(area))
            return wayPoints[area][Random.Range(0, wayPoints[area].Count)];

        Debug.LogError("Tried to get way point for area: " + area + " But no such area exists in wayPoints.");
        return null;
    }

    public Transform GetSpawnPointInArea(string area)
    {
        if (wayPoints.ContainsKey(area))
        {
            List<Transform> potentialSpawnPoints = new List<Transform>();
            foreach (Transform trans in wayPoints[area])
                if (trans.GetComponent<SpawnPoint>().GetSpawnAllowed())
                    potentialSpawnPoints.Add(trans);

            if (potentialSpawnPoints.Count > 0)
                return potentialSpawnPoints[Random.Range(0, potentialSpawnPoints.Count)];

            Debug.LogError("Tried to get spawn point in area: " + area + " But no spawn point in the specified area was found.");
            return null;
        }

        Debug.LogError("Tried to get spawn point for area: " + area + " But no such area exists in wayPoints.");
        return null;
    }

    private void AddModels()
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer rend in meshes)
            _models.Add(rend.gameObject);
    }

    public void SwitchModel()
    {
        int n = Random.Range(1, _models.Count());
        GameObject tmp = _models[0];
        _models[0] = _models[n];
        _models[n] = tmp;
        _models[0].SetActive(true);
        _models[n].SetActive(false);
        _colls = new List<BoxCollider>(_models[0].GetComponentsInChildren<BoxCollider>());
        _rend = _models[0].GetComponent<MeshRenderer>();
    }

    private void DisableAllModels()
    {
        foreach (GameObject go in _models)
            go.SetActive(false);
    }

    private void EnableAllModels()
    {
        foreach (GameObject go in _models)
            go.SetActive(true);
    }

    public bool IsVisible()
    {
        return _visible;
    }

    public void StopAudio()
    {
        _footstepBasicAudio.Stop();
        basicAudio.Pause();
    }

    public int GetThreatLevel()
    {
        bool dandelion = false;
        bool crimson = false;

        foreach(Area area in AreaTracker.GetVisitedAreas())
        {
            if (area.GetName() == "Dandelion")
                dandelion = true;
            else if (area.GetName() == "Crimson")
                crimson = true;
        }

        if (crimson && dandelion)
            return 2;
        else if (crimson || dandelion)
            return 1;
        else
            return 0;
    }

    public string GetTargetArea()
    {
        int threatLevel = GetThreatLevel();

        if (AreaTracker.GetCurrentPlayerArea() != null && AreaTracker.GetCurrentEnemyArea() != null)
        {
            if (threatLevel > 0 && AreaTracker.GetCurrentPlayerArea().GetName() != "Escape Pod")
                return AreaTracker.GetCurrentPlayerArea().GetName();
            else if (threatLevel > 0)
                return "Indigo";
        }

        return "Dandelion";
    }

    public void SaveOrReload()
    {
        navMeshAgent.Warp(GetSpawnPointInArea(GetTargetArea()).position);
        TransitionTo<Patrol>();
    }

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Light"))
            coll.GetComponent<LightController>().EnemyFlickerOn();
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Light"))
            coll.GetComponent<LightController>().EnemyFlickerOff();
    }
}