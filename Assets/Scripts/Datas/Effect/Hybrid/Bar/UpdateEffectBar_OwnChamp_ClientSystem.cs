using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(HandleEffectClientUISystemGroup))]
public partial struct UpdateEffectBar_OwnChamp_ClientSystem : ISystem {
    [ReadOnly] private ComponentLookup<SetNameRequest> nameLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<NetworkTime>();

        nameLookup = SystemAPI.GetComponentLookup<SetNameRequest>(
            isReadOnly: true);
    }

    public void OnUpdate(ref SystemState state) {
        nameLookup.Update(ref state);

        foreach (var (
            effectBuffer
          , effectHash
            ) in SystemAPI
            .Query<
                DynamicBuffer<EffectBuffer>
              , RefRO<EffectBufferHashData>
            >().WithAll<
                ChampionTag
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >()) {
            if (effectHash.ValueRO.NeedFix)
                PlayerHUD.Instance.EffectBarUI.FixAllUI(effectBuffer, nameLookup);

            PlayerHUD.Instance.EffectBarUI.UpdateAllUI(
                SystemAPI.GetSingleton<NetworkTime>().ServerTick
              , effectBuffer);
        }
    }
}