using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapePodEngine : MonoBehaviour {

    [Header("Engine")]
    [SerializeField] private float _speed;

    [Header("Image")]
    [SerializeField] private float _fadeSpeed;
    [SerializeField] private Sprite _sprite;
    private Canvas _canvas;
    private RectTransform _imageTransform;
    private Image _image;

    [Header("Pod")]
    [SerializeField] private Transform _pod;
    private bool _activated = false;

    void Start()
    {
        _canvas = GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>();
        _imageTransform = new GameObject().AddComponent<RectTransform>();
        _imageTransform.transform.SetParent(_canvas.transform);
        _imageTransform.sizeDelta = new Vector2(10000, 10000);
        _imageTransform.localPosition = Vector3.zero;
        _image = _imageTransform.gameObject.AddComponent<Image>();
        _image.sprite = _sprite;
        _image.color = new Color(1f, 1f, 1f, 0f);
    }

	void OnTriggerEnter(Collider coll)
    {
        if(!_activated && coll.tag == "Player")
        {
            StartCoroutine(Shake());
            StartCoroutine(FadeOut());
            StartCoroutine(Move());
            _activated = true;
        }
    }

    private IEnumerator Move()
    {
        while (true)
        {
            _pod.RotateAround(_pod.position, _pod.up, Time.deltaTime * 5f);
            _pod.position += _pod.forward * _speed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        while(_image.color.a < 1f)
        {
            _image.color = new Color(1f, 1f, 1f, _image.color.a + _fadeSpeed * Time.deltaTime);
            yield return null;
        }

        StopAllCoroutines();
    }

    private IEnumerator Shake()
    {
        while (true)
        {
            CameraShake.AddIntensity(1f);
            yield return new WaitForSeconds(10f);
        }
    }
}
