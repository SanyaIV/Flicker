using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour {

    [Header("Tracker")]
    private bool _visited;
    private float _timeSpent;

    [Header("Area")]
    [SerializeField] private string _area;

    [Header("Save")]
    private bool _savedVisited;

    public void Awake()
    {
        AreaTracker.AddArea(this);
        GameManager.AddSaveEvent(Save);
        GameManager.AddReloadEvent(Reload);
    }

	public string GetName()
    {
        return _area;
    }

    public bool GetVisited()
    {
        return _visited;
    }

    public void IncreaseTime()
    {
        _timeSpent += Time.deltaTime;
    }

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("Player"))
        {
            _visited = true;
            AreaTracker.SetCurrentPlayerArea(this);
        }
        else if (coll.CompareTag("Enemy"))
        {
            AreaTracker.SetCurrentEnemyArea(this);
        }
    }

    public void Save()
    {
        _savedVisited = _visited;
    }

    public void Reload()
    {
        _visited = _savedVisited;
    }
}
