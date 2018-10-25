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

    public void Start()
    {
        foreach (string key in _requiredParts)
            _requiredPartsDict.Add(key, false);

        _text.text = _string + " 0 of " + _requiredPartsDict.Count;
        _text.color = Color.red;
    }

    public override void Interact(PlayerController player)
    {
        foreach(string key in _requiredParts)
        {
            if (player.HasEscapePodPart(key))
                _requiredPartsDict[key] = true;
        }

        SetText();

        if(CountParts() >= _requiredPartsDict.Count)
        {
            _text.text = "Repair Complete";
            _text.color = Color.green;

            foreach(Door door in _doors)
            {
                door.Unlock();
                door.Open();
                GetComponent<BasicAudio>().PlayAudio();
            }

            Destroy(this);
        }

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
}
