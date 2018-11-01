using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/States/Air")]
public class AirState : PlayerState {

    [Header("Fast Falling")]
    public float fastFallingModifier = 2f;
    [HideInInspector] public bool canCancelJump = false;

    [Header("Aircontrol")]
    public float acceleration = 50f;
    public float friction = 5f;

    [Header("Ghost Jump")]
    public float ghostJumpTime = 0.1f;
    private float _enterTime;
    [HideInInspector] public bool ghostJumpAllowed = false;

    public override void Enter()
    {
        //_controller.Anim.SetBool("Grounded", false);
        _enterTime = Time.time;
        controller.collision = CollisionFlags.None;
    }

    public override void Update()
    {
        base.Update();

        CancelJump();
        if (Input.GetButtonDown("Jump"))
            UpdateJump();

        if (controller.collision == CollisionFlags.Above)
            controller.moveDir.y = -2f;

        controller.collision = controller.charCtrl.Move(moveDir * Time.deltaTime);

        GetInteractible();
        if (Input.GetButtonDown("Fire1"))
            Interact();
    }

    public override void FixedUpdate()
    {
        if (controller.IsGrounded())
            controller.TransitionTo<GroundState>();

        UpdateGravity();
        UpdateMovement();
    }

    public override void Exit()
    {
        ghostJumpAllowed = true;
    }

    private void UpdateGravity()
    {
        float multiplier = moveDir.y > 0.0f ? 1.0f : fastFallingModifier;
        moveDir += Vector3.down * controller.gravity * multiplier * Time.fixedDeltaTime;
    }

    private void CancelJump()
    {
        float minJumpVelocity = controller.GetState<GroundState>().jumpVelocity.Min;
        if (moveDir.y < minJumpVelocity) canCancelJump = false;
        if (!canCancelJump || Input.GetButton("Jump")) return;
        canCancelJump = false;
        controller.moveDir.y = minJumpVelocity;
    }

    private void UpdateMovement()
    {
        Vector3 input = controller.Input;
        input = transform.forward * input.z + transform.right * input.x;

        if (input.magnitude > controller.inputRequiredToMove)
            Accelerate(input);
        Decelerate();
    }

    private void Accelerate(Vector3 input)
    {
        Vector3 delta = input * acceleration * Time.fixedDeltaTime;

        float y = moveDir.y;
        controller.moveDir.y = 0.0f;

        if ((moveDir + delta).magnitude < controller.maxSpeed || Vector3.Dot(moveDir.normalized, input.normalized) < 0.0f)
            moveDir += delta;
        else
            moveDir = input.normalized * controller.moveDir.magnitude;

        controller.moveDir.y = y;
    }

    private void Decelerate()
    {
        float y = moveDir.y;
        controller.moveDir.y = 0.0f;
        moveDir -= moveDir * Mathf.Clamp01(friction * Time.fixedDeltaTime);
        controller.moveDir.y = y;
    }

    private void UpdateJump()
    {
        if (ghostJumpAllowed && Time.time - _enterTime <= ghostJumpTime && controller.previousState is GroundState)
            controller.GetState<GroundState>().UpdateJump();
        else
            controller.GetState<GroundState>().timeOfAirJump = Time.time;

    }
}
