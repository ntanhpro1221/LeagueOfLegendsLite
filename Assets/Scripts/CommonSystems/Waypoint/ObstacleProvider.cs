using System;
using System.Collections.Generic;
using System.Linq;
using NGDtuanh.Singleton;
using NGDtuanh.Utils;
using Pathfinding;
using Pathfinding.Util;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Just to be used in <see cref="PrepareObstacleDataSystem"/>
/// </summary>
public class ObstacleProvider : SceneSingleton<ObstacleProvider> {
    private static readonly Vector3 BLANK_ROOM = new Vector3(1e5f, 1e5f, 1e5f);

    [SerializeField]
    private GameObject _ObstaclePrefab;

    private readonly Stack<NavmeshCut> _Available = new();
    private readonly Stack<NavmeshCut> _Used      = new();

    public void ReleaseAllCutter() {
        foreach (var cutter in _Used) {
            _Available.Push(cutter);
            cutter.transform.position = BLANK_ROOM;
        }

        _Used.Clear();
    }

    public NavmeshCut Get() {
        NavmeshCut result;

        // Try to get from available cutter
        if (!_Available.TryPop(out result))
            // instantiate new cutter 
            result = Instantiate(_ObstaclePrefab).GetComponent<NavmeshCut>();

        // Add to used cutters collection
        _Used.Push(result);

        return result;
    }
}