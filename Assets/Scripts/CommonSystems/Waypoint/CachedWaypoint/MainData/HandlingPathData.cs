using Pathfinding;
using Unity.NetCode;

public class HandlingPathData : ICachedData<HandlingPathData.ItemData> {
    public struct ItemData {
        public ABPath path;
    }

    public static ItemData NewData(ABPath path) => new() {
        path = path
    };

    public void ForceComplete(NetworkTick startFromTick, in CachedPathData cachedPath) {
        foreach (var (tick, codes) in _TickRefs) {
            if (tick.IsNewerThan(startFromTick)) break;

            foreach (var code in codes) {
                var path = _Datas[code].data.path;

                // Wait for complete
                AstarPath.BlockUntilCalculated(path);

                // Apply Filter
                path.vectorPath.Add(path.endPoint);
                path.vectorPath.Reverse();
                path.vectorPath.Add(path.startPoint);

                // Cache data
                cachedPath.PushData(code, CachedPathData.NewData(path));

                foreach (var tickOfOldCode in _Datas[code].tickRefs) {
                    // push tick
                    cachedPath.PushTick(code, tickOfOldCode);

                    // save all tick for later pop tick
                    _Tmp_TickStack.Push(tickOfOldCode);
                }
            }
        }

        while (_Tmp_TickStack.Count > 0)
            PopTick(_Tmp_TickStack.Pop());
    }

    public override void DisposeAll() {
        foreach (var dataPair in _Datas.Values) {
            dataPair.data.path.Error();
            PathHolderForWaypoint.Release(dataPair.data.path);
        }

        base.DisposeAll();
    }
}