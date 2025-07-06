using Unity.Burst;
using Unity.Entities;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateInGroup(typeof(InGameHandleSystemGroup))]
public partial struct HandleIncomingClient_ServerSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<IncomingClientBuffer>();
        state.RequireForUpdate<TeamMemberBuffer>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var clientBuffer = SystemAPI.GetSingletonBuffer<IncomingClientBuffer>(isReadOnly: false);
        if (clientBuffer.Length == 0) return;

        var memberBuffer = SystemAPI.GetSingletonBuffer<TeamMemberBuffer>(isReadOnly: false);
        foreach (var client in clientBuffer) memberBuffer.Add(TeamMemberBuffer.BuildFrom(client));

        clientBuffer.Clear();
    }
}