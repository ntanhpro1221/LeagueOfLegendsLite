using System.Linq;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class AllMonsterAuthoring : MonoBehaviour {
    public class AllMonsterDataBaker : ExtendBaker<AllMonsterAuthoring> {
        public override void Bake(AllMonsterAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AllMonsterData data = new();
            GameSO.Monster.CreateBlobAssetReferenceInBaker(out data._Ref, this, out _);
            AddComponent(entity, data);
        }
    }
}