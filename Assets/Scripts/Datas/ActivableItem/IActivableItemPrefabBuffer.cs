using Unity.Entities;

public interface IActivableItemPrefabBuffer : IBufferElementData {
    Entity entity { set; } 
}