﻿using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Utility;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Player/States/Ground")]
public class GroundState : PlayerState {

    [Header("Sanity")]
    [SerializeField] private MinMaxFloat _clampSanitySpeedMultiplier = new MinMaxFloat(0.7f, 1f);

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
    [SerializeField] private float[] _walkJoggRunVolume = {0.5f, 0.7f, 1f};
    [SerializeField] private float _defaultEnemySoundDetectRange = 15f;
    public AudioClip[] footstepSounds;
    public AudioClip[] landingSounds;
    public AudioClip[] jumpSounds;

    [Header("Temporary Variables")]
    private RaycastHit _hit;
    private bool _firstRun = true;

    [Header("Goop")]
    [SerializeField] private float _goopSpeedMultiplier;
    [SerializeField] private float _goopSanityDrainMultiplier;

    private float friction { get { return acceleration / controller.maxSpeed; } }

    public override void Initialize(Controller owner)
    {
        base.Initialize(owner);

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

        if (_firstRun) //Workaround to avoid playing footsteps when the player starts.
        {
            _firstRun = false;
            return;
        }

        if (controller.previousState is AirState) {
            if (Time.time - timeOfAirJump <= maxGhostAirJumpTime)
                UpdateJump();

            controller.StartCoroutine(_jumpBob.DoBobCycle());
            PlayLandingSound();
        }
    }

    public override void Update()
    {
        base.Update();

        if (GameManager.pausePlayerMovement)
            return;

        if (Input.GetButtonDown("Jump"))
            UpdateJump();
        if (Input.GetKey(KeyCode.LeftShift) && !(controller.Input.z < 0))
        {
            controller.maxSpeed = runSpeed;
            controller.feetAudioSource.volume = _walkJoggRunVolume[2];
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            controller.maxSpeed = walkSpeed;
            controller.feetAudioSource.volume = _walkJoggRunVolume[0];
        }
        else
        {
            controller.maxSpeed = joggSpeed;
            controller.feetAudioSource.volume = _walkJoggRunVolume[1];
        }

        controller.maxSpeed *= Mathf.Clamp(controller.sanity.GetSanity(), _clampSanitySpeedMultiplier.Min, _clampSanitySpeedMultiplier.Max);

        if (controller.GoopCheck())
        {
            controller.sanity.DepleteSanity(_goopSanityDrainMultiplier);
            controller.maxSpeed *= _goopSpeedMultiplier;
        }  

        controller.collision = charCtrl.Move(moveDir * Time.deltaTime);
        GetInteractable();

        if (Debug.isDebugBuild && Input.GetKeyDown(KeyCode.G))
            controller.TransitionTo<GhostState>();
    }

    public override void FixedUpdate()
    {
        if (GameManager.pausePlayerMovement)
            return;

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
            controller.feetAudioSource.clip = audio[n];
            controller.feetAudioSource.PlayOneShot(controller.feetAudioSource.clip);
            audio[n] = audio[0];
            audio[0] = controller.feetAudioSource.clip;
        }
        else if(audio.Length == 1)
            controller.feetAudioSource.PlayOneShot(audio[0]);
        else
            return;

        if(Physics.OverlapSphere(transform.position, _defaultEnemySoundDetectRange * controller.feetAudioSource.volume, controller.enemyLayer).Length > 0 && !(controller.enemy.currentState is DoorBlock) && !(controller.enemy.currentState is Frozen))
            controller.enemy.TransitionTo<Hunt>();
            
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
}
