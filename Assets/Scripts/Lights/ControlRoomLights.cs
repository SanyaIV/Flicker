﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class ControlRoomLights : Interactable {

    [Header("Switch")]
    [SerializeField] private bool _switchEnabled = true;

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
    private bool _savedSwitchMethodGroup;

    public override void Start()
    {
        base.Start();

        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(ReloadSave);
    }

    public void SwitchMethodGroup()
    {
        _switchMethodGroup = !_switchMethodGroup;
    }

    public void DisableSwitch()
    {
        _switchEnabled = false;
    }

    public void EnableSwitch()
    {
        _switchEnabled = true;
    }

    public override void Interact(PlayerController player)
    {
        Interact();
    }

    private void Interact()
    {
        if (_switchEnabled)
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

    public void Save()
    {
        _savedSwitchMethodGroup = _switchMethodGroup;
    }

    public void ReloadSave()
    {
        _switchMethodGroup = _savedSwitchMethodGroup;
    }
}
