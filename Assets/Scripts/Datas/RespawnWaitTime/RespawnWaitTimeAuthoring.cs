using Unity.Entities;
using UnityEngine;

public struct BaseRespawnWaitTimeBuffer : IBufferElementData {
    public int valuex100;
}

public class RespawnWaitTimeAuthoring : MonoBehaviour {
    public int[] BaseRespawnWaitTimesx100;

    private class Baker : Baker<RespawnWaitTimeAuthoring> {
        public override void Bake(RespawnWaitTimeAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var buffer = AddBuffer<BaseRespawnWaitTimeBuffer>(entity);
            foreach (var valuex100 in authoring.BaseRespawnWaitTimesx100)
                buffer.Add(new() { valuex100 = valuex100 });
        }
    }
}