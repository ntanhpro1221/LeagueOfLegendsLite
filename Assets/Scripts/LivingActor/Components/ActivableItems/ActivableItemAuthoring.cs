using System;
using NGDtuanh.BubleAsset;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct NeedBuildActivableItemData : IComponentData { }

public struct AllActivableItemData : ICleanupComponentData, IDisposable {
    public NativeList<BlobAssetReference<ActivableItemData>> _Data;

    private void TryDisposeElement(int index) {
        if (!_Data[index].IsCreated) return;
        _Data[index].Dispose();
        _Data[index] = default;
    }

    public ref ActivableItemData this[PlayerTrigger.Item key] => ref _Data[(int)key].Value;

    public readonly bool IsActivable(PlayerTrigger.Item key) =>
        _Data[(int)key].IsCreated
     && _Data[(int)key].Value.activeSettings.isActivable;

    public void Set(PlayerTrigger.Item key, ref ActivableItemData value) {
        int index = (int)key;
        TryDisposeElement(index);
        value.CreateBlobAssetReference(out _Data.ElementAt(index));
    }

    public void Init(ref ChampionData source) {
        _Data = new(Allocator.Persistent);
        _Data.Resize(PlayerTrigger.ITEM_COUNT, NativeArrayOptions.ClearMemory);
        
        Set(PlayerTrigger.Item.Skill_Passive, ref source.passive);
        
        Set(PlayerTrigger.Item.Skill_Q, ref source.skills[0]);
        Set(PlayerTrigger.Item.Skill_W, ref source.skills[1]);
        Set(PlayerTrigger.Item.Skill_E, ref source.skills[2]);
        Set(PlayerTrigger.Item.Skill_R, ref source.skills[3]);
    }

    public void Dispose() {
        if (!_Data.IsCreated) return;
        
        for (int i = 0; i < PlayerTrigger.ITEM_COUNT; ++i) TryDisposeElement(i);
        _Data.Dispose();
    }
}

public struct ActivableItemBonusBuffer : IBufferElementData {
    [GhostField] public NetworkTick activatedAtTick;
    [GhostField] public NetworkTick doneAtTick;

    [GhostField] public ItemId itemId;

    [GhostField] public int level;

    public void UpdateCooldownAfterActive(in NetworkTick curTick, uint cooldownTick) {
        activatedAtTick = curTick;
        doneAtTick      = curTick.WithBonusTick(cooldownTick);
    }
}

public class ActivableItemAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<ActivableItemAuthoring> {
        public override void Bake(ActivableItemAuthoring authoring) {
            GetDynamicEntity(out var entity);
            
            AddComponent<NeedBuildActivableItemData>(entity);
            AddCleanBuffer<ActivableItemBonusBuffer>(entity, PlayerTrigger.ITEM_COUNT);

            if (TryAddChampSkillPrefab(entity, authoring)) return;
        }

        private bool TryAddChampSkillPrefab(in Entity entity, ActivableItemAuthoring authoring) {
            var champTag = authoring.GetComponent<ChampionTagAuthoring>();
            if (champTag == null) return false;

            GameSO.Champ[champTag.id].AddAllSkillPrefabBuffer(this, entity);
            return true;
        }
    }
}