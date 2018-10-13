using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/States/Ground")]
public class GroundState : PlayerState {

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float joggSpeed = 8f;
    public float RunSpeed = 12f;
    public float Acceleration = 100f;
    public float ExtraFriction = 30f;
    public float ExtraSecondaryFriction = 30f;
    public float StopSlidingLimit = 1.5f;
    [HideInInspector] public bool InputGotten = false;

    [Header("Ground")]
    public float StickToGroundForce = 10f;

    [Header("Jumping")]
    public MinMaxFloat JumpHeight;
    public float TimeToJumpApex = 0.5f;
    public float InitialJumpDistance = 0.15f;
    public float MaxGhostAirJumpTime = 0.2f;
    [HideInInspector] public MinMaxFloat JumpVelocity;
    [HideInInspector] public float TimeOfAirJump;
    public AudioClip[] JumpSounds;

    [Header("Temporary Variables")]
    private RaycastHit hit;

    private float Friction { get { return Acceleration / _controller.MaxSpeed; } }

    public override void Initialize(Controller owner)
    {
        base.Initialize(owner);

        _controller.Gravity = (2 * JumpHeight.Max) / Mathf.Pow(TimeToJumpApex, 2);
        JumpVelocity.Max = _controller.Gravity * TimeToJumpApex;
        JumpVelocity.Min = Mathf.Sqrt(2 * _controller.Gravity * JumpHeight.Min);
        TimeOfAirJump = -100f;
    }

    public override void Enter()
    {
        InputGotten = false;

        if (_controller.PreviousState is AirState && Time.time - TimeOfAirJump <= MaxGhostAirJumpTime)
            UpdateJump();
    }

    public override void Exit()
    {
        transform.parent = null;
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetButtonDown("Jump"))
            UpdateJump();
        if (Input.GetKey(KeyCode.LeftShift) && !(_controller.Input.z < 0))
            _controller.MaxSpeed = RunSpeed;
        else if (Input.GetKey(KeyCode.LeftControl))
            _controller.MaxSpeed = walkSpeed;
        else
            _controller.MaxSpeed = joggSpeed;

        _controller._collision = _charCtrl.Move(MoveDir * Time.deltaTime);
    }

    public override void FixedUpdate()
    {
        hit = _controller.GroundCheck();

        UpdateMovement();
        UpdateFriction();

        if (hit.collider != null)
            _controller.MoveDir.y = -StickToGroundForce;
        else
            _controller.TransitionTo<AirState>();

        StopSliding();
    }

    private void UpdateMovement()
    {
        Vector3 input = _controller.Input;
        input = transform.forward * input.z + transform.right * input.x;

        /*if (hit.collider != null && hit.collider.CompareTag("Platform"))
            transform.parent = hit.collider.gameObject.transform.parent;
        else
            transform.parent = null;*/

        input = Vector3.ProjectOnPlane(input, hit.normal).normalized;

        MoveDir += input * Acceleration;

        if(input.magnitude > 0)
        {
            InputGotten = true;

            if (MoveDir.magnitude > _controller.MaxSpeed)
                MoveDir = MoveDir.normalized * _controller.MaxSpeed;
        }
    }

    private void UpdateFriction()
    {
        float extraFriction = InputGotten ? _controller.Input.magnitude < _controller.InputRequiredToMove ? ExtraFriction : 0.0f : ExtraSecondaryFriction;
        float friction = Mathf.Clamp01((Friction + extraFriction) * Time.fixedDeltaTime);
        MoveDir -= MoveDir * friction;
    }

    private void StopSliding()
    {
        if (MoveDir.magnitude < StopSlidingLimit && _controller.Input.magnitude < _controller.InputRequiredToMove)
            MoveDir = Vector3.zero;
    }

    public void UpdateJump()
    {
        PlayJumpSound();
        transform.position += Vector3.up * InitialJumpDistance;
        _controller.MoveDir.y = JumpVelocity.Max;
        _controller.GetState<AirState>().GhostJumpAllowed = false;
        _controller.GetState<AirState>().CanCancelJump = true;
        if(!(_controller.CurrentState is AirState))_controller.TransitionTo<AirState>();
    }

    private void PlayJumpSound()
    {
        if (JumpSounds.Length > 0)
        {
            int index = Random.Range(1, JumpSounds.Length);
            AudioClip clip = JumpSounds[index];
            _controller._audioSource.PlayOneShot(clip);
            JumpSounds[index] = JumpSounds[0];
            JumpSounds[0] = clip;
        }
    }
}
