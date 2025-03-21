using Unity.Entities;
using UnityEngine;

namespace NGDtuanh.BlobAssetExtend {
    public struct BubleEntity : IBlobBuildable<GameObject> {
        public Entity Value;
        
        public void BuildBlob(ref BlobBuilder builder, GameObject source, IBaker baker) {
            Value = baker.GetEntity(source, TransformUsageFlags.Dynamic);
        }
    }
}