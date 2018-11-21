using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAudio : OffScreenIndicator {

    [Header("Audio")]
    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private bool _noInterrupt;
    private bool _isPaused;

    [Header("Random")]
    [SerializeField] private bool _continiousRandom;
    [SerializeField] private MinMaxFloat _randomWaitRange;

    public override void Start()
    {
        base.Start();

        if (_continiousRandom)
            StartCoroutine(ContiniousRandomPlay());
        if(_enableIndicator)
            StartCoroutine(OffScreenIndicator());
    }

    public void Pause()
    {
        if (_audioSource.isPlaying)
            _audioSource.Pause();

        _isPaused = true;
    }

    public void Stop()
    {
        _audioSource.Stop();
    }

    public void Resume()
    {
        if (_isPaused)
        {
            _audioSource.UnPause();
            _isPaused = false;
        }
    }

    public void PlayAudio()
    {
        if (_noInterrupt && _audioSource.isPlaying || _isPaused)
            return;

        if (_audioClips.Length > 1)
        {
            int n = Random.Range(1, _audioClips.Length);
            _audioSource.clip = _audioClips[n];
            _audioSource.Play();
            _audioClips[n] = _audioClips[0];
            _audioClips[0] = _audioSource.clip;
        }
        else if (_audioClips.Length == 1)
        {
            _audioSource.clip = _audioClips[0];
            _audioSource.Play();
        }
        else
        {
            Debug.LogWarning("There are no Audio Clips for BasicAudio script on: " + gameObject);
            return;
        }
            
    }

    public void PlayOneShot(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    private IEnumerator OffScreenIndicator() {
        while (true)
        {
            if (_audioSource.isPlaying && !_isPaused && !_enabled)
                EnableIndicator(_audioSource.GetComponentInParent<Transform>());
            else if (_audioSource.isPlaying && !_isPaused && _enabled)
                yield return null;
            else
                DisableIndicator();

            yield return null;
        }
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
