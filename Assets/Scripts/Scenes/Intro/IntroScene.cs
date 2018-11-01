using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : LightController {

    [Header("Scene")]
    [SerializeField] private string _sceneToLoad;

    [Header("Text")]
    [SerializeField] private Text _text;
    [SerializeField] private string[] _name;
    private int _nameCounter;
    private int _counterCounter;
    private float _counter = 0f;
    private AsyncOperation asyncLoad;

    // Use this for initialization
    public override void Start () {
        base.Start();

        _text.text = _name[0];
        StartCoroutine(LoadScene());
	}

    void Update()
    {
        _counter += Time.deltaTime;

        Color tmp = _text.color;
        tmp.a = Mathf.Lerp(0f, 1f, _light.intensity / _intensity.Max);
        _text.color = tmp;

        if(_counter > 0.5f && tmp.a <= 0f)
        {
            _counter = 0f;
            if (_nameCounter < _name.Length)
                _text.text = _name[++_nameCounter];
            else if (_counterCounter++ >= 3)
                asyncLoad.allowSceneActivation = true;
                
        }
    }

    private IEnumerator Flicker()
    {

        while (true)
        {
            if (_light.intensity < _intensity.Max / 2)
            {
                StopMinMax();

                _fadeMax = StartCoroutine(FadeMax(Random.Range(_flickerFadeSpeed.Min, _flickerFadeSpeed.Max)));
                yield return new WaitForSeconds(Random.Range(_flickerOnWait.Min, _flickerOnWait.Max));
            }
            else
            {
                StopMinMax();

                _fadeMin = StartCoroutine(FadeOff(Random.Range(_flickerFadeSpeed.Min, _flickerFadeSpeed.Max)));
                yield return new WaitForSeconds(Random.Range(_flickerOffWait.Min, _flickerOffWait.Max));
            }

            yield return null;
        }
    }

    private IEnumerator LoadScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync(_sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            Debug.Log(asyncLoad.progress);
            yield return null;
        }
    }
}
