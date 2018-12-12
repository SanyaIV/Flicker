using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardLight : MonoBehaviour
{
    [Header("Hazard Light")]
    [SerializeField] private bool _enableAtStart = false;

    [Header("Lights")]
    [SerializeField] private LightController[] _lights;

    [Header("Rotation Script")]
    [SerializeField] private Rotate _rotateScript;

    [Header("Constants")]
    const float TIME_TO_WAIT_FOR_UNITY_TO_STOP_MESSING_WITH_MY_THINGS = 1f;

    public void Start()
    {
        StartCoroutine(UnityBSWorkaround());
    }

    public void Enable()
    {
        foreach (LightController light in _lights)
            light.Max();
        _rotateScript.Enable();
    }

    public void Disable()
    {
        foreach (LightController light in _lights)
            light.Off();
        _rotateScript.Disable();
    }

    private IEnumerator UnityBSWorkaround()
    {
        yield return new WaitForSeconds(TIME_TO_WAIT_FOR_UNITY_TO_STOP_MESSING_WITH_MY_THINGS);
        if (!_enableAtStart)
            Disable();
        else
            Enable();

        yield break;
    }
}
