using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[UpdateInGroup(typeof(TeamMemberHandleSystemGroup))]
public partial struct UpdateTeamMemberData_ServerSystem : ISystem {
    [ReadOnly] private ComponentLookup<NetworkId> netIdLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate(SystemAPI.QueryBuilder()
            .WithAny<
                LockChampRpc
              , LockTeamRpc
            >().Build());

        netIdLookup = SystemAPI.GetComponentLookup<NetworkId>(isReadOnly: true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        netIdLookup.Update(ref state);

        var ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var member = SystemAPI.GetSingletonBuffer<TeamMemberBuffer>(isReadOnly: false);

        // LOCK CHAMPION
        foreach (var (
            rpc
          , receiveRpc
            ) in SystemAPI
            .Query<
                RefRO<LockChampRpc>
              , RpcHelpers.ReceiveRpcAspect
            >()) {
            var netId = receiveRpc.CommonProcess(ecb, netIdLookup);

            for (int i = 0; i < member.Length; ++i)
                if (member[i].netId.Value == netId.Value) {
                    member.ElementAt(i).LockChamp(rpc.ValueRO.champId);

                    break;
                }
        }

        // LOCK TEAM
        foreach (var (
            rpc
          , receiveRpc
            ) in SystemAPI
            .Query<
                RefRO<LockTeamRpc>
              , RpcHelpers.ReceiveRpcAspect
            >()) {
            var netId = receiveRpc.CommonProcess(ecb, netIdLookup);

            for (int i = 0; i < member.Length; ++i)
                if (member[i].netId.Value == netId.Value) {
                    var changedMember = member[i];
                    changedMember.team = rpc.ValueRO.teamId;
                    member.RemoveAt(i);
                    member.Add(changedMember);

                    break;
                }
        }
    }
}