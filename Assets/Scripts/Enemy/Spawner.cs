using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public Transform[] spawnPoints;
    public EnemyController _controller;

    public float spawnTime = 0;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            calculateClosestSpawnPoint();
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

    private void spawnRandom()
    {
        
    }
}
