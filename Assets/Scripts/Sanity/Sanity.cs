using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Sanity : MonoBehaviour {

    [Header("Sanity")]
    [SerializeField] private MinMaxFloat _sanityRange;
    [SerializeField] private float _depletionRate = 0.01f;
    [SerializeField] private float _refillRate = 0.01f;
    private float _sanity = 1f;

    [Header("Vignette")]
    [SerializeField] private PostProcessVolume _ppVolume;
    private Vignette _vignette;

    public void Start()
    {
        _sanity = _sanityRange.Max;

        _vignette = ScriptableObject.CreateInstance<Vignette>();
        _vignette.enabled.Override(true);
        _vignette.intensity.Override(0f);
        _vignette.smoothness.Override(1f);

        _ppVolume = PostProcessManager.instance.QuickVolume(LayerMask.NameToLayer("PostProcessing"), 100f, _vignette);
    }

    public float GetSanity()
    {
        return _sanity;
    }

    public void DepleteSanity(float multiplier = 1f)
    {
        if (_sanity > _sanityRange.Min)
            _sanity -= _depletionRate * multiplier * Time.deltaTime;
        if (_sanity < _sanityRange.Min)
            _sanity = _sanityRange.Min;
        /*if (_sanity == _sanityRange.Min)
            Death();*/

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
        _vignette.intensity.value = Mathf.Lerp(1f, 0f, _sanity / (_sanityRange.Max - _sanityRange.Min));
    }

    void Destroy()
    {
        RuntimeUtilities.DestroyVolume(_ppVolume, true);
    }
}
