using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public class AllChampionAuthoring : MonoBehaviour {
    public AllChampionDataSO championsSO;

    public class AllChampionDataBaker : Baker<AllChampionAuthoring> {
        public override void Bake(AllChampionAuthoring authoring) {
            if (authoring.championsSO == null) return;
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AllChampionData data = new();
            authoring.championsSO.value.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}