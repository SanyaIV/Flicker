using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private Transform _player;
    private Vector3 _savedPlayerPos;

    public void Start()
    {
        _player = GetComponent<Transform>();
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
