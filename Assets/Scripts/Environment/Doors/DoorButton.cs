using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : Interactable {

    [Header("Doors")]
    [SerializeField] private Door[] doors;

    [Header("Passcard")]
    [SerializeField] private string _passcard;

    [Header("Basic Audio")]
    [SerializeField] private BasicAudio _basicAudio;

    public override void Interact(PlayerController player)
    {
        if (_passcard.Length > 0)
        {
            if (!player.HasPasscard(_passcard))
                return;
            else
            {
                foreach(Door door in doors)
                {
                    door.Unlock();
                }
            }
        }

        foreach (Door door in doors)
        {
            if (door.opening)
                door.Close();
            else if (door.closing)
                door.Open();
            else if (door.isOpen)
                door.Close();
            else
                door.Open();

            if (_basicAudio)
                _basicAudio.PlayAudio();
        }
    }

    public override string ActionType()
    {
        foreach (Door door in doors)
        {
            if (door.opening)
                return "Close";
            else if (door.closing)
                return "Open";
            else if (door.isOpen)
                return "Close";
            else
                return "Open";
        }

        return "open/close";
    }

    public override string GetName()
    {
        if (doors.Length > 1)
            return "Doors";
        else
            return "Door";
    }
}
