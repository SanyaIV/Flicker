using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour {

    [Header("Tracker")]
    private bool _visited = false;
    private float _timeSpent;

    [Header("Area")]
    [SerializeField] private string _area;

    [Header("Save")]
    private bool _savedVisited = false;

    public void Awake()
    {
        AreaTracker.AddArea(this);
        GameManager.AddSaveEvent(Save);
        GameManager.AddLateReloadEvent(Reload);
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

    public float GetTimeSpent()
    {
        return _timeSpent;
    }

    public void OnTriggerStay(Collider coll)
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
