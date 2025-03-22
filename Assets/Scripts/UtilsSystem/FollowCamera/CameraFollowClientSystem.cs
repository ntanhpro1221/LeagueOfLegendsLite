using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct CameraFollowClientSystem : ISystem {
     [BurstCompile]
     public void OnCreate(ref SystemState state) {
          state.RequireForUpdate<CameraFollowTransformData>();
          state.RequireForUpdate<CameraFollowTag>();
     }

     public void OnUpdate(ref SystemState state) {
          var camTrans = Camera.main?.transform;
          if (camTrans == null) return;

          var camTransDel = SystemAPI.GetSingleton<CameraFollowTransformData>();

          foreach (var localToWorld in SystemAPI
               .Query<RefRO<LocalToWorld>>()
               .WithAll<CameraFollowTag>()) {
               camTrans.position = localToWorld.ValueRO.Position + camTransDel.delta;
               break; // we just have one camera so ...
          }
     }
}