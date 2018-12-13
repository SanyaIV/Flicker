using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSensor : MonoBehaviour
{
    [Header("Sensor")]
    [SerializeField] private bool _useOnTriggerStay = false;

    [Header("Door Buttons")]
    [SerializeField] private DoorButton _doorButton;

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
            _doorButton.SensorEnter();
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Player"))
            _doorButton.SensorExit();
    }

    public void OnTriggerStay(Collider coll)
    {
        if (_useOnTriggerStay && coll.CompareTag("Player"))
            _doorButton.SensorEnter();
    }
}
