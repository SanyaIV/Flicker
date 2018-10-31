using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : Interactable {

    [Header("Master/Slave")]
    [SerializeField] private DoorButton _master;

    [Header("Doors")]
    [SerializeField] private Door[] doors;

    [Header("Passcard")]
    [SerializeField] private string _passcard;

    [Header("Basic Audio")]
    [SerializeField] private BasicAudio _basicAudio;

    private PlayerController _player;

    public void Start()
    {
        _player = GameManager.player.GetComponent<PlayerController>();

        if(_passcard.Length > 0)
            foreach (Door door in doors)
                door.Lock();
    }

    public override void Interact(PlayerController player)
    {
        if (_master)
        {
            _master.Interact(player);
            return;
        }
           

        if (_passcard.Length > 0 && doors[0].locked)
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
        if (_master)
            return _master.ActionType();

        if (doors[0].locked && !_player.HasPasscard(_passcard))
            return "Locked";
        else if (doors[0].locked && _player.HasPasscard(_passcard))
            return "Unlock";

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
        if (_master)
            return _master.GetName();

        if (doors.Length > 1)
            return "Doors";
        else
            return "Door";
    }
}
