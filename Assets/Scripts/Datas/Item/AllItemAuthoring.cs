using System.Linq;
using NGDtuanh.BlobAssetExtend;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class AllItemAuthoring : MonoBehaviour {
    public AllItemDataSO itemsSO;

    private class Baker : Baker<AllItemAuthoring> {
        public override void Bake(AllItemAuthoring authoring) {
            if (authoring.itemsSO == null) return;
            Entity entity = GetEntity(TransformUsageFlags.None);

            // MANAGED VERSION
            AddComponentObject(entity, authoring.itemsSO.value);

            // UNMANAGED VERSION
            AllItemData data = new();
            authoring.itemsSO.value.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}