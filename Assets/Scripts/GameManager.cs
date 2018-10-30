using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameObject player;
    private Transform _player;
    private Vector3 _savedPlayerPos;

    public void Awake()
    {
        _player = GetComponent<Transform>();
        player = gameObject;
        Save();
    }

    public void Respawn()
    {
        _player.position = _savedPlayerPos;
    }

    public void Save()
    {
        _savedPlayerPos = _player.position;
    }
}
