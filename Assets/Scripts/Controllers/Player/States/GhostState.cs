using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/States/Debug/Ghost")]
public class GhostState : PlayerState
{
    public float walkSpeed = 4f;
    public float joggSpeed = 8f;
    public float runSpeed = 12f;
    public float acceleration = 100f;
    public float ExtraFriction = 30f;
    public float extraSecondaryFriction = 30f;
    public float stopSlidingLimit = 1.5f;
    [HideInInspector] public bool inputGotten = false;

    private float friction { get { return acceleration / controller.maxSpeed; } }

    public override void Update()
    {
        base.Update();
        
        if (Input.GetKey(KeyCode.LeftShift) && !(controller.Input.z < 0))
            controller.maxSpeed = runSpeed;
        else if (Input.GetKey(KeyCode.LeftControl))
            controller.maxSpeed = walkSpeed;
        else
            controller.maxSpeed = joggSpeed;

        transform.Translate(moveDir * Time.deltaTime, Space.Self);

        if (Input.GetKeyDown(KeyCode.G))
            controller.TransitionTo<AirState>();
    }

    public override void FixedUpdate()
    {
        UpdateMovement();
        UpdateFriction();
        StopSliding();
    }

    private void UpdateMovement()
    {
        Vector3 input = controller.Input;
        input = Vector3.forward * input.z + Vector3.right * input.x;

        moveDir += input * acceleration;

        if (input.magnitude > 0)
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
}
