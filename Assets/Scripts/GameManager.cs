using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {

    public static GameObject player;
    private static UnityEvent _saveEventManager = new UnityEvent();
    private static UnityEvent _reloadEventManager = new UnityEvent();
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

    public static void RemoveReloadEvent(UnityAction call)
    {
        _reloadEventManager.RemoveListener(call);
    }

    private IEnumerator LateStart()
    {
        yield return null;
        Save();
        yield break;
    }
}
