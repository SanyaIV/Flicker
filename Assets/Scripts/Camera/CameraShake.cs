using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    private static float _intensity;

    public float MaxDuration = 2f;
    public float PowerOf = 2f;

    public Vector3 Max;
    public Vector3 Scale;

	public static void AddIntensity(float intensity)
    {
        _intensity += intensity;
    }

    public static void SetIntensity(float intensity)
    {
        _intensity = intensity;
    }
	
	// Update is called once per frame
	void Update () {
        _intensity -= Time.deltaTime / MaxDuration;
        _intensity = Mathf.Clamp01(_intensity);

        float magnitude = Mathf.Pow(_intensity, PowerOf);
        float x = (0.5f - Mathf.PerlinNoise(Time.time * Scale.x, 0f)) * Max.x * magnitude;
        float y = (0.5f - Mathf.PerlinNoise(0f, Time.time * Scale.y)) * Max.y * magnitude;
        float z = (0.5f - Mathf.PerlinNoise(Time.time * Scale.z, Time.time * Scale.z)) * Max.z * magnitude;

        transform.localRotation = Quaternion.Euler(x, y, z);
    }
}
