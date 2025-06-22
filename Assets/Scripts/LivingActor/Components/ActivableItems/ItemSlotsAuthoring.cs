using System;
using System.Collections.Generic;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public interface IHaveSkillsManaged {
    IActivableItemSO       Passive { get; }
    List<IActivableItemSO> Skills  { get; }
}

[GhostComponent(PrefabType = GhostPrefabType.Client)]
public struct NeedInitSkillUI : IComponentData { }

public struct ItemSlotsData : IComponentData {
    [GhostField] public Strum.SlotItem.Fields<Element> data;

    #region HASH

    /// <summary>
    /// Just hash for dynamic item (not spell and skill).
    /// </summary>
    [GhostField] public int serverHash;

    /// <summary>
    /// <inheritdoc cref="serverHash"/>
    /// </summary>
    public int clientHash;
    
    public bool NeedFix => serverHash != clientHash;

    #endregion

    public struct Element {
        public Common common;

        #region FOR ITEM

        [GhostField] public ItemId itemId;

        public void SetItem(ItemId _itemId) =>
            (common.containItem, itemId) = (true, _itemId);

        public void RemoveItem() =>
            common.containItem = false;

        #endregion

        #region FOR SKILL

        [GhostField] public int level;

        #endregion

        public struct Common {
            public bool containItem;

            #region COOLDOWN

            [GhostField] public NetworkTick activatedAtTick;
            [GhostField] public NetworkTick doneAtTick;

            public void UpdateCooldownAfterActive(in NetworkTick curTick, uint cooldownTick) {
                activatedAtTick = curTick;
                doneAtTick      = curTick.WithBonusTick(cooldownTick);
            }

            #endregion
        }
    }
}

public struct SkillsData : IComponentData {
    public BlobAssetReference<ActivableItemData>                               _PassiveRef;
    public BlobAssetReference<BubleArray<ActivableItemData, IActivableItemSO>> _SkillsRef;

    public ref ActivableItemData                               Passive => ref _PassiveRef.Value;
    public ref BubleArray<ActivableItemData, IActivableItemSO> Skills  => ref _SkillsRef.Value;

    public ref ActivableItemData this[SlotItemId id] {
        get {
            if (id.IsSkill())
                return ref id == SlotItemId.Skill_Passive
                    ? ref _PassiveRef.Value
                    : ref _SkillsRef.Value[id - Strum.SlotItem.First_SkillNotPassive];

            throw new Exception($"NGDtuanh: {nameof(SkillsData)} only store data for skills, founded: {id} {(int)id}");
        }
    }
}

[RequireComponent(
    typeof(IRaceTag))]
public class ItemSlotsAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<ItemSlotsAuthoring> {
        public override void Bake(ItemSlotsAuthoring authoring) {
            if (ActorAuthoringHelpers.IsBaseRace(authoring)) return;

            GetDynamicEntity(out var entity);

            AddComponent<NeedInitSkillUI>(entity);

            ItemSlotsData slotData   = default;
            SkillsData    skillsData = default;

            var data = ActorAuthoringHelpers.ExtractDataFromTag(authoring);

            if (data is IHaveSkillsManaged skillsSource) {
                if (skillsSource.Passive != null) {
                    // Add to SkillsData
                    skillsSource.Passive.CreateBlobAssetReferenceInBaker(out skillsData._PassiveRef, this, out _);
                    // Add prefab buffer
                    skillsSource.Passive.AddPrefabBuffer(this, entity);
                    // mark contain item in ItemSlotData
                    slotData.data.Skill_Passive.common.containItem = true;
                }

                if (skillsSource.Skills != null) {
                    // Add to SkillsData
                    skillsSource.Skills.CreateBlobAssetReferenceInBaker(out skillsData._SkillsRef, this, out _);
                    for ((SlotItemId key, int index) index = (Strum.SlotItem.First_SkillNotPassive, 0)
                         ; index.key <= Strum.SlotItem.Last_Skill && index.index < skillsSource.Skills.Count
                         ; ++index.key, ++index.index) {
                        // Add prefab buffer
                        skillsSource.Skills[index.index].AddPrefabBuffer(this, entity);

                        // mark contain item in ItemSlotData
                        slotData.data.ValueRW(index.key).common.containItem = true;
                    }
                }
            }

            AddComponent(entity, slotData);
            AddComponent(entity, skillsData);
        }
    }
}