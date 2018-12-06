using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChristmasBS : MonoBehaviour
{
    [Header("ChristmasBS")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _minPitch;
    [SerializeField] private float _pitchDecreaseSpeed;
    private int _christmasBSCounter = 0;
    private LightController[] _lights;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            StartChristmasBS();
    }

    private void StartChristmasBS()
    {
        if(_christmasBSCounter == 0) {
            _lights = GameObject.Find("Ship").GetComponentsInChildren<LightController>();
            foreach(LightController light in _lights)
                light.StartChristmasBS();
        }
        if(_christmasBSCounter == 1)
        {
            foreach (LightController light in _lights)
                light.StartCancerMode();
            _audioSource.Play();
            StartCoroutine(ShiftAudioPitch());
        }

        _christmasBSCounter++;
    }

    private IEnumerator ShiftAudioPitch()
    {
        while (_audioSource.pitch > _minPitch)
        {
            _audioSource.pitch -= _pitchDecreaseSpeed * Time.deltaTime;
            yield return null;
        }

        yield return null;
    }
}
