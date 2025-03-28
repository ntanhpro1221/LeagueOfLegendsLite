using Unity.Entities;

public interface IPrefabBuffer : IBufferElementData {
    public Entity Entity { get; set; }
}