using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class Sanity : MonoBehaviour {

    [Header("Player")]
    [SerializeField] private PlayerController _player;

    [Header("Sanity")]
    [SerializeField] private MinMaxFloat _sanityRange;
    [SerializeField] private float _depletionRate = 0.01f;
    [SerializeField] private float _refillRate = 0.01f;
    private float _sanity = 1f;
    private bool _tookDamage = false;

    [Header("Vignette")]
    [SerializeField] private PostProcessVolume _ppVolume;
    private Vignette _vignette;

    [Header("Pulsing Vignette")]
    [SerializeField] private Image _vignetteImage;
    [SerializeField] private MinMaxFloat _vignetteImageOpacityRange;
    [SerializeField] private float _pulseUpSpeed;
    [SerializeField] private float _pulseDownSpeed;
    [SerializeField] private float _pulseUpWaitForSeconds;
    [SerializeField] private float _pulseDownWaitForSeconds;

    public void Start()
    {
        if (!_vignetteImage)
            _vignetteImage = GameObject.FindWithTag("PulseVignette").GetComponent<Image>();

        _player = GetComponent<PlayerController>();
        _sanity = _sanityRange.Max;

        _vignette = ScriptableObject.CreateInstance<Vignette>();
        _vignette.enabled.Override(true);
        _vignette.intensity.Override(0f);
        _vignette.smoothness.Override(1f);

        _ppVolume = PostProcessManager.instance.QuickVolume(LayerMask.NameToLayer("PostProcessing"), 100f, _vignette);
        StartCoroutine(PulseSanity());
    }

    public float GetSanity()
    {
        return _sanity;
    }

    public float GetSanity01()
    {
        return Mathf.Lerp(1f, 0f, (_sanity - _sanityRange.Min) / (_sanityRange.Max - _sanityRange.Min));
    }

    public float GetSanity10()
    {
        return Mathf.Lerp(0f, 1f, (_sanity - _sanityRange.Min) / (_sanityRange.Max - _sanityRange.Min));
    }

    public void DepleteSanity(float multiplier = 1f)
    {
        if (_sanity > _sanityRange.Min)
        {
            _sanity -= _depletionRate * multiplier * Time.deltaTime;
            _tookDamage = true;
        }
        if (_sanity < _sanityRange.Min)
            _sanity = _sanityRange.Min;
        if (_sanity == _sanityRange.Min)
            if(!(_player.currentState is DeadState) && !(_player.currentState is MutualDestruction) && !(_player.currentState is MutualEscape))
                _player.TransitionTo<DeadState>();

        UpdateVignette();
    }

    public void RefillSanity(float multiplier = 1f)
    {
        if(_sanity < _sanityRange.Max)
            _sanity += _refillRate * multiplier * Time.deltaTime;
        if (_sanity > _sanityRange.Max)
            _sanity = _sanityRange.Max;

        UpdateVignette();
    }

    public void ResetSanity()
    {
        _sanity = _sanityRange.Max;

        UpdateVignette();
    }

    private void UpdateVignette()
    {
        _vignette.intensity.value = GetSanity01();
    }

    private IEnumerator PulseSanity()
    {
        bool up = false;
        float tmpMin = 0f;
        Color tmp = _vignetteImage.color;
        while (true)
        {
            tmpMin = Mathf.Lerp(_vignetteImageOpacityRange.Max, _vignetteImageOpacityRange.Min, GetSanity10());

            if (_tookDamage)
            {
                if (up)
                {
                    tmp.a = Mathf.Clamp(tmp.a + _pulseUpSpeed * Time.deltaTime, _vignetteImageOpacityRange.Min, _vignetteImageOpacityRange.Max);
                    if (tmp.a >= _vignetteImageOpacityRange.Max)
                    {
                        up = false;
                        yield return new WaitForSeconds(_pulseDownWaitForSeconds);
                    }
                }
                else
                {
                    tmp.a = Mathf.Clamp(tmp.a - _pulseDownSpeed * Time.deltaTime, tmpMin, _vignetteImageOpacityRange.Max);
                    if (tmp.a <= tmpMin)
                    {
                        up = true;
                        yield return new WaitForSeconds(_pulseUpWaitForSeconds);
                    }
                }
            }
            else
                tmp.a = Mathf.Clamp(tmp.a - _pulseDownSpeed * Time.deltaTime, _vignetteImageOpacityRange.Min, _vignetteImageOpacityRange.Max);

            _vignetteImage.color = tmp;
            _tookDamage = false;

            yield return null;
        }
    }

    void Destroy()
    {
        RuntimeUtilities.DestroyVolume(_ppVolume, true);
    }
}
