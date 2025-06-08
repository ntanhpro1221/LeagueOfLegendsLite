using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public class AllChampionAuthoring : MonoBehaviour {
    public class AllChampionDataBaker : Baker<AllChampionAuthoring> {
        public override void Bake(AllChampionAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AllChampionData data = new();
            data.CreateBlobAssetReferenceInBaker(this);
            AddComponent(entity, data);
        }
    }
}