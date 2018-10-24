using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAudio : OffScreenIndicator {

    [Header("Audio")]
    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private bool _noInterrupt;

    [Header("Random")]
    [SerializeField] private bool _continiousRandom;
    [SerializeField] private MinMaxFloat _randomWaitRange;

    public override void Start()
    {
        base.Start();

        if (_continiousRandom)
            StartCoroutine(ContiniousRandomPlay());
    }

    public void PlayAudio()
    {
        if (_noInterrupt && _audioSource.isPlaying)
            return;

        if (_audioClips.Length > 1)
        {
            int n = Random.Range(1, _audioClips.Length);
            _audioSource.clip = _audioClips[n];
            _audioSource.Play();
            _audioClips[n] = _audioClips[0];
            _audioClips[0] = _audioSource.clip;
            StartCoroutine(OffScreenIndicator());
        }
        else if (_audioClips.Length == 1)
        {
            _audioSource.clip = _audioClips[0];
            _audioSource.Play();
            StartCoroutine(OffScreenIndicator());
        }
        else
            return;
    }

    private IEnumerator OffScreenIndicator() {
        EnableIndicator(_audioSource.GetComponentInParent<Transform>());

        while (_audioSource.isPlaying)
        {
            yield return null;
        }

        DisableIndicator();
        yield break;
    }

    private IEnumerator ContiniousRandomPlay()
    {
        while (true)
        {
            PlayAudio();
            yield return new WaitForSeconds(Random.Range(_randomWaitRange.Min, _randomWaitRange.Max));
        }
    }
}
