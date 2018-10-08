using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/States/Air")]
public class AirState : PlayerState {

    public float FastFallingModifier = 2f;
    [HideInInspector] public bool CanCancelJump = false;

    [Header("Aircontrol")]
    public float Acceleration = 50f;
    public float Friction = 5f;

    [Header("Ghost Jump")]
    public float GhostJumpTime = 0.1f;
    private float _enterTime;
    [HideInInspector] public bool GhostJumpAllowed = false;

    public override void Enter()
    {
        //_controller.Anim.SetBool("Grounded", false);
        _enterTime = Time.time;
        _controller._collision = CollisionFlags.None;
    }

    public override void Update()
    {
        base.Update();

        CancelJump();
        if (Input.GetButtonDown("Jump"))
            UpdateJump();

        if (_controller._collision == CollisionFlags.Above)
            _controller.MoveDir.y = -2f;

        _controller._collision = _controller._charCtrl.Move(MoveDir * Time.deltaTime);
    }

    public override void FixedUpdate()
    {
        if (_controller.IsGrounded())
            _controller.TransitionTo<GroundState>();

        UpdateGravity();
        UpdateMovement();
    }

    public override void Exit()
    {
        GhostJumpAllowed = true;
    }

    private void UpdateGravity()
    {
        float multiplier = MoveDir.y > 0.0f ? 1.0f : FastFallingModifier;
        MoveDir += Vector3.down * _controller.Gravity * multiplier * Time.fixedDeltaTime;
    }

    private void CancelJump()
    {
        float minJumpVelocity = _controller.GetState<GroundState>().JumpVelocity.Min;
        if (MoveDir.y < minJumpVelocity) CanCancelJump = false;
        if (!CanCancelJump || Input.GetButton("Jump")) return;
        CanCancelJump = false;
        _controller.MoveDir.y = minJumpVelocity;
    }

    private void UpdateMovement()
    {
        Vector3 input = _controller.Input;
        input = transform.forward * input.z + transform.right * input.x;

        if (input.magnitude > _controller.InputRequiredToMove)
            Accelerate(input);
        Decelerate();
    }

    private void Accelerate(Vector3 input)
    {
        Vector3 delta = input * Acceleration * Time.fixedDeltaTime;

        float y = MoveDir.y;
        _controller.MoveDir.y = 0.0f;

        if ((MoveDir + delta).magnitude < _controller.MaxSpeed || Vector3.Dot(MoveDir.normalized, input.normalized) < 0.0f)
            MoveDir += delta;
        else
            MoveDir = input.normalized * _controller.MoveDir.magnitude;

        _controller.MoveDir.y = y;
    }

    private void Decelerate()
    {
        float y = MoveDir.y;
        _controller.MoveDir.y = 0.0f;
        MoveDir -= MoveDir * Mathf.Clamp01(Friction * Time.fixedDeltaTime);
        _controller.MoveDir.y = y;
    }

    private void UpdateJump()
    {
        if (GhostJumpAllowed && Time.time - _enterTime <= GhostJumpTime && _controller.PreviousState is GroundState)
            _controller.GetState<GroundState>().UpdateJump();
        else
            _controller.GetState<GroundState>().TimeOfAirJump = Time.time;

    }
}
