using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public class AllEffectAuthoring : MonoBehaviour {
    public class AllEffectDataBaker : Baker<AllEffectAuthoring> {
        public override void Bake(AllEffectAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AllEffectData data = new();
            GameSO.Effect.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}