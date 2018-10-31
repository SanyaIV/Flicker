using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (CharacterController))]
public class PlayerController : Controller {

    private Vector3 _startPos;
    private Quaternion _startRot;

    [Header("Status")]
    [HideInInspector]public Sanity sanity;

    [Header("Movement")]
    public float gravity = 50f;
    public Vector3 moveDir = Vector3.zero;
    [Range(0f, 1f)] public float inputRequiredToMove = 0.5f;
    [HideInInspector] public float maxSpeed = 10f;

    [Header("Collision")]
    public LayerMask collisionLayers;
    public LayerMask goopLayers;
    public float groundCheckDistance = 0.15f;
    [HideInInspector] public CharacterController charCtrl;
    [HideInInspector] public CollisionFlags collision;

    [Header("Camera")]
    public Transform firstPersonCamera;
    public Camera cam;

    [Header("Audio")]
    [SerializeField] public AudioSource audioSource;

    [Header("Inventory")]
    [SerializeField] private List<string> _passcards;
    [SerializeField] private List<string> _escapePodParts;

    [Header("Save")]
    private List<string> _savedPasscards;
    private List<string> _savedEscapePodParts;

    public override void Awake()
    {
        base.Awake();

        _passcards = new List<string>();
        _escapePodParts = new List<string>();

        if(!audioSource)
            audioSource = GetComponent<AudioSource>();

        sanity = GetComponent<Sanity>();
    }

    public override void Start()
    {
        base.Start();

        _startPos = transform.position;
        _startRot = transform.rotation;
        charCtrl = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (firstPersonCamera == null)
            firstPersonCamera = GameObject.FindWithTag("FirstPersonCamera").transform;
        if (cam == null)
            cam = Camera.main;

        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(ReloadSave);
    }

    /*private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

        if (rb != null && !rb.isKinematic)
        {
            Vector3 direction = -hit.normal;
            direction.y = MoveDir.normalized.y;
            rb.AddForce(direction.normalized * MoveDir.magnitude * PushForceMultiplier, ForceMode.Impulse);
            CameraShake.SetIntensity(0.5f);
        }
    }*/

    public Vector3 Input
    {
        get
        {
            return new Vector3(UnityEngine.Input.GetAxisRaw("Horizontal"), 0.0f, UnityEngine.Input.GetAxisRaw("Vertical"));
        }
    }

    public bool IsGrounded()
    {
        RaycastHit hitInfo;
        bool hit = Physics.SphereCast(transform.position, charCtrl.radius, Vector3.down, out hitInfo, groundCheckDistance, collisionLayers, QueryTriggerInteraction.Ignore);

        return ((!hit || hitInfo.point.magnitude < MathHelper.FloatEpsilon) ? false : true);
    }

    public RaycastHit GroundCheck()
    {
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, charCtrl.radius, Vector3.down, out hitInfo, groundCheckDistance, collisionLayers, QueryTriggerInteraction.Ignore);

        return hitInfo;
    }

    public bool GoopCheck()
    {
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, charCtrl.radius, Vector3.down, out hitInfo, groundCheckDistance, goopLayers, QueryTriggerInteraction.Collide);

        return hitInfo.collider;
    }

    public void ResetTransform()
    {
        transform.position = _startPos;
        transform.rotation = _startRot;
    }

    public void AddPasscard(string passcard)
    {
        if(!_passcards.Contains(passcard))
            _passcards.Add(passcard);
    }

    public bool HasPasscard(string passcard)
    {
        return _passcards.Contains(passcard);
    }

    public void AddEscapePodPart(string escapePodPart)
    {
        if (!_escapePodParts.Contains(escapePodPart))
            _escapePodParts.Add(escapePodPart);
    }

    public bool HasEscapePodPart(string escapePodPart)
    {
        return _escapePodParts.Contains(escapePodPart);
    }

    public bool HasSavedEscapePodPart(string escapePodPart)
    {
        return _savedEscapePodParts.Contains(escapePodPart);
    }

    public void Save()
    {
        _savedEscapePodParts = new List<string>(_escapePodParts);
        _savedPasscards = new List<string>(_passcards);
    }

    public void ReloadSave()
    {
        _escapePodParts = new List<string>(_savedEscapePodParts);
        _passcards = new List<string>(_savedPasscards);
    }
}
