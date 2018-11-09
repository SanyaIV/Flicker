using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EscapePodModule : Interactable {

    [Header("Parts")]
    [SerializeField] private string _module;
    [SerializeField] private List<EscapePodModel> _requiredParts;

    [Header("Escape Pod Canvas")]
    [SerializeField] private EscapePodScreen _screen;
    [SerializeField] private Text _entry;
    [SerializeField] private Text _count;

    [Header("Save")]
    private List<EscapePodModel> _savedRequiredParts;
    private bool _savedEnabled;

    [Header("Private Variables")]
    [SerializeField] private PlayerController _player;

    public override void Start ()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        _screen.AddModule(this);
        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(Reload);
        SetText();
	}
	
    public override void Interact(PlayerController player)
    {
        CheckParts(player);
        SetText();

        if(_requiredParts.Count == 0)
            _screen.UpdateScreen();
    }

    public override string ActionType()
    {

        foreach (EscapePodModel part in _requiredParts)
        {
            if (_player.HasEscapePodPart(_module, part.GetPartName()))
            {
                _showMouse = true;
                return "Repair";
            }
        }

        _showMouse = false;
        return "Damaged";
    }

    public override string GetName()
    {
        return _module;
    }

    private void CheckParts(PlayerController player)
    {
        List<EscapePodModel> repaired = null;

        foreach (EscapePodModel part in _requiredParts)
        {
            if (player.HasEscapePodPart(_module, part.GetPartName()))
            {
                if (repaired == null)
                    repaired = new List<EscapePodModel>();

                repaired.Add(part);
                part.gameObject.SetActive(true);
                part.Repair();
            }
        }

        if (repaired != null)
            foreach (EscapePodModel model in repaired) _requiredParts.Remove(model);
    }

    private void SetText()
    {

        if(_requiredParts.Count > 0)
        {
            _count.text = _requiredParts.Count.ToString();
            _count.color = Color.red;
        }
        else
        {
            _count.text = "0";
            _count.color = Color.green;
            _enabled = false;
        }
    }

    public void HideText()
    {
        _entry.enabled = false;
        _count.enabled = false;
    }

    public void ShowText()
    {
        _entry.enabled = true;
        _count.enabled = true;
    }

    public bool Repaired()
    {
        return _requiredParts.Count == 0;
    }

    public void Save()
    {
        _savedRequiredParts = new List<EscapePodModel>(_requiredParts);
        _savedEnabled = _enabled;
    }

    public void Reload()
    {
        _requiredParts = new List<EscapePodModel>(_savedRequiredParts);
        _enabled = _savedEnabled;
        SetText();
    }
}
