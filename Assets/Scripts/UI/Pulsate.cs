using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsate : MonoBehaviour
{
    [SerializeField] private CanvasRenderer _canvasRenderer;
    [SerializeField] private float _speed;
    [SerializeField] private Color _firstColor;
    [SerializeField] private Color _secondColor;
    private int _direction = -1;
    private float _timer = 0f;

    // Update is called once per frame
    void Update()
    {
        _timer = Mathf.Clamp01(_timer + _speed * Time.deltaTime * _direction);
        _canvasRenderer.SetColor(Color.Lerp(_firstColor, _secondColor, _timer));

        if (_timer == 0f || _timer == 1f)
            _direction *= -1;
    }

    void OnValidate()
    {
        if(_canvasRenderer == null)
            _canvasRenderer = GetComponent<CanvasRenderer>();
    }
}
