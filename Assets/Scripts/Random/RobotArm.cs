using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArm : MonoBehaviour
{
    private Vector3 _lastPosition;

    void LateUpdate()
    {
        _lastPosition = transform.position;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            Vector3 direction = transform.position - _lastPosition;

            coll.GetComponent<CharacterController>().Move(direction * 10f);
        }
    }
}
