using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(HandleShopUISystemGroup))]
public partial struct UpdateShopUI_OwnChamp_ClientSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<OwnChampBuyable>();
    }

    public void OnUpdate(ref SystemState state) {
        if (!ShopUI.IsAvailable) return;
        
        var shop = ShopUI.Instance;
        if (!shop.Visible) return;

        ref var buyable = ref SystemAPI.GetSingletonRW<OwnChampBuyable>().ValueRW;

        foreach (var gold in SystemAPI
            .Query<
                RefRO<GoldData>
            >().WithAll<
                ChampionTag
              , GhostOwnerIsLocal
            >().WithNone<
                DummyTag
            >()) {
            shop.Coin.Value = gold.ValueRO.gold;
            if (buyable.PopNeedUpdate()) shop.Buyable.ForceAssignAndUpdate(buyable.buyable);
        }
    }
}