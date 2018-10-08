using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper{

    public const float FloatEpsilon = 0.001f;
    public static int Sign(float value)
    {
        return value > FloatEpsilon ? 1 : Mathf.Abs(value) < FloatEpsilon ? 0 : -1;
    }

    public static Vector3 GetNormalForce(Vector3 velocity, Vector3 normal)
    {
        Vector3 deltaVelocity = GetVectorInDirection(velocity, normal);
        float dot = Vector3.Dot(deltaVelocity.normalized, normal.normalized);
        return dot > 0.0f ? Vector3.zero : -deltaVelocity;
    }

    public static Vector3 GetVectorInDirection(Vector3 originalVector, Vector3 direction)
    {
        return direction * Vector3.Dot(direction.normalized, originalVector.normalized) * originalVector.magnitude;
    }

    public static bool CheckAllowedSlope(float tollerance, Vector3 normal)
    {
        float dot = Vector3.Dot(normal, Vector3.up);
        float result = Mathf.Abs(dot - 1f);
        return result <= tollerance;
    }

    public static float GetWallAngleDelta(Vector3 normal)
    {
        float angle = Vector3.Angle(normal, Vector3.up);
        return Mathf.Abs(angle - 90f);
    }
	
}
