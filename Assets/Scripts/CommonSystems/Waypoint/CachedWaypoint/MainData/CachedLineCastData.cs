using Pathfinding;
using Unity.NetCode;
using UnityEngine;

public class CachedLineCastData : ICachedData<CachedLineCastData.ItemData> {
    public struct ItemData {
        public bool    haveObstacle;
        public Vector3 furthestPnt;
    }

    public ref readonly ItemData Linecast(NetworkTick curTick, NavmeshBase graph, PathId pid) {
        int code = pid.code;

        if (!ContainsCode(code)) {
            var newItem = new ItemData {
                haveObstacle = graph.Linecast(
                    TryGetExactlyEdgePnt(pid.start, graph, out var orgNode)
                  , TryGetExactlyEdgePnt(pid.end,   graph, out _)
                  , orgNode
                  , out var hit)
              , furthestPnt = hit.point
            };
            PushData(code, newItem);
        }

        if (!ContainsTick(code, curTick)) PushTick(code, curTick);

        return ref _Datas[code].data;
    }
    
    public new void TrimOldData()                    => base.TrimOldData();

    public static Vector3 TryGetExactlyEdgePnt(Vector3 pnt, NavmeshBase graph, out GraphNode node) {
        var result = graph.GetNearest(pnt, NNConstraintHub.ClosestAsSeenFromAbove);

        // This is absolutely not inside the graph
        if (result.distanceCostSqr > 1) {
            node = null;
            return pnt;
        }

        node = result.node;
        return result.position;
    }
}