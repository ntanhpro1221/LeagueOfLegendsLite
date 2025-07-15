using System;
using System.Collections.Generic;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.Mathematics;
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
        #region COMMON

        public struct Common {
            public bool containItem;

            /// <summary>
            /// To implement complex conditions that  is not enough to be presented by <see cref="Strum.ItemActiveCond"/>.<br/>
            /// To implement one, just set this value automatically in a system that updated in <see cref="UpdateItemActiveRequestSystemGroup"/>.<br/>
            /// </summary>
            public bool notSatisSpecialCond;

            #region COOLDOWN

            public NetworkTick activatedAtTick;
            public NetworkTick doneAtTick;

            public void UpdateCooldownAfterActive(in NetworkTick curTick, uint cooldownTick) {
                activatedAtTick = curTick;
                doneAtTick      = curTick.WithBonusTick(cooldownTick);
            }

            #endregion
        }
        
        public Common common;

        #endregion

        #region FOR ITEM

        public ItemId itemId;

        public void SetItem(ItemId _itemId) =>
            (common.containItem, itemId) = (true, _itemId);

        public void RemoveItem() =>
            common.containItem = false;

        #endregion

        #region FOR SKILL

        public int level;

        public readonly int CalcSafeLevelIndex() => math.max(0, level - 1);

        #endregion
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
                        try {
                            skillsSource.Skills[index.index].AddPrefabBuffer(this, entity);
                        } catch (Exception e) {
                            Debug.LogException(e);
                            Debug.LogError($"NGDtuanh add skill prefab for: {authoring.name} {index.key.ToString()}");
                        }

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