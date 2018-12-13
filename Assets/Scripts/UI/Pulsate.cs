using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulsate : MonoBehaviour
{
    [SerializeField] private CanvasRenderer _canvasRenderer;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private MinMaxFloat _emissionRange = new MinMaxFloat(0f, 1f);
    [SerializeField] private float _speed;
    [SerializeField] private Color _firstColor;
    [SerializeField] private Color _secondColor;
    private int _direction = -1;
    private float _timer = 0f;

    // Update is called once per frame
    void Update()
    {
        _timer = Mathf.Clamp01(_timer + _speed * Time.deltaTime * _direction);

        if (_canvasRenderer != null)
            _canvasRenderer.SetColor(Color.Lerp(_firstColor, _secondColor, _timer));
        if (_meshRenderer != null)
        {
            _meshRenderer.material.color = Color.Lerp(_firstColor, _secondColor, _timer);
            _meshRenderer.material.SetColor("_EmissiveColor", _meshRenderer.material.color * Mathf.Lerp(_emissionRange.Max, _emissionRange.Min, _timer));
        }

        if (_timer == 0f || _timer == 1f)
            _direction *= -1;
    }

    void OnValidate()
    {
        if (_canvasRenderer == null && GetComponent<CanvasRenderer>() != null)
            _canvasRenderer = GetComponent<CanvasRenderer>();
            
        if (_meshRenderer == null && GetComponent<MeshRenderer>() != null)
            _meshRenderer = GetComponent<MeshRenderer>();
    }
}
