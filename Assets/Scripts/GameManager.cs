using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private Transform _player;
    private Vector3 _savedPlayerPos;

    public void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    void respawn()
    {
        _player.position = _savedPlayerPos;
    }

    void save()
    {
        _savedPlayerPos = _player.position;

    }
}
