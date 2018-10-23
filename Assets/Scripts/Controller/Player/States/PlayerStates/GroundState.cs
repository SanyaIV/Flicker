using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Utility;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Player/States/Ground")]
public class GroundState : PlayerState {

    [Header("Constants")]
    [HideInInspector] public readonly MinMaxFloat CLAMP_SANITY_SPEED_MULTIPLIER = new MinMaxFloat(0.5f, 1f);

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float joggSpeed = 8f;
    public float runSpeed = 12f;
    public float acceleration = 100f;
    public float ExtraFriction = 30f;
    public float extraSecondaryFriction = 30f;
    public float stopSlidingLimit = 1.5f;
    [HideInInspector] public bool inputGotten = false;

    [Header("Ground")]
    public float stickToGroundForce = 10f;

    [Header("Jumping")]
    public MinMaxFloat jumpHeight;
    public float timeToJumpApex = 0.5f;
    public float initialJumpDistance = 0.15f;
    public float maxGhostAirJumpTime = 0.2f;
    [HideInInspector] public MinMaxFloat jumpVelocity;
    [HideInInspector] public float timeOfAirJump;
    
    [Header("HeadBob")]
    [SerializeField] private bool _useHeadBob;
    [SerializeField] private CurveControlledBob _headBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob _jumpBob = new LerpControlledBob();
    private Vector3 _originalCamPos;

    [Header("Footstep")]
    [SerializeField] private float _stepInterval;
    [SerializeField] [Range(0f, 1f)]private float _runStepLengthen;
    private float _stepCycle;
    private float _nextStep;

    [Header("Audio")]
    public AudioClip[] footstepSounds;
    public AudioClip[] landingSounds;
    public AudioClip[] jumpSounds;

    [Header("Interactable")]
    [SerializeField] private float _range;
    [SerializeField] private LayerMask _interactableLayerMask;
    private Text _interactableText;

    [Header("Temporary Variables")]
    private RaycastHit _hit;

    [Header("Goop")]
    [SerializeField] private float _goopSpeedMultiplier;
    [SerializeField] private float _goopSanityDrainMultiplier;

    private float friction { get { return acceleration / controller.maxSpeed; } }

    public override void Initialize(Controller owner)
    {
        base.Initialize(owner);

        GameObject interactText = GameObject.Find("InteractText");
        if(interactText)
            _interactableText = interactText.GetComponent<Text>();

        controller.gravity = (2 * jumpHeight.Max) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity.Max = controller.gravity * timeToJumpApex;
        jumpVelocity.Min = Mathf.Sqrt(2 * controller.gravity * jumpHeight.Min);
        timeOfAirJump = -100f;

        //HeadBob
        _originalCamPos = controller.cam.transform.localPosition;
        _headBob.Setup(controller.cam, _stepInterval);
        _stepCycle = 0f;
        _nextStep = _stepCycle / 2f;
    }

    public override void Enter()
    {
        inputGotten = false;

        if (controller.previousState is AirState) {
            if (Time.time - timeOfAirJump <= maxGhostAirJumpTime)
                UpdateJump();

            controller.StartCoroutine(_jumpBob.DoBobCycle());
            PlayLandingSound();
        }
    }

