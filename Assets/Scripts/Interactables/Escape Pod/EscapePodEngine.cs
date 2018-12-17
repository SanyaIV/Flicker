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

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private AudioClip _engineHum;

    [Header("Enemy Spawn Point")]
    [SerializeField] Transform _spawnPoint;

    public override void Interact(PlayerController player)
    {
        base.Interact(player);

        if (!_activated)
        {
            player.transform.parent = transform;
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
        _audioSource.clip = _engineHum;
        _audioSource.loop = true;
        _audioSource.Play();
        _audioSource.PlayOneShot(_audioClip);

        if (!GameObject.FindWithTag("SelfDestruct").GetComponent<SelfDestruct>().Activated())
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().TransitionTo<MutualEscape>();
            Transform enemy = GameObject.FindWithTag("Enemy").transform;
            enemy.GetComponent<NavMeshAgent>().enabled = false;
            enemy.position = _spawnPoint.position;
            enemy.rotation = _spawnPoint.rotation;
            enemy.SetParent(gameObject.transform);
        }
        else
        {
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().TransitionTo<Escape>();
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(Shake());
        StartCoroutine(Move());

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
