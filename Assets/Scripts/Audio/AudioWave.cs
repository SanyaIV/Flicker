using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioWave : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private Image _image;
    [SerializeField] private float _scaleSpeed;
    [SerializeField] private float _colorSpeed;

    void Start () {
        StartCoroutine(Wave());
	}

    private IEnumerator Wave()
    {
        Color tmp = _image.color;
        tmp.a = 0f;

        while(tmp.a < 1f)
        {
            _image.transform.localScale += Vector3.one * _scaleSpeed * Time.deltaTime;
            tmp.a += _colorSpeed * Time.deltaTime;
            _image.color = tmp;

            yield return null;
        }

        while(tmp.a > 0f)
        {
            _image.transform.localScale += Vector3.one * _scaleSpeed * Time.deltaTime;
            tmp.a -= _colorSpeed * Time.deltaTime;
            _image.color = tmp;

            yield return null;
        }

        Destroy(gameObject);
        yield break;
    }
}
