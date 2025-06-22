using System;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(HandleShopUISystemGroup))]
[UpdateBefore(typeof(UpdateShopUI_OwnChamp_ClientSystem))]
public partial struct UpdateBuyable_OwnChamp_ClientSystem : ISystem {
    private EntityQuery ownChampQuery;

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AllItemData>();
        state.RequireForUpdate(ownChampQuery = SystemAPI.QueryBuilder()
            .WithAll<
                ChampionTag
              , GhostOwnerIsLocal
              , ItemSlotsData
              , GoldData
            >().WithNone<
                DummyTag
            >().Build());
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        state.CompleteDependency();

        ref var buyable = ref SystemAPI.GetSingletonRW<OwnChampBuyable>().ValueRW;

        var allItem  = SystemAPI.GetSingleton<AllItemData>();
        var slotData = ownChampQuery.GetSingleton<ItemSlotsData>();
        var goldData = ownChampQuery.GetSingleton<GoldData>();

        var newHash = 0;
        foreach (var itemId in Strum.Items.Indexes) {
            var requiredGold = allItem.Items[itemId].settings.cost;

            Strum.SlotItem.Fields<bool> sacrificeSlots = default;
            ModifyItemSystem.FindSacrificeSlots(ref sacrificeSlots, itemId, slotData, ref allItem);
            for (var slot = Strum.SlotItem.Last_Item; slot >= Strum.SlotItem.First_Item; --slot)
                if (sacrificeSlots[slot])
                    requiredGold -= allItem.Items[slotData.data[slot].itemId].settings.cost;

            buyable.buyable[itemId] = requiredGold <= goldData.gold;

            // Add to new hash
            newHash = HashCode.Combine(newHash, buyable.buyable[itemId]);
        }

        if (buyable.hash != newHash) {
            buyable.hash = newHash;
            buyable.MarkNeedUpdate();
        }
    }
}