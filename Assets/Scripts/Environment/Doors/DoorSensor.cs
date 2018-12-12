using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSensor : MonoBehaviour
{
    [Header("Door Buttons")]
    [SerializeField] private DoorButton[] _doorButtons;

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
            foreach (DoorButton door in _doorButtons)
                door.SensorEnter();
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Player"))
            foreach (DoorButton door in _doorButtons)
                door.SensorExit();
    }
}
