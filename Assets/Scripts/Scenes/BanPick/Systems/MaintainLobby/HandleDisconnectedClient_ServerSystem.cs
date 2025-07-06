using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateAfter(typeof(NetworkReceiveSystemGroup))]
public partial struct HandleDisconnectedClient_ServerSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkStreamDriver>();
        state.RequireForUpdate<TeamMemberBuffer>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var events = SystemAPI.GetSingleton<NetworkStreamDriver>().ConnectionEventsForTick;
        if (events.Length == 0) return;
        
        var member = SystemAPI.GetSingletonBuffer<TeamMemberBuffer>(isReadOnly: false);

        foreach (var evt in events)
            if (evt.State == ConnectionState.State.Disconnected)
                member.Remove(evt.Id);
    }
}