using Unity.Collections;
using Unity.Entities;

public readonly partial struct ItemDataAspectRO : IAspect {
    private readonly    RefRO<AllActivableItemData> _Static;
    public ref readonly AllActivableItemData        Static => ref _Static.ValueRO;
    
    [ReadOnly] public readonly DynamicBuffer<ActivableItemBonusBuffer> Dynamic;
}