using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePodModel : MonoBehaviour {

    [Header("Part")]
    [SerializeField] private string _part;
    [SerializeField] private MeshRenderer _module;
    private bool _repaired = false;

    [Header("Light")]
    [SerializeField] private float _delay = 0.5f;
    [SerializeField] private GameObject _light;
    [SerializeField] private Material _brokenMaterial;
    [SerializeField] private Material _repairedMaterial;
    private Renderer _lightRenderer;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _beepSound;

    [Header("Save")]
    private bool _savedRepaired = false;

    public void Start()
    {
        _lightRenderer = _light.GetComponent<Renderer>();

        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(Reload);

        UpdateModel();
    }

    private void UpdateLight()
    {
        if (_repaired)
            _lightRenderer.material = _repairedMaterial;
        else
            _lightRenderer.material = _brokenMaterial;
    }

    private void UpdateMesh()
    {
        if (_repaired)
            _module.enabled = true;
        else
            _module.enabled = false;
    }

    private void UpdateModel()
    {
        UpdateMesh();
        UpdateLight();
    }

    private void UpdateModelDelayedLight()
    {
        UpdateMesh();
        StartCoroutine(WaitToLight());
    }

	public void Repair()
    {
        _repaired = true;
        UpdateModelDelayedLight();
        _audioSource.PlayOneShot(_clickSound);
    }

    public bool IsRepaired()
    {
        return _repaired;
    }

    public string GetPartName()
    {
        return _part;
    }

    public void Save()
    {
        _savedRepaired = _repaired;
    }

    public void Reload()
    {
        _repaired = _savedRepaired;
        UpdateModel();
    }

    private IEnumerator WaitToLight()
    {
        yield return new WaitForSeconds(_delay);

        UpdateLight();
        _audioSource.PlayOneShot(_beepSound);
    }
}