    public override void Exit()
    {
        transform.parent = null;
        if(_interactableText)
            _interactableText.text = "";
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Jump"))
            UpdateJump();
        if (Input.GetKey(KeyCode.LeftShift) && !(controller.Input.z < 0))
            controller.maxSpeed = runSpeed;
        else if (Input.GetKey(KeyCode.LeftControl))
            controller.maxSpeed = walkSpeed;
        else
            controller.maxSpeed = joggSpeed;

        controller.maxSpeed *= Mathf.Clamp(controller.sanity.GetSanity(), CLAMP_SANITY_SPEED_MULTIPLIER.Min, CLAMP_SANITY_SPEED_MULTIPLIER.Max);

        if (controller.GoopCheck())
        {
            controller.sanity.DepleteSanity(_goopSanityDrainMultiplier);
            controller.maxSpeed *= _goopSpeedMultiplier;
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1"))
            Interact();
            

        controller.collision = charCtrl.Move(moveDir * Time.deltaTime);
        GetInteractible();
    }

    public override void FixedUpdate()
    {
        _hit = controller.GroundCheck();

        UpdateMovement();
        UpdateFriction();

        if (_hit.collider != null)
            controller.moveDir.y = -stickToGroundForce;
        else
            controller.TransitionTo<AirState>();

        StopSliding();
        ProgressStepCycle();
        UpdateCameraPosition();
    }

    private void UpdateMovement()
    {
        Vector3 input = controller.Input;
        input = transform.forward * input.z + transform.right * input.x;

        if (_hit.collider != null && _hit.collider.CompareTag("Escape Pod"))
            transform.parent = _hit.collider.gameObject.transform.parent;
        else
            transform.parent = null;

        input = Vector3.ProjectOnPlane(input, _hit.normal).normalized;

        moveDir += input * acceleration;

        if(input.magnitude > 0)
        {
            inputGotten = true;

            if (moveDir.magnitude > controller.maxSpeed)
                moveDir = moveDir.normalized * controller.maxSpeed;
        }
    }

    private void UpdateFriction()
    {
        float extraFriction = inputGotten ? controller.Input.magnitude < controller.inputRequiredToMove ? ExtraFriction : 0.0f : extraSecondaryFriction;
        float friction = Mathf.Clamp01((this.friction + extraFriction) * Time.fixedDeltaTime);
        moveDir -= moveDir * friction;
    }

    private void StopSliding()
    {
        if ((Mathf.Abs(moveDir.x) + Mathf.Abs(moveDir.z)) < stopSlidingLimit && controller.Input.magnitude < controller.inputRequiredToMove)
            moveDir = Vector3.zero;
    }

    public void UpdateJump()
    {
        PlayAudio(ref jumpSounds);
        transform.position += Vector3.up * initialJumpDistance;
        controller.moveDir.y = jumpVelocity.Max;
        controller.GetState<AirState>().ghostJumpAllowed = false;
        controller.GetState<AirState>().canCancelJump = true;
        if(!(controller.currentState is AirState))controller.TransitionTo<AirState>();
    }

    private void ProgressStepCycle()
    {
        if((Mathf.Abs(moveDir.x) + Mathf.Abs(moveDir.z)) > 0)
            _stepCycle += (controller.charCtrl.velocity.magnitude + (controller.maxSpeed * (controller.maxSpeed == runSpeed ? _runStepLengthen : 1f))) * Time.fixedDeltaTime;

        if (!(_stepCycle > _nextStep))
            return;

        _nextStep = _stepCycle + _stepInterval;

        PlayAudio(ref footstepSounds);
    }

    private void PlayAudio(ref AudioClip[] audio)
    {
        if(audio.Length > 1)
        {
            int n = Random.Range(1, audio.Length);
            controller.audioSource.clip = audio[n];
            controller.audioSource.PlayOneShot(controller.audioSource.clip);
            audio[n] = audio[0];
            audio[0] = controller.audioSource.clip;
        }
        else if(audio.Length == 1)
            controller.audioSource.PlayOneShot(audio[0]);
        else
            return;
    }

    private void PlayLandingSound()
    {
        PlayAudio(ref landingSounds);
        _nextStep = _stepCycle + 0.5f;
    }

    private void UpdateCameraPosition()
    {
        Vector3 newCameraPosition;
        if (!_useHeadBob)
            return;

        if((Mathf.Abs(moveDir.x) + Mathf.Abs(moveDir.z)) > 0)
        {
            controller.cam.transform.localPosition = _headBob.DoHeadBob(controller.charCtrl.velocity.magnitude + (controller.maxSpeed * (controller.maxSpeed == runSpeed ? _runStepLengthen : 1f)));
            newCameraPosition = controller.cam.transform.localPosition;
            newCameraPosition.y = controller.cam.transform.localPosition.y - _jumpBob.Offset();
        }
        else
        {
            newCameraPosition = controller.cam.transform.localPosition;
            newCameraPosition.y = _originalCamPos.y - _jumpBob.Offset();
        }

        controller.cam.transform.localPosition = newCameraPosition;
    }

    private Interactable GetInteractible()
    {
        RaycastHit hit;
        if (Physics.Raycast(controller.cam.transform.position, controller.cam.transform.forward, out hit, _range, _interactableLayerMask))
            if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                Interactable actable = hit.transform.GetComponent<Interactable>();

                if (!actable)
                    return null;

                if(_interactableText)
                    _interactableText.text = actable.ActionType() + " " + actable.GetName();

                return actable;
            }

        if(_interactableText)
            _interactableText.text = "";

        return null;
    }

    private void Interact()
    {
        Interactable actable = GetInteractible();
        if (actable)
            actable.Interact(controller);
    }
}
