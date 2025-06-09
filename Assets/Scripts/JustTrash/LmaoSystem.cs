using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct LmaoSystem : ISystem {
    // [BurstCompile]
    // public void OnUpdate(ref SystemState state) {
    //     foreach (var (
    //         input
    //       , expBuffer
    //         ) in SystemAPI
    //         .Query<
    //             PlayerInputAspectRO
    //           , DynamicBuffer<IncomingExpBuffer>
    //         >().WithAll<
    //             Simulate
    //         >())
    //         if (input.GetEvent_WithData(PlayerTrigger.Item.Spell_D))
    //             expBuffer.Add(100);
    // }
}