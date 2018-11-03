using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : Interactable {

    [Header("Self Destruct")]
    [SerializeField] private float _timerInSeconds;
    private Coroutine _selfDestruct;

    [Header("Save")]
    private bool _saveEnabled;
    private float _saveTimerInSeconds;

    public override void Start()
    {
        base.Start();

        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(Reload);
    }

    public override void Interact(PlayerController player)
    {
        if (!_enabled)
            return;

        _enabled = false;
        _selfDestruct = StartCoroutine(InitiateSelfDestruct());
    }

    public override string ActionType()
    {
        return "Activate";
    }

    public override string GetName()
    {
        return "Self Destruct";
    }

    public bool Activated()
    {
        return !_enabled;
    }

    private IEnumerator InitiateSelfDestruct()
    {
        Debug.LogWarning("Self Destruct Initiated!");

        while(_timerInSeconds > 0)
        {
            _timerInSeconds -= Time.deltaTime;

            yield return null;
        }
        
        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        if(!(player.currentState is DeadState) && !(player.currentState is MutualDestruction) && !(player.currentState is MutualEscape) && !(player.currentState is Escape))
        {
            Debug.LogWarning("BOOM!");
            GetComponent<AudioSource>().Play();
            player.TransitionTo<MutualDestruction>();
        }
            
    }

    public void Save()
    {
        _saveEnabled = _enabled;
        _saveTimerInSeconds = _timerInSeconds;
    }

    public void Reload()
    {
        _enabled = _saveEnabled;
        _timerInSeconds = _saveTimerInSeconds;

        if(_enabled && _selfDestruct != null)
        {
            StopCoroutine(_selfDestruct);
        }
    }
}
