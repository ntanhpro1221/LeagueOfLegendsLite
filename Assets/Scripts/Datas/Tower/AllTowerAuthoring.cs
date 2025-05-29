using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public class AllTowerAuthoring : MonoBehaviour {
    public DataSOReader soReader;

    public class AllTowerDataBaker : Baker<AllTowerAuthoring> {
        public override void Bake(AllTowerAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AllTowerData data = new();
            authoring.soReader.Tower.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}