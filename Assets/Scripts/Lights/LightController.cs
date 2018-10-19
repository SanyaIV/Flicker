using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightController : MonoBehaviour {

    [Header("Lamp")]
    [SerializeField] private GameObject _lampBulb;
    private Renderer _lampBulbRenderer;
    private Material _lampBulbMaterial;
    private Color _baseLampBulbEmissionColor;

    [Header("Light")]
    [SerializeField] private Light _light;
    [SerializeField] private MinMaxFloat _intensity;

    [Header("Fade In/Out")]
    [SerializeField] private float _fadeInTime;
    [SerializeField] private float _fadeOutTime;

    [Header("Flicker")]
    [SerializeField] private MinMaxFloat _flickerFadeSpeed;
    [Tooltip("Used to wait while the light is off and going to be turned on")]
    [SerializeField] private MinMaxFloat _flickerOnWait;
    [Tooltip("Used to wait while the light is on and going to be turned off")]
    [SerializeField] private MinMaxFloat _flickerOffWait;
    private Coroutine _flickerCoroutine;

    void Start () {
        _light = GetComponent<Light>();

        if (_lampBulb)
        {
            _lampBulbRenderer = _lampBulb.GetComponent<Renderer>();
            _lampBulbMaterial = _lampBulbRenderer.material;
            _baseLampBulbEmissionColor = _light.color;
        }

        SetLampEmission();
	}

    public void On()
    {
        _light.intensity = _intensity.Max;
        SetLampEmission();
    }

    public void Off()
    {
        _light.intensity = 0f;
        SetLampEmission();
    }
	
    public void Max()
    {
        _light.intensity = _intensity.Max;
        SetLampEmission();
    }

    public void FadeMax()
    {
        StartCoroutine(FadeMax(_fadeInTime));
    }

    public void Min()
    {
        _light.intensity = _intensity.Min;
        SetLampEmission();
    }

    public void FadeMin()
    {
        StartCoroutine(FadeMin(_fadeOutTime));
    }

    public void FadeOff()
    {
        StartCoroutine(FadeOff(_fadeOutTime));
    }

    public void ToggleMinMax()
    {
        if (_intensity.Max - _light.intensity <= (_intensity.Max - _intensity.Min) / 2)
            _light.intensity = _intensity.Min;
        else
            _light.intensity = _intensity.Max;

        SetLampEmission();
    }

    public void ToggleOnOff()
    {
        if (_light.intensity > 0f)
            _light.intensity = _intensity.Min;
        else
            _light.intensity = _intensity.Max;

        SetLampEmission();
    }

    public void ToggleMinMaxFade()
    {
        StopAllCoroutines();

        if (_intensity.Max - _light.intensity <= (_intensity.Max - _intensity.Min) / 2)
            FadeMin();
        else
            FadeMax();
    }

    public void ToggleOnOffFade()
    {
        StopAllCoroutines();

        if (_light.intensity > 0f)
            FadeOff();
        else
            FadeMax();
    }

    public void StartFlicker()
    {
        StopAllCoroutines();
        _flickerCoroutine = StartCoroutine(Flicker());
    }

    public void StartFlickerMinMax()
    {
        StopAllCoroutines();
        _flickerCoroutine = StartCoroutine(FlickerMinMax());
    }

    public void StartFlickerForSecondsLeaveOn(float timeInSeconds)
    {
        StopAllCoroutines();
        StartCoroutine(FlickerForSecondsLeaveOn(timeInSeconds));
    }

    public void StartFlickerMinMaxForSecondsLeaveMax(float timeInSeconds)
    {
        StopAllCoroutines();
        StartCoroutine(FlickerMinMaxForSecondsLeaveMax(timeInSeconds));
    }

    public void StartFlickerForSecondsLeaveOff(float timeInSeconds)
    {
        StopAllCoroutines();
        StartCoroutine(FlickerForSecondsLeaveOff(timeInSeconds));
    }

    public void StartFlickerMinMaxForSecondsLeaveMin(float timeInSeconds)
    {
        StopAllCoroutines();
        StartCoroutine(FlickerMinMaxForSecondsLeaveMin(timeInSeconds));
    }

    public void StopFlicker(bool leaveOn = false)
    {
        if(_flickerCoroutine != null)
            StopCoroutine(_flickerCoroutine);
    }

    private void SetLampEmission()
    {
        if (_lampBulbMaterial)
        {
            Color finalColor = _baseLampBulbEmissionColor * Mathf.LinearToGammaSpace(Mathf.Lerp(0f, 1f, _light.intensity / _intensity.Max));
            _lampBulbMaterial.SetColor("_EmissionColor", finalColor * _light.intensity * _light.range / 10);
        }
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            if (_light.intensity < _intensity.Max / 2)
            {
                yield return new WaitForSeconds(Random.Range(_flickerOnWait.Min, _flickerOnWait.Max));
                StartCoroutine(FadeMax(Random.Range(_flickerFadeSpeed.Min, _flickerFadeSpeed.Max)));
            }  
            else
            {
                yield return new WaitForSeconds(Random.Range(_flickerOffWait.Min, _flickerOffWait.Max));
                StartCoroutine(FadeOff(Random.Range(_flickerFadeSpeed.Min, _flickerFadeSpeed.Max)));
            }
            
            yield return null;
        }
    }

    private IEnumerator FlickerMinMax()
    {
        while (true)
        {
            if (_light.intensity < (_intensity.Max - _intensity.Min) / 2)
            {
                yield return new WaitForSeconds(Random.Range(_flickerOnWait.Min, _flickerOnWait.Max));
                StartCoroutine(FadeMax(Random.Range(_flickerFadeSpeed.Min, _flickerFadeSpeed.Max)));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(_flickerOffWait.Min, _flickerOffWait.Max));
                StartCoroutine(FadeMin(Random.Range(_flickerFadeSpeed.Min, _flickerFadeSpeed.Max)));
            }

            yield return null;
        }
    }

    private IEnumerator FlickerForSecondsLeaveOn(float seconds)
    {
        _flickerCoroutine = StartCoroutine(Flicker());
        yield return new WaitForSeconds(seconds);
        StopCoroutine(_flickerCoroutine);
        _light.intensity = _intensity.Max;
        SetLampEmission();
    }

    private IEnumerator FlickerMinMaxForSecondsLeaveMax(float seconds)
    {
        _flickerCoroutine = StartCoroutine(FlickerMinMax());
        yield return new WaitForSeconds(seconds);
        StopCoroutine(_flickerCoroutine);
        _light.intensity = _intensity.Max;
        SetLampEmission();
    }

    private IEnumerator FlickerForSecondsLeaveOff(float seconds)
    {
        _flickerCoroutine = StartCoroutine(Flicker());
        yield return new WaitForSeconds(seconds);
        StopCoroutine(_flickerCoroutine);
        _light.intensity = 0f;
        SetLampEmission();
    }

    private IEnumerator FlickerMinMaxForSecondsLeaveMin(float seconds)
    {
        _flickerCoroutine = StartCoroutine(FlickerMinMax());
        yield return new WaitForSeconds(seconds);
        StopCoroutine(_flickerCoroutine);
        _light.intensity = _intensity.Min;
        SetLampEmission();
    }

    private IEnumerator FadeMax(float speed)
    {
        float start = _light.intensity;
        float time = 0f;

        while (_light.intensity < _intensity.Max)
        {
            _light.intensity = Mathf.Lerp(start, _intensity.Max, time += Time.deltaTime / speed);
            SetLampEmission();
            yield return null;
        }

        _light.intensity = _intensity.Max;
        SetLampEmission();
        yield break;
    }

    private IEnumerator FadeMin(float speed)
    {
        float start = _light.intensity;
        float time = 0f;

        while (_light.intensity > _intensity.Min)
        {
            _light.intensity = Mathf.Lerp(start, _intensity.Min, time += Time.deltaTime / speed);
            SetLampEmission();
            yield return null;
        }

        _light.intensity = _intensity.Min;
        SetLampEmission();
        yield break;
    }

    private IEnumerator FadeOff(float speed)
    {
        float start = _light.intensity;
        float time = 0f;

        while (_light.intensity > 0f)
        {
            _light.intensity = Mathf.Lerp(start, 0f, time += Time.deltaTime / speed);
            SetLampEmission();
            yield return null;
        }

        _light.intensity = 0f;
        SetLampEmission();
        yield break;
    }

    void OnValidate()
    {
        if (!_light)
            _light = GetComponent<Light>();

        if (_intensity.Min > _intensity.Max)
            Debug.LogWarning("Configured Max intensity: [" + _intensity.Max + "] for Light: [" + _light + "] is less than Min intensity: [" + _intensity.Min + "]", gameObject);
        if (_intensity.Max == 0)
            Debug.LogWarning("Configured Max intensity for Light: [" + _light + "] is: [0]", gameObject);
        if (_intensity.Max < _light.intensity)
            Debug.LogWarning("Current intensity: [" + _light.intensity + "] of light: [" + _light + "] is higher than the configured Max intensity: [" + _intensity.Max + "]", gameObject);
    }

}
