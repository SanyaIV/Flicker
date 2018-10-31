using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePodPart : Interactable {

    [Header("Part")]
    [SerializeField] private string _part;

    public void Start()
    {
        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(ReloadSave);
    }

    public override void Interact(PlayerController player)
    {
        player.AddEscapePodPart(_part);
        gameObject.SetActive(false);
    }

    public override string ActionType()
    {
        return "Pick Up";
    }

    public override string GetName()
    {
        return "Part";
    }

    public void Save()
    {
        if (!gameObject.activeSelf)
        {
            GameManager.RemoveSaveEvent(Save);
            GameManager.RemoveReloadEvent(ReloadSave);
            Destroy(gameObject);
        }
           
    }

    public void ReloadSave()
    {
        gameObject.SetActive(true);
    }
}
