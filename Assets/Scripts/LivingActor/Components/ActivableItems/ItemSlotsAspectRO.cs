using Unity.Entities;

public readonly partial struct ItemSlotsAspectRO : IAspect {
    private readonly RefRO<SkillsData>    _Skills;
    private readonly RefRO<ItemSlotsData> _Slots;

    public ref readonly SkillsData Skills => ref _Skills.ValueRO;

    public ref readonly Strum.SlotItem.Fields<ItemSlotsData.Element> Slots => ref _Slots.ValueRO.data;

    public ref readonly ItemSlotsData RawSlots => ref _Slots.ValueRO;
    
    /// <summary>
    /// Only use it when you sure about the existence of this <see cref="id"/> in <see cref="Slots"/>
    /// </summary>
    public ref ActivableItemData GetItemDataUnsafe(SlotItemId id, in AllItemData allItem) =>
        ref id.IsSkill()
            ? ref Skills[id]
            : ref allItem.Items[Slots[id].itemId].common;

    public bool IsActivable(SlotItemId id, in AllItemData allItem) =>
        Slots[id].common.containItem
     && GetItemDataUnsafe(id, allItem).activeSettings.isActivable;
}