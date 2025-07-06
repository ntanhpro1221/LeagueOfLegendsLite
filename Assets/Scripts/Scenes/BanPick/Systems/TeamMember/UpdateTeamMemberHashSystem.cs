using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(TeamMemberHandleSystemGroup), OrderLast = true)]
public partial struct UpdateTeamMemberHashSystem : ISystem {
    private bool isClient;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        isClient = state.WorldUnmanaged.IsClient();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (
            buffer
          , data
            ) in SystemAPI
            .Query<
                DynamicBuffer<TeamMemberBuffer>
              , RefRW<TeamMemberData>
            >())
            if (isClient) data.ValueRW.UpdateHash_Client();
            else data.ValueRW.UpdateHash_Server(buffer);
    }
}