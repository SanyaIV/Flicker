using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{

    NavMeshAgent _navMeshAgent;
    public Transform[] spawnPoints;
    public EnemyController _controller;

    public float spawnTime = 0;
    public float cooldown = 0;

    private bool justSpawned = false;

    public void Start()
    {
        _navMeshAgent = _controller.GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.T))
        {
            calculateClosestSpawnPoint();
            justSpawned = true;
            Debug.Log("Just spawned");
        }
    }

    public void calculateClosestSpawnPoint()
    {
        for (int i = 0; i < spawnPoints.Length - 1; i++)
        {
            if ((_controller.player.transform.position - spawnPoints[i + 1].transform.position).magnitude < (_controller.player.transform.position - spawnPoints[i].transform.position).magnitude)
                _controller.transform.position = spawnPoints[i + 1].transform.position;
        }
    }
}
