using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sanity : MonoBehaviour {

    [Header("Sanity")]
    [SerializeField] private MinMaxFloat _sanityRange;
    [SerializeField] private float _depletionRate = 0.01f;
    [SerializeField] private float _refillRate = 0.01f;
    private float _sanity = 1f;

    public void Start()
    {
        _sanity = _sanityRange.Max;
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
    }

    public void RefillSanity(float multiplier = 1f)
    {
        if(_sanity < _sanityRange.Max)
            _sanity += _refillRate * multiplier * Time.deltaTime;
        if (_sanity > _sanityRange.Max)
            _sanity = _sanityRange.Max;
    }

    public void ResetSanity()
    {
        _sanity = _sanityRange.Max;
    }
}
