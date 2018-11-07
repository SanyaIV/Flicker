using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EscapePodEngine : Interactable {

    [Header("Engine")]
    [SerializeField] private float _speed;

    [Header("Pod")]
    [SerializeField] private Transform _pod;
    private bool _activated = false;

    [Header("Enemy Spawn Point")]
    [SerializeField] Transform _spawnPoint;

    public override void Start()
    {
        base.Start();
    }

    public override void Interact(PlayerController player)
    {
        if (!_activated)
        {
            _activated = true;
            _enabled = false;
            StartCoroutine(Launch());
        }
    }

    public override string ActionType()
    {
        return "Activate";
    }

    public override string GetName()
    {
        return "Emergency Escape";
    }

    private IEnumerator Launch()
    {
        StartCoroutine(Shake());
        StartCoroutine(Move());

        if (!GameObject.FindWithTag("SelfDestruct").GetComponent<SelfDestruct>().Activated())
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().TransitionTo<MutualEscape>();
            Transform enemy = GameObject.FindWithTag("Enemy").transform;
            enemy.GetComponent<NavMeshAgent>().enabled = false;
            enemy.position = _spawnPoint.position;
            enemy.SetParent(gameObject.transform);
        }
        else
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().TransitionTo<Escape>();
        }

        yield break;
    }

    private IEnumerator Move()
    {
        while (true)
        {
            _pod.RotateAround(_pod.position, _pod.up, Time.deltaTime * 5f);
            _pod.position += _pod.forward * _speed * Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Shake()
    {
        while (true)
        {
            CameraShake.AddIntensity(1f);
            yield return new WaitForSeconds(1f);
        }
    }
}
