using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct WaypointCalculateConfig : IComponentData {
    public uint  tickToCalculate;
    public float fixablePathDisSqr;

    public readonly NetworkTick DoneAtTick(NetworkTick curTick) {
        curTick.Add(tickToCalculate);
        return curTick;
    }
}

public class WaypointCalculateConfigAuthoring : MonoBehaviour {
    public uint  tickToCalculate = 5;
    public float fixablePathDis  = 666;

    private class Baker : ExtendBaker<WaypointCalculateConfigAuthoring> {
        public override void Bake(WaypointCalculateConfigAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new WaypointCalculateConfig {
                tickToCalculate   = authoring.tickToCalculate
              , fixablePathDisSqr = authoring.fixablePathDis.Sqr()
            });
        }
    }
}