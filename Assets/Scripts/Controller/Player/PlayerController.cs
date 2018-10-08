using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (CharacterController))]
public class PlayerController : Controller {

    private Vector3 _startPos;
    private Quaternion _startRot;

    [Header("Movement")]
    public float MaxSpeed = 10f;
    public float Gravity = 50f;
    public Vector3 MoveDir = Vector3.zero;
    [Range(0f, 1f)] public float InputRequiredToMove = 0.5f;

    [Header("Collision")]
    public LayerMask CollisionLayers;
    public float GroundCheckDistance = 0.15f;
    [Range(0f, 1f)] public float CollisionBounceMultiplier; //Delete?
    [HideInInspector] public CharacterController _charCtrl;
    [HideInInspector] public CollisionFlags _collision;

    [Header("Camera")]
    public Transform cam;

    [Header("Audio")]
    [HideInInspector] public AudioSource _audioSource;

    public override void Awake()
    {
        base.Awake();

        _audioSource = GetComponent<AudioSource>();
    }

    public override void Start()
    {
        base.Start();

        _startPos = transform.position;
        _startRot = transform.rotation;
        _charCtrl = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (cam == null)
            cam = GameObject.FindWithTag("FirstPersonCamera").transform;
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
        bool hit = Physics.SphereCast(transform.position, _charCtrl.radius, Vector3.down, out hitInfo, GroundCheckDistance, CollisionLayers, QueryTriggerInteraction.Ignore);

        return ((!hit || hitInfo.point.magnitude < MathHelper.FloatEpsilon) ? false : true);
    }

    public RaycastHit GroundCheck()
    {
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, _charCtrl.radius, Vector3.down, out hitInfo, GroundCheckDistance, CollisionLayers, QueryTriggerInteraction.Ignore);

        return hitInfo;
    }

    public void ResetTransform()
    {
        transform.position = _startPos;
        transform.rotation = _startRot;
    }
}
