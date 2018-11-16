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
    [SerializeField] private float _minWaitBetweenTriggersTime;
    [SerializeField] private UnityEvent _event;
    private int _triggerTimes;
    private bool _waiting = false;

    void Trigger()
    {
        if (_triggerTimes >= _maxTriggerTimes || _waiting)
            return;

        _event.Invoke();
        _triggerTimes++;
        StartCoroutine(Wait());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == _triggerTag)
            if (Random.Range(1, 101) <= _percentTriggerChance)
                Trigger();
    }

    IEnumerator Wait()
    {
        _waiting = true;
        yield return new WaitForSeconds(_minWaitBetweenTriggersTime);
        _waiting = false;
        yield break;
    }
}
