using System.Linq;
using BlobAssetExtend;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class AllItemAuthoring : MonoBehaviour {
    public AllItemDataSO itemsSO;

    private class Baker : Baker<AllItemAuthoring> {
        public override void Bake(AllItemAuthoring authoring) {
            if (authoring.itemsSO == null) return;
            Entity entity = GetEntity(TransformUsageFlags.None);
            
            // MANAGED
            AddComponentObject(entity, authoring.itemsSO.items);
            
            // UNMANAGED
            using var builder = new BlobBuilder(Allocator.Temp);

            ref var hashMap    = ref builder.ConstructRoot<BlobHashMap<EquatableEnum<ItemId>, ItemData>>();
            builder.SetHashMap(this, ref hashMap, authoring.itemsSO.items.ToList().ToEquatableEnumCollectionKey());

            var blobRef = builder.CreateBlobAssetReference<BlobHashMap<EquatableEnum<ItemId>, ItemData>>(Allocator.Persistent);
            
            AddBlobAsset(ref blobRef, out var hash);
            
            AddComponent(entity, new AllItemData {
                items = blobRef
            });
        }
    }
}