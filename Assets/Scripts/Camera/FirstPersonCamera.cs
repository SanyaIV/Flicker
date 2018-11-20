using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private const int INVERSE = -1;

    [Header("Follow")]
    public Transform target;

    [Header("Rotation")]
    public MinMaxFloat yAngleClamp;
    public bool yClamp;
    public Vector2 speed;
    public float requiredInputMagnitude = 0.3f;
    private Vector2 _V2Rotation;

    private void Start()
    {
        if (target == null)
            target = GameObject.FindWithTag("Player").transform;

        InitializeRotation();
    }

    private void InitializeRotation()
    {
        _V2Rotation.x = target.rotation.eulerAngles.y;
        _V2Rotation.y = transform.rotation.eulerAngles.x;
    }

    private void Update()
    {
        if (GameManager.paused)
            return;

        UpdateRotation();
        UpdateTargetRotation();
    }

    private void LateUpdate()
    {
        UpdateOwnRotation();
    }

    private void UpdateRotation()
    {
        Vector2 input = new Vector2(Input.GetAxis("CameraX"), Input.GetAxis("CameraY"));
        input.x = Mathf.Abs(input.x) > requiredInputMagnitude ? input.x : 0.0f;
        input.y = Mathf.Abs(input.y) > requiredInputMagnitude ? input.y : 0.0f;
        input += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * INVERSE);

        _V2Rotation.x += input.x * speed.x;
        _V2Rotation.y += input.y * speed.y;

        if (yClamp) _V2Rotation.y = Mathf.Clamp(_V2Rotation.y, yAngleClamp.Min, yAngleClamp.Max);
    }

    private void UpdateTargetRotation()
    {
        target.rotation = Quaternion.Euler(0.0f, _V2Rotation.x, 0.0f);
    }

    private void UpdateOwnRotation()
    {
        transform.localRotation = Quaternion.Euler(_V2Rotation.y, 0f, 0f);
    }
}
