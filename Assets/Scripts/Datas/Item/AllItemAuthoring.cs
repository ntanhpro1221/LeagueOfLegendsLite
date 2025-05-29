using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public class AllItemAuthoring : MonoBehaviour {
    public DataSOReader soReader;
    
    private class Baker : Baker<AllItemAuthoring> {
        public override void Bake(AllItemAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.None);
            
            AllItemData data = new();
            authoring.soReader.Item.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}