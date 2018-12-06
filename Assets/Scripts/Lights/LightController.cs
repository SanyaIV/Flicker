using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightController : MonoBehaviour {

    [Header("Switch")]
    [SerializeField] private bool _locked;

    [Header("Lamp")]
    [SerializeField] private GameObject _lampBulb;
    [SerializeField] private float _intensityMultiplier;
    private Renderer _lampBulbRenderer;
    private Material _lampBulbMaterial;
    private Color _baseLampBulbEmissionColor;

    [Header("Light")]
    [SerializeField] protected Light _light;
    [SerializeField] protected MinMaxFloat _intensity;

    [Header("Fade In/Out")]
    [SerializeField] private float _fadeInTime;
    [SerializeField] private float _fadeOutTime;
    protected Coroutine _fadeMax = null;
    protected Coroutine _fadeMin = null;

    [Header("Flicker")]
    [SerializeField] private bool _flickerOnStart;
    [SerializeField] protected MinMaxFloat _flickerOnFadeSpeed = new MinMaxFloat(0.05f, 0.2f);
    [SerializeField] protected MinMaxFloat _flickerOffFadeSpeed = new MinMaxFloat(0.05f, 0.2f);
    [Tooltip("Used to wait while the light is off and going to be turned on")]
    [SerializeField] protected MinMaxFloat _flickerOnWait = new MinMaxFloat(0.05f, 0.7f);
    [Tooltip("Used to wait while the light is on and going to be turned off")]
    [SerializeField] protected MinMaxFloat _flickerOffWait = new MinMaxFloat(0.1f, 1f);
    private Coroutine _flickerCoroutine;

    [Header("Save")]
    private float _savedIntensity;

    public virtual void Start () {
        _light = GetComponent<Light>();

        if (_intensity.Max == 0f)
            _intensity.Max = _light.intensity;

        if (_lampBulb)
        {
            _lampBulbRenderer = _lampBulb.GetComponent<Renderer>();
            _lampBulbMaterial = _lampBulbRenderer.material;
            _baseLampBulbEmissionColor = _light.color;
        }

        SetLampEmission();

        if (_flickerOnStart)
            StartFlicker();

        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(ReloadSave);
    }

    public void Lock()
    {
        _locked = true;
    }

    public void Unlock()
    {
        _locked = false;
    }

    public void On()
    {
        if (_locked)
            return;

        StopAllCoroutines();
        _light.intensity = _intensity.Max;
        SetLampEmission();
    }

    public void Off()
    {
        if (_locked)
            return;

        StopAllCoroutines();
        _light.intensity = 0f;
        SetLampEmission();
    }
	
    public void Max()
    {
        if (_locked)
            return;

        StopAllCoroutines();
        _light.intensity = _intensity.Max;
        SetLampEmission();
    }

    public void FadeMax()
    {
        if (_locked)
            return;

        StopAllCoroutines();
        StartCoroutine(FadeMax(_fadeInTime));
    }

    public void Min()
    {
        if (_locked)
            return;

        StopAllCoroutines();
        _light.intensity = _intensity.Min;
        SetLampEmission();
    }

    public void FadeMin()
    {
        if (_locked)
            return;

        StopAllCoroutines();
        StartCoroutine(FadeMin(_fadeOutTime));
    }

    public void FadeOff()
    {
        if (_locked)
            return;

        StopAllCoroutines();
        StartCoroutine(FadeOff(_fadeOutTime));
    }

    public void StartFlicker()
    {
        if (_locked)
            return;

        StopAllCoroutines();
        _flickerCoroutine = StartCoroutine(Flicker());
    }

    public void StartChristmasBS()
    {
        StartCoroutine(ChristmasBS());
    }

    public void StartCancerMode()
    {
        StartCoroutine(CancerMode());
    }

    public void StartFlickerMinMax()
    {
        if (_locked)
            return;

        StopAllCoroutines();
        _flickerCoroutine = StartCoroutine(FlickerMinMax());
    }

    public void StartFlickerForSecondsLeaveOn(float timeInSeconds)
    {
        if (_locked)
            return;

        StopAllCoroutines();
        StartCoroutine(FlickerForSecondsLeaveOn(timeInSeconds));
    }

    public void StartFlickerMinMaxForSecondsLeaveMax(float timeInSeconds)
    {
        if (_locked)
            return;

        StopAllCoroutines();
        StartCoroutine(FlickerMinMaxForSecondsLeaveMax(timeInSeconds));
    }

    public void StartFlickerForSecondsLeaveOff(float timeInSeconds)
    {
        if (_locked)
            return;

        StopAllCoroutines();
        StartCoroutine(FlickerForSecondsLeaveOff(timeInSeconds));
    }

    public void StartFlickerMinMaxForSecondsLeaveMin(float timeInSeconds)
    {
        if (_locked)
            return;

        StopAllCoroutines();
        StartCoroutine(FlickerMinMaxForSecondsLeaveMin(timeInSeconds));
    }

    public void StopFlicker()
    {
        if (_locked)
            return;

        if (_flickerCoroutine != null)
            StopCoroutine(_flickerCoroutine);
    }

    public void StopMinMax()
    {
        if (_fadeMin != null)
            StopCoroutine(_fadeMin);
        if (_fadeMax != null)
            StopCoroutine(_fadeMax);
    }

    private void SetLampEmission()
    {
        if (_lampBulbMaterial)
        {
            Color finalColor = _baseLampBulbEmissionColor * Mathf.LinearToGammaSpace(Mathf.Lerp(0f, 1f, _light.intensity / _intensity.Max) * _intensityMultiplier);
            _lampBulbMaterial.SetColor("_EmissiveColor", finalColor * _light.intensity * _intensityMultiplier / 10);
        }
    }

    public void EnemyFlickerOn()
    {
        if (_flickerOnStart)
            return;

        StartFlicker();
    }

    public void EnemyFlickerOff()
    {
        if (_flickerOnStart)
            return;

        FadeMax();
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            if (_light.intensity < _intensity.Max / 2)
            {
                StopMinMax();

                _fadeMax = StartCoroutine(FadeMax(Random.Range(_flickerOnFadeSpeed.Min, _flickerOnFadeSpeed.Max)));
                yield return new WaitForSeconds(Random.Range(_flickerOnWait.Min, _flickerOnWait.Max));
            }  
            else
            {
                StopMinMax();

                _fadeMin = StartCoroutine(FadeOff(Random.Range(_flickerOffFadeSpeed.Min, _flickerOffFadeSpeed.Max)));
                yield return new WaitForSeconds(Random.Range(_flickerOffWait.Min, _flickerOffWait.Max));
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
                StopMinMax();

                _fadeMax = StartCoroutine(FadeMax(Random.Range(_flickerOnFadeSpeed.Min, _flickerOnFadeSpeed.Max)));
                yield return new WaitForSeconds(Random.Range(_flickerOnWait.Min, _flickerOnWait.Max));
            }
            else
            {
                StopMinMax();

                _fadeMin = StartCoroutine(FadeMin(Random.Range(_flickerOffFadeSpeed.Min, _flickerOffFadeSpeed.Max)));
                yield return new WaitForSeconds(Random.Range(_flickerOffWait.Min, _flickerOffWait.Max));
            }

            yield return null;
        }
    }

    private IEnumerator FlickerForSecondsLeaveOn(float seconds)
    {
        _flickerCoroutine = StartCoroutine(Flicker());
        yield return new WaitForSeconds(seconds);

        StopCoroutine(_flickerCoroutine);
        StopMinMax();

        _light.intensity = _intensity.Max;
        SetLampEmission();
    }

    private IEnumerator FlickerMinMaxForSecondsLeaveMax(float seconds)
    {
        _flickerCoroutine = StartCoroutine(FlickerMinMax());
        yield return new WaitForSeconds(seconds);

        StopCoroutine(_flickerCoroutine);
        StopMinMax();

        _light.intensity = _intensity.Max;
        SetLampEmission();
    }

    private IEnumerator FlickerForSecondsLeaveOff(float seconds)
    {
        _flickerCoroutine = StartCoroutine(Flicker());
        yield return new WaitForSeconds(seconds);

        StopCoroutine(_flickerCoroutine);
        StopMinMax();

        _light.intensity = 0f;
        SetLampEmission();
    }

    private IEnumerator FlickerMinMaxForSecondsLeaveMin(float seconds)
    {
        _flickerCoroutine = StartCoroutine(FlickerMinMax());
        yield return new WaitForSeconds(seconds);

        StopCoroutine(_flickerCoroutine);
        StopMinMax();

        _light.intensity = _intensity.Min;
        SetLampEmission();
    }

    protected IEnumerator FadeMax(float speed)
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

    protected IEnumerator FadeMin(float speed)
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

    protected IEnumerator FadeOff(float speed)
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

    private IEnumerator CancerMode()
    {
        while (true)
        {
            if (_light.intensity != _intensity.Max)
                _light.intensity = _intensity.Max;
            else
                _light.intensity = _intensity.Min;

            SetLampEmission();
            yield return null;
        }
    }

    private IEnumerator ChristmasBS()
    {
        _light.color = Color.green;

        while(true){
            if (_light.color == Color.green)
                _light.color = Color.red;
            else if (_light.color == Color.red)
                _light.color = Color.green;

            _baseLampBulbEmissionColor = _light.color;
            SetLampEmission();

            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    void OnValidate()
    {
        if (!_light)
            _light = GetComponent<Light>();

        if (_intensity.Max == 0 || _intensity.Max < _light.intensity)
            _intensity.Max = _light.intensity;

        if (_intensity.Min > _intensity.Max)
            _intensity.Min = _intensity.Max;
    }

    public void Save()
    {
        _savedIntensity = _light.intensity;
    }

    public void ReloadSave()
    {
        _light.intensity = _savedIntensity;
    }
}
