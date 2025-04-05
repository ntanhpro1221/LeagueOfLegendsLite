using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public class AllMinionAuthoring : MonoBehaviour {
    public AllMinionDataSO MinionsSO;

    public class AllMinionDataBaker : Baker<AllMinionAuthoring> {
        public override void Bake(AllMinionAuthoring authoring) {
            if (authoring.MinionsSO == null) return;
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            AllMinionData data = new();
            authoring.MinionsSO.value.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}