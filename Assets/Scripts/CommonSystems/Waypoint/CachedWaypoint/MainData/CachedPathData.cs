using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class CachedPathData : ICachedData<CachedPathData.ItemData> {
    public struct ItemData {
        public ABPath path;
        public bool   isAppliedModify;
        public bool   canReturnImmediately;

        public List<Vector3> waypoints => path.vectorPath;

        public Vector3 originPnt => path.vectorPath[^1];

        public void ApplyModify(MonoModifier modifier) {
            isAppliedModify = true;
            modifier.Apply(path);
        }
    }

    protected override void OnCleanupItem(ref ItemData item) {
        PathHolderForWaypoint.Release(item.path);
    }

    public static ItemData NewData(ABPath path) => new() {
        path = path
    };

    public new void TrimOldData()                    => base.TrimOldData();
    public     bool IsAppliedModify(int        code) => GetData(code).isAppliedModify;
    public     bool IsCanReturnImmediately(int code) => GetData(code).canReturnImmediately;
}