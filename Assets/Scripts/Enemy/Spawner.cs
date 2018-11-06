using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{

    NavMeshAgent _navMeshAgent;
    private List<Transform> _spawnPoints;
    public EnemyController _controller;
    public float spawnTime = 0;
    public float correct = 10f;

    [SerializeField] private MinMaxFloat _randomNum;

    public void Start()
    {
        _navMeshAgent = _controller.GetComponent<NavMeshAgent>();  
        _spawnPoints = new List<Transform>();

        foreach(GameObject go in GameObject.FindGameObjectsWithTag("Spawn Point"))
        {
            _spawnPoints.Add(go.transform);
        }
        StartCoroutine(SpawnWhenClose());
        
    }

    public IEnumerator SpawnWhenClose()
    {
        float drainDistance = _controller.GetState<Hunt>()._distanceToDepleteSanity;
       
        while(true)
        {
            if(Vector3.Distance(_controller.transform.position, _controller.player.transform.position) < drainDistance && !(_controller.visible))
            {
                if (Random.Range(0, 100) <= correct)
                {
                    _controller.transform.position = _spawnPoints[Random.Range(0, _spawnPoints.Count)].position;
                }
            }
            yield return new WaitForSeconds(Random.Range(_randomNum.Min, _randomNum.Max));
        }
    }
}