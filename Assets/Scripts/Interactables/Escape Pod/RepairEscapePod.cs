using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairEscapePod : Interactable {

    [Header("Parts")]
    [SerializeField] private List<string> _requiredParts = new List<string>();
    [SerializeField] private Dictionary<string, bool> _requiredPartsDict = new Dictionary<string, bool>();

    [Header("Parts Text")]
    [SerializeField] private Text _text;
    [SerializeField] private string _string;

    [Header("Escape Pod Doors")]
    [SerializeField] Door[] _doors;

    public override void Start()
    {
        base.Start();

        foreach (string key in _requiredParts)
            _requiredPartsDict.Add(key, false);

        _text.text = _string + " 0 of " + _requiredPartsDict.Count;
        _text.color = Color.red;

        GameManager.AddReloadEvent(ReloadSave);
    }

    public override void Interact(PlayerController player)
    {
        foreach(string key in _requiredParts)
        {
            if (player.HasEscapePodPart(key))
                _requiredPartsDict[key] = true;
        }

        SetText();

        if(CheckParts(player))
        {
            _text.text = "Repair Complete";
            _text.color = Color.green;

            foreach(Door door in _doors)
            {
                door.Unlock();
                door.Open();
                GetComponent<BasicAudio>().PlayAudio();
            }

            this.enabled = false;
        }
    }

    private bool CheckParts(PlayerController player)
    {
        if (CountParts() >= _requiredPartsDict.Count)
            return true;
        else
            return false;
    }

    public override string ActionType()
    {
        return "Repair";
    }

    public override string GetName()
    {
        return "Escape Pod";
    }

    private int CountParts()
    {
        int count = 0;
        foreach (bool repaired in _requiredPartsDict.Values)
        {
            if (repaired)
                count++;
        }

        return count;
    }

    private void SetText()
    {
        _text.text = _string + " " + CountParts() + " of " + _requiredPartsDict.Count;
    }

    public void ReloadSave()
    {
        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        foreach (string key in _requiredParts)
        {
            if (player.HasSavedEscapePodPart(key))
                _requiredPartsDict[key] = true;
            else
                _requiredPartsDict[key] = false;
        }

        if (!CheckParts(player))
            this.enabled = true;

        SetText();
    }
}
