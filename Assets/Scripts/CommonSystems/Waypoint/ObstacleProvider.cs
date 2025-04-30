using System.Collections.Generic;
using NGDtuanh.Singleton;
using NGDtuanh.Utils;
using Pathfinding;
using Unity.Entities;
using UnityEngine;

/// <summary>
/// Just to be used in <see cref="ProvideObstacleSystem"/>
/// </summary>
public class ObstacleProvider : SceneSingleton<ObstacleProvider> {
    [SerializeField]
    private GameObject _ObstaclePrefab;

    private readonly Stack<NavmeshCut>              _Available = new();
    private          Dictionary<Entity, NavmeshCut> _PrevUsed  = new();
    private          Dictionary<Entity, NavmeshCut> _Used      = new();

    public NavmeshCut Get(in Entity entity, bool allowCreateNewInstance) {
        NavmeshCut result;

        // Try to get from previous used cutter first
        if (!_PrevUsed.Remove(entity, out result))
            // Then try to get from available cutter
            if (!_Available.TryPop(out result)) {
                // Final, instantiate new cutter 
                if (allowCreateNewInstance)
                    result = Instantiate(_ObstaclePrefab).GetComponent<NavmeshCut>();
                else return null;
            }

        // Active and add to used cutters collection
        _Used.Add(entity, result);
        result.gameObject.SetActive(true);

        return result;
    }
    
    public void ReleaseUnusedCutter() {
        foreach (var cutter in _PrevUsed.Values) {
            _Available.Push(cutter);
            cutter.gameObject.SetActive(false);
        }

        _PrevUsed.Clear();
    }
    
    public void SwapUsedContainer() {
        Swapper.Swap(ref _PrevUsed, ref _Used);
    }
}