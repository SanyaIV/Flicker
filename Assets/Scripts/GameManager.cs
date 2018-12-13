﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager gameManager;
    public static bool visualIconsEnabled = false;
    public static bool paused = false;
    public static bool pausePlayerMovement = false;
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
        Time.timeScale = 1f;

        if (gameManager == null)
            gameManager = this;
        if (gameManager != this)
            Destroy(this);

        _player = GetComponent<Transform>();
        player = gameObject;
    }

    public void Start()
    {
        StartCoroutine(LateStart());
        StartCoroutine(FadeIn());
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
                UnPause();
            else
                Pause();
        }
    }

    private void UnPause()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        pauseScreen.SetActive(false);
        paused = false;
        pausePlayerMovement = false;
        AudioListener.pause = false;
    }

    private void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        pauseScreen.SetActive(true);
        paused = true;
        pausePlayerMovement = true;
        AudioListener.pause = true;
    }

    public static void Respawn()
    {
        _player.position = _savedPlayerPos;
        _reloadEventManager.Invoke();
        gameManager.StartCoroutine(LateReload());
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

    public void RestartGame()
    {
        UnPause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator LateStart()
    {
        yield return null;
        Save();
        yield break;
    }

    private static IEnumerator LateReload()
    {
        yield return null;
        _lateReloadEventManager.Invoke();
        yield break;
    }

    private IEnumerator FadeIn()
    {
        Image fade = GameObject.FindGameObjectWithTag("HUD Fade In").GetComponent<Image>();
        fade.color = new Color(0f, 0f, 0f, 1f);

        while(fade != null && fade.color.a > 0f)
        {
            fade.color = new Color(0f, 0f, 0f, fade.color.a - 0.5f * Time.deltaTime);
            yield return null;
        }

        yield break;
    }
}
