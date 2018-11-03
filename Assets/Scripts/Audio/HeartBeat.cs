using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBeat : MonoBehaviour {

    public const int SECONDS_PER_MINUTE = 60;

    [Header("Heart beat")]
    [SerializeField] private MinMaxFloat _rangeOfBPM;
    [SerializeField] private MinMaxFloat _rangeOfPitch;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    void Start()
    {
        if (!_audioSource)
            _audioSource = GetComponent<AudioSource>();
        if (_audioClip)
            _audioSource.clip = _audioClip;

        //Warn if settings results in a clip that will want to play more often than the clip length allows for
        MinMaxFloat clipLenght = new MinMaxFloat(CalculateClipLength(_rangeOfPitch.Min), CalculateClipLength(_rangeOfPitch.Max));
        MinMaxFloat clipDelay = new MinMaxFloat(BPMToDelay(_rangeOfBPM.Min), BPMToDelay(_rangeOfBPM.Max));
        if (clipDelay.Min < clipLenght.Min)
            Debug.LogWarning("Shortest clip length is longer than the shortest delay! Shortest Clip Length: " + clipLenght.Min + " Shortest Delay: " + clipDelay.Min);
        if (clipDelay.Max < clipLenght.Max)
            Debug.LogWarning("Longest clip length is longer than the longest delay! Longest Clip Length: " + clipLenght.Min + " Longest Delay: " + clipDelay.Min);
    }
	
	// Update is called once per frame
	void Update () {
        if (!_audioSource.isPlaying)
        {
            float percent = (CalculateBPM() - _rangeOfBPM.Min) / (_rangeOfBPM.Max - _rangeOfBPM.Min);

            _audioSource.PlayScheduled(AudioSettings.dspTime + BPMToDelay(CalculateBPM()));
            _audioSource.volume = percent;
            _audioSource.pitch = Mathf.Lerp(_rangeOfPitch.Min, _rangeOfPitch.Max, percent);
        }
	}

    private float BPMToDelay(float BPM)
    {
        return SECONDS_PER_MINUTE / BPM;
    }

    private float CalculateBPM()
    {

        Sanity sanity = GameObject.FindWithTag("Player").GetComponent<PlayerController>().sanity;
        return Mathf.Lerp(_rangeOfBPM.Min, _rangeOfBPM.Max, sanity.GetSanity01());
    }

    private float CalculateClipLength(float pitch)
    {
        if (pitch == 0)
            return 0;

        return _audioSource.clip.length / Mathf.Abs(pitch);
    }
}
