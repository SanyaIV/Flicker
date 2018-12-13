﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroScene : LightController {

    [Header("Scene")]
    [SerializeField] private string _sceneToLoad;

    [Header("Text")]
    [SerializeField] private TextMesh _text;
    [SerializeField] private string[] _name;
    private int _nameCounter;
    private int _counterCounter;
    private float _counter = 0f;
    private AsyncOperation asyncLoad;
    private bool _activated = false;
    private bool _secondActivated = false;

    [Header("Fade Out")]
    [SerializeField] private Image _fadeImage;
    [SerializeField] private float _fadeSpeed;
    private bool _fading = false;

    // Use this for initialization
    public override void Start () {
        base.Start();

        _text.text = _name[0];
    }

    void Update()
    {
        Color tmp = _text.color;
        tmp.a = Mathf.Lerp(0f, 1f, _light.intensity / _intensity.Max);
        _text.color = tmp;

        if (!_activated && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            GameObject.Find("Photosensitive warning").SetActive(false);
            _activated = true;
        }

        if (!_activated)
            return;

        _counter += Time.deltaTime;

        if(_counter >= 1.5f && !_secondActivated)
        {
            _secondActivated = true;
            _counter = 0f;
            StartCoroutine(Flicker());
            StartCoroutine(LoadScene());
        }

        if (!_fading && asyncLoad != null && asyncLoad.progress >= 0.9f && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space)))
            StartCoroutine(FadeOut());
            

        if (_secondActivated)
        {
            if (_counter > 1f && tmp.a <= 0f)
            {
                _counter = 0f;
                if (_nameCounter < _name.Length - 1)
                    _text.text = _name[++_nameCounter];
                else if (!_fading && _counterCounter++ >= 0)
                    StartCoroutine(FadeOut());
            }
        }
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

    private IEnumerator LoadScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync(_sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            Debug.Log(asyncLoad.progress);
            yield return null;
        }

        yield break;
    }

    private IEnumerator FadeOut()
    {
        _fading = true;

        while(_fadeImage.color.a < 1f)
        {
            _fadeImage.color = new Color(0f, 0f, 0f, _fadeImage.color.a + _fadeSpeed * Time.deltaTime);
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
        yield break;
    }
}
