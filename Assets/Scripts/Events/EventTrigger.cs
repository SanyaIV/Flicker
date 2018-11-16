using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EventTrigger : MonoBehaviour {

    [Header("Trigger")]
    [SerializeField] private string _triggerTag = "Player";
    [SerializeField] [Range(0, 100)] private int _percentTriggerChance;
    [SerializeField] private int _maxTriggerTimes;
    [SerializeField] private UnityEvent _event;
    private int _triggerTimes;

    void Trigger()
    {
        _event.Invoke();
        _triggerTimes++;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_triggerTimes < _maxTriggerTimes && other.tag == _triggerTag)
            if (Random.Range(1, 101) <= _percentTriggerChance)
                Trigger();
    }
}
