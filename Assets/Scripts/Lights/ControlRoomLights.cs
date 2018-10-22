using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ControlRoomLights : Interactable {

    [Header("Switch")]
    [SerializeField] private bool _enabled = true;

    [Header("Method Group 1")]
    [SerializeField] private UnityEvent _methodGroup1;

    [Header("Method Group 2")]
    [SerializeField] private UnityEvent _methodGroup2;

    private bool _switchMethodGroup;

    public void SwitchMethodGroup()
    {
        _switchMethodGroup = !_switchMethodGroup;
    }

    public void DisableSwitch()
    {
        _enabled = false;
    }

    public void EnableSwitch()
    {
        _enabled = true;
    }

    public override void Interact(PlayerController player)
    {
        if (_enabled)
        {
            if (!_switchMethodGroup)
                _methodGroup1.Invoke();
            else if (_switchMethodGroup)
                _methodGroup2.Invoke();
        }
    }

    public override string ActionType()
    {
        return "Use";
    }

    public override string GetName()
    {
        return "Light Switch";
    }
}
