using Unity.Entities;

public interface IConstructableFromOtherVersion<TManagedType> {
    void Construct(BlobBuilder builder, IBaker baker, in TManagedType dataManaged);
}