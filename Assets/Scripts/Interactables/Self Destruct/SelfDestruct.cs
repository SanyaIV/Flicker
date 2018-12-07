using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelfDestruct : Interactable {

    [Header("Self Destruct")]
    [SerializeField] private float _timerInSeconds = 0f;
    private Coroutine _selfDestruct;

    [Header("GUI")]
    [SerializeField] private Text _timerText;
    [SerializeField] private Text _monitorPrompt;
    [SerializeField] private Text _monitorTimer;
    [SerializeField] private string _monitorPromptDisabled = "Activate Self Destruct?";
    [SerializeField] private string _monitorPromptEnabled = "Self Destruct Activated";
    private int _minutes;
    private int _seconds;

    [Header("Save")]
    private bool _saveEnabled; //Interactable enabled, not self-destruct enabled.
    private float _saveTimerInSeconds;

    public override void Start()
    {
        base.Start();

        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(Reload);

        _monitorTimer.text = SecondsToMMSS((int)_timerInSeconds);
    }

    public override void Interact(PlayerController player)
    {
        if (!_enabled)
            return;

        _enabled = false;
        _timerText.gameObject.SetActive(true);
        _monitorPrompt.text = _monitorPromptEnabled;
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

    public string SecondsToMMSS(int seconds)
    {
        _minutes = Mathf.Clamp(seconds / 60, 0, 99);
        _seconds = Mathf.Clamp(seconds - _minutes * 60, 0, 59);
        return string.Format("{0:00}:{1:00}", _minutes, _seconds);
    }

    private IEnumerator InitiateSelfDestruct()
    {
        while(_timerInSeconds > 0)
        {
            _timerInSeconds -= Time.deltaTime;
            _timerText.text = SecondsToMMSS((int)_timerInSeconds);
            _monitorTimer.text = _timerText.text;

            yield return null;
        }
        
        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        if(!(player.currentState is DeadState) && !(player.currentState is MutualDestruction) && !(player.currentState is MutualEscape) && !(player.currentState is Escape))
        {
            GetComponent<AudioSource>().Play();
            player.TransitionTo<MutualDestruction>();
        }
        else
        {
            _timerText.text = "";
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
            _monitorPrompt.text = _monitorPromptDisabled;
            _monitorTimer.text = SecondsToMMSS((int)_timerInSeconds);
            _timerText.gameObject.SetActive(false);
        }
    }
}
