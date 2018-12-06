using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTracker : MonoBehaviour {

    [Header("Areas")]
    private static List<Area> _areas = new List<Area>();
    private static Area _currentPlayerArea;
    private static Area _currentEnemyArea;

    public void Update()
    {
        if (_currentPlayerArea != null)
            _currentPlayerArea.IncreaseTime();
    }

    public static void AddArea(Area area)
    {
        _areas.Add(area);
    }

    public static void SetCurrentPlayerArea(Area area)
    {
        if (_areas.Contains(area))
            _currentPlayerArea = area;
    }

    public static void SetCurrentEnemyArea(Area area)
    {
        if (_areas.Contains(area))
            _currentEnemyArea = area;
    }

    public static Area GetCurrentPlayerArea()
    {
        return _currentPlayerArea;
    }

    public static Area GetCurrentEnemyArea()
    {
        return _currentEnemyArea;
    }

    public static List<Area> GetVisitedAreas()
    {
        List<Area> visitedAreas = new List<Area>();

        foreach(Area area in _areas)
        {
            if (area.GetVisited())
                visitedAreas.Add(area);
        }

        return visitedAreas;
    }
}
