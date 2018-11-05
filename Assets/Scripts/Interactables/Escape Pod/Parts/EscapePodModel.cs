using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePodModel : MonoBehaviour {

    [Header("Part")]
    [SerializeField] private string _part;
    [SerializeField] private MeshRenderer _module;
    private bool _repaired = false;

    [Header("Light")]
    [SerializeField] private Light _light;
    [SerializeField] private Color _brokenColor;
    [SerializeField] private Color _repairedColor;

    [Header("Save")]
    private bool _savedRepaired = false;

    public void Start()
    {
        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(Reload);
        UpdateModel();
    }

    private void UpdateLight()
    {
        if (_repaired)
            _light.color = _repairedColor;
        else
            _light.color = _brokenColor;
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

	public void Repair()
    {
        _repaired = true;
        UpdateModel();
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
}
