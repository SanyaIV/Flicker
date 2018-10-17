using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private const int INVERSE = -1;
    private Vector3 _startPos;
    private Quaternion _startRot;

    [Header("Follow")]
    public bool useFollow;
    public Transform target;
    public Vector3 positionOffset;

    [Header("Rotation")]
    public MinMaxFloat yAngleClamp;
    public bool yClamp;
    public Vector2 speed;
    public float requiredInputMagnitude = 0.3f;
    private Quaternion _rotation;
    private Vector2 _V2Rotation;

    private void Start()
    {
        _startPos = transform.position;
        _startRot = transform.rotation;
        _V2Rotation.x = _startRot.eulerAngles.y;

        if (target == null)
            target = GameObject.FindWithTag("Player").transform;
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
        if (useFollow)
            transform.position = target.position + positionOffset;

        transform.rotation = _rotation;
    }

    private void UpdateRotation()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("CameraX"), Input.GetAxisRaw("CameraY"));
        input.x = Mathf.Abs(input.x) > requiredInputMagnitude ? input.x : 0.0f;
        input.y = Mathf.Abs(input.y) > requiredInputMagnitude ? input.y : 0.0f;
        input += new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y") * INVERSE);

        _V2Rotation.x += input.x * speed.x * Time.fixedDeltaTime;
        _V2Rotation.y += input.y * speed.y * Time.fixedDeltaTime;

        if (yClamp) _V2Rotation.y = Mathf.Clamp(_V2Rotation.y, yAngleClamp.Min, yAngleClamp.Max);
        _rotation = Quaternion.Euler(_V2Rotation.y, _V2Rotation.x, 0.0f);
    }

    private void UpdateTargetRotation()
    {
        target.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);
    }

    public void ResetTransform()
    {
        transform.position = _startPos;
        transform.rotation = _startRot;
    }
}
