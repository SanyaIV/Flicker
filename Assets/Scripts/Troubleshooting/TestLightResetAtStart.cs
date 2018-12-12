using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLightResetAtStart : MonoBehaviour
{
    public Light _light;

    public void Start()
    {
        _light = GetComponent<Light>();
        _light.intensity = 0f;
    }

    public void Update()
    {
        Debug.Log(_light.intensity);
    }
}
