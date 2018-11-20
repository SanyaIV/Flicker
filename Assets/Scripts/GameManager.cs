using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static bool visualIconsEnabled = true;
    public static bool paused = false;
    public GameObject pauseScreen;
    public AudioMixer audioMixer;

    public static GameObject player;
    private static UnityEvent _saveEventManager = new UnityEvent();
    private static UnityEvent _reloadEventManager = new UnityEvent();
    private static UnityEvent _lateReloadEventManager = new UnityEvent();
    private static Transform _player;
    private static Vector3 _savedPlayerPos;

    public void Awake()
    {
        _player = GetComponent<Transform>();
        player = gameObject;
    }

    public void Start()
    {
        StartCoroutine(LateStart());
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1f;
                pauseScreen.SetActive(false);
                paused = false;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
                pauseScreen.SetActive(true);
                paused = true;
            }
        }
    }

    public static void Respawn()
    {
        _player.position = _savedPlayerPos;
        _reloadEventManager.Invoke();
    }

    public static void Save()
    {
        _savedPlayerPos = _player.position;
        _saveEventManager.Invoke();
    }

    public static void AddSaveEvent(UnityAction call)
    {
        _saveEventManager.AddListener(call);
    }

    public static void RemoveSaveEvent(UnityAction call)
    {
        _saveEventManager.RemoveListener(call);
    }

    public static void AddReloadEvent(UnityAction call)
    {
        _reloadEventManager.AddListener(call);
    }

    public static void AddLateReloadEvent(UnityAction call)
    {
        _lateReloadEventManager.AddListener(call);
    }

    public static void RemoveReloadEvent(UnityAction call)
    {
        _reloadEventManager.RemoveListener(call);
    }

    public static void RemoveLateReloadEvent(UnityAction call)
    {
        _lateReloadEventManager.RemoveListener(call);
    }

    public void SetMouseSensitivity(float value)
    {
        player.GetComponentInChildren<FirstPersonCamera>().speed = new Vector2(value, value);
    }

    public void SetMasterAudioVolume(float value)
    {
        audioMixer.SetFloat("masterVol", value);
    }

    public void SetVisualIcons(bool enabled)
    {
        visualIconsEnabled = enabled;
    }

    private IEnumerator LateStart()
    {
        yield return null;
        Save();
        yield break;
    }
}
