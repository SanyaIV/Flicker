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

    [Header("Audio")]
    [SerializeField] private bool _playAudio = true;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private MinMaxFloat _timeToDelayAudio = new MinMaxFloat(0f, 0.5f);

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
        if(_playAudio)
            PlayAudio();
    }

    public void Disable()
    {
        foreach (LightController light in _lights)
            light.Off();
        _rotateScript.Disable();
        _audioSource.Stop();
    }

    private void PlayAudio()
    {
        _audioSource.clip = _audioClip;
        _audioSource.loop = true;
        StartCoroutine(WaitToPlay());
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

    private IEnumerator WaitToPlay()
    {
        yield return new WaitForSeconds(Random.Range(_timeToDelayAudio.Min, _timeToDelayAudio.Max));
        _audioSource.Play();
    }
}
