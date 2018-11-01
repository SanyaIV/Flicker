using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoorButton : Interactable {

    [Header("Master/Slave")]
    [SerializeField] private DoorButton _master;
    private List<DoorButton> _slaves = new List<DoorButton>();

    [Header("Doors")]
    [SerializeField] private Door[] _doors;

    [Header("Passcard")]
    [SerializeField] private string _passcard;

    [Header("Basic Audio")]
    [SerializeField] private BasicAudio _basicAudio;

    [Header("Canvas")]
    [SerializeField] private Image _background;
    [SerializeField] private Image _lockedImage;
    [SerializeField] private Image _unlockedImage;
    [SerializeField] private Color _lockedColor;
    [SerializeField] private Color _openColor;

    private PlayerController _player;

    public void Awake()
    {
        if (_master) {
            _master.Submit(this);
            _doors = _master._doors;
        } 
    }

    public override void Start()
    {
        base.Start();

        _player = GameManager.player.GetComponent<PlayerController>();

        if(_passcard.Length > 0)
            foreach (Door door in _doors)
                door.Lock();

        SetCanvas();
    }

    public void Submit(DoorButton slave)
    {
        _slaves.Add(slave);
    }

    public override void Interact(PlayerController player)
    {
        if (_master)
        {
            _master.Interact(player);
            return;
        }

        if (_passcard.Length > 0 && _doors[0].locked)
        {
            if (!player.HasPasscard(_passcard))
                return;
            else
            {
                foreach(Door door in _doors)
                {
                    door.Unlock();
                }
            }
        }

        foreach (Door door in _doors)
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

        SetCanvas();
    }

    private void SetCanvas()
    {
        bool locked = false;

        foreach(Door door in _doors)
        {
            if (door.locked)
                locked = true;

            if (locked)
                break;
        }

        if (locked)
        {
            _background.color = _lockedColor;
            _lockedImage.enabled = true;
            _unlockedImage.enabled = false;
        }
        else
        {
            _background.color = _openColor;
            _lockedImage.enabled = false;
            _unlockedImage.enabled = true;
        }

        foreach (DoorButton slave in _slaves)
        {
            slave.SetCanvas();
        }
    }

    public override string ActionType()
    {
        if (_master)
            return _master.ActionType();

        if (_doors[0].locked && !_player.HasPasscard(_passcard))
            return "Locked";
        else if (_doors[0].locked && _player.HasPasscard(_passcard))
            return "Unlock";

        foreach (Door door in _doors)
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

        if (_doors.Length > 1)
            return "Doors";
        else
            return "Door";
    }
}
