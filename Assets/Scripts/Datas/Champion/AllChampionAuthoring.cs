using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public class AllChampionAuthoring : MonoBehaviour {
    public DataSOReader soReader;

    public class AllChampionDataBaker : Baker<AllChampionAuthoring> {
        public override void Bake(AllChampionAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AllChampionData data = new();
            authoring.soReader.Champ.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}