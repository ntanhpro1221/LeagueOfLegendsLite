using Unity.Entities;
using UnityEngine;

public struct BaseRespawnWaitTimeBuffer : IBufferElementData {
    public float_Q3 value;
}

public class RespawnWaitTimeAuthoring : MonoBehaviour {
    public float_Q3[] BaseRespawnWaitTimes;

    private class Baker : Baker<RespawnWaitTimeAuthoring> {
        public override void Bake(RespawnWaitTimeAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            var buffer = AddBuffer<BaseRespawnWaitTimeBuffer>(entity);
            foreach (var value in authoring.BaseRespawnWaitTimes)
                buffer.Add(new() { value = value });
        }
    }
}