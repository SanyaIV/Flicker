using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePodPart : Interactable {

    [Header("Repair Part")]
    [SerializeField] private string _module;
    [SerializeField] private string _part;

    public override void Start()
    {
        base.Start();

        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(ReloadSave);
    }

    public override void Interact(PlayerController player)
    {
        player.AddEscapePodPart(this);
        gameObject.SetActive(false);
    }

    public override string ActionType()
    {
        return "Pick Up";
    }

    public override string GetName()
    {
        return _module;
    }

    public string GetModule()
    {
        return _module;
    }

    public string GetPart()
    {
        return _part;
    }

    public bool IsModule(string module)
    {
        return _module == module;
    }

    public bool IsPart(string part)
    {
        return _part == part;
    }

    public bool IsPart(string module, string part)
    {
        return IsModule(module) && IsPart(part);
    }

    public void Save()
    {
        if (!gameObject.activeSelf)
        {
            GameManager.RemoveSaveEvent(Save);
            GameManager.RemoveReloadEvent(ReloadSave);
        }
    }

    public void ReloadSave()
    {
        gameObject.SetActive(true);
    }
}
