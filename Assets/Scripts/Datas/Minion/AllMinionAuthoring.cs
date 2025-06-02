using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public class AllMinionAuthoring : MonoBehaviour {
    public class AllMinionDataBaker : Baker<AllMinionAuthoring> {
        public override void Bake(AllMinionAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AllMinionData data = new();
            GameSO.Minion.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}