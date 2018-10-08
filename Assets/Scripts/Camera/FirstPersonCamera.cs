using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private const int INVERSE = -1;
    private Vector3 _startPos;
    private Quaternion _startRot;

    [Header("Follow")]
    public Transform Target;
    public Vector3 PositionOffset;
    public float RequiredInputMagnitude = 0.3f;

    [Header("Rotation")]
    public MinMaxFloat YAngleClamp;
    public bool YClamp;
    public Vector2 Speed;
    private Quaternion _rotation;
    private Vector2 _V2Rotation;

    private void Start()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
        _V2Rotation.x = _startRot.eulerAngles.y;

        if (Target == null)
            Target = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        UpdateRotation();
    }

    private void LateUpdate()
    {
        UpdateMovement();
        UpdateTargetRotation();
    }

    private void UpdateMovement()
    {
        transform.position = Target.position + PositionOffset;
        transform.rotation = _rotation;
    }

    private void UpdateRotation()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("CameraX"), Input.GetAxisRaw("CameraY"));
        input.x = Mathf.Abs(input.x) > RequiredInputMagnitude ? input.x : 0.0f;
        input.y = Mathf.Abs(input.y) > RequiredInputMagnitude ? input.y : 0.0f;
        input += new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y") * INVERSE);

        _V2Rotation.x += input.x * Speed.x * Time.fixedDeltaTime;
        _V2Rotation.y += input.y * Speed.y * Time.fixedDeltaTime;

        if (YClamp) _V2Rotation.y = Mathf.Clamp(_V2Rotation.y, YAngleClamp.Min, YAngleClamp.Max);
        _rotation = Quaternion.Euler(_V2Rotation.y, _V2Rotation.x, 0.0f);
    }

    private void UpdateTargetRotation()
    {
        Target.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);
    }

    public void ResetTransform()
    {
        transform.position = _startPos;
        transform.rotation = _startRot;
    }
}
