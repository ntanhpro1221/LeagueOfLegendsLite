using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public class AllTowerAuthoring : MonoBehaviour {
    public AllTowerDataSO TowersSO;

    public class AllTowerDataBaker : Baker<AllTowerAuthoring> {
        public override void Bake(AllTowerAuthoring authoring) {
            if (authoring.TowersSO == null) return;
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AllTowerData data = new();
            authoring.TowersSO.value.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}