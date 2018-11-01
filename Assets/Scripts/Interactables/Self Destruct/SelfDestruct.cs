using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : Interactable {

    [Header("Self Destruct")]
    [SerializeField] private float _timerInSeconds;

    public override void Interact(PlayerController player)
    {
        if (!_enabled)
            return;

        _enabled = false;
        StartCoroutine(InitiateSelfDestruct());
    }

    public override string ActionType()
    {
        return "Activate";
    }

    public override string GetName()
    {
        return "Self Destruct";
    }

    private IEnumerator InitiateSelfDestruct()
    {
        Debug.LogWarning("Self Destruct Initiated!");

        while(_timerInSeconds > 0)
        {
            _timerInSeconds -= Time.deltaTime;

            yield return null;
        }

        Debug.LogWarning("BOOM!");
    }
}
