using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {

    [Header("Spawn")]
    [SerializeField] private bool _allowSpawn = false;

    [Header("Area")]
    [SerializeField] private string _area;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, 1);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _allowSpawn ? new Color(0, 1, 0, 1) : new Color(1, 0, 0, 1);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    public string GetArea()
    {
        return _area;
    }

    public bool GetSpawnAllowed()
    {
        return _allowSpawn;
    }
}
