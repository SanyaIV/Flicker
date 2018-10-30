using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ControlRoomLights : Interactable {

    [Header("Switch")]
    [SerializeField] private bool _enabled = true;

    [Header("Trigger")]
    [SerializeField] private bool _useTrigger;
    [SerializeField] [Range(0, 100)] private int _percentTriggerChance;
    [SerializeField] private int _maxTriggerTimes;
    private int _triggerTimes;

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
        Interact();
    }

    private void Interact()
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

    public void OnTriggerEnter(Collider coll)
    {
        if (_useTrigger && _triggerTimes < _maxTriggerTimes && coll.tag == "Player")
        {
            if(Random.Range(1, 101) <= _percentTriggerChance)
            {
                Interact();
                _triggerTimes++;
            }
        }
    }
}
