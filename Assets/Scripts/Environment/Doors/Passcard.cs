using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passcard : Interactable {

    [Header("Passcard")]
    [SerializeField] private string _passcard;

    public override void Start()
    {
        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(ReloadSave);
    }

    public override void Interact(PlayerController player)
    {
        player.AddPasscard(_passcard);
        gameObject.SetActive(false);
    }

    public override string ActionType()
    {
        return "Pick Up";
    }

    public override string GetName()
    {
        return "Passcard";
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
