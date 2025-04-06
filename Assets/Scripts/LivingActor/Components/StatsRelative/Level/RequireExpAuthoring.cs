using System.Collections.Generic;
using NGDtuanh.BubleAsset;
using Unity.Entities;
using UnityEngine;

public struct RequireExpData : IComponentData {
    public BlobAssetReference<BubleArray<int>> _RequireExpRef;

    public ref BubleArray<int> RequireExp => ref _RequireExpRef.Value;
    public     int             MaxLevel   => _RequireExpRef.Value.Count;

    public int CalcRequireExpForNextLevel(int curLevel) {
        --curLevel; // buffer start from 0 but level start from 1

        // invalid level
        if (curLevel >= RequireExp.Count
         || curLevel < 0) {
            Debug.LogError("Invalid level to calculate require exp");
            return 0;
        }

        return RequireExp[curLevel];
    }
}

public class RequireExpAuthoring : MonoBehaviour {
    public List<int> requireExp;

    private class Baker : Baker<RequireExpAuthoring> {
        public override void Bake(RequireExpAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var data   = new RequireExpData();
            authoring.requireExp.CreateBlobAssetReferenceInBaker(out data._RequireExpRef, this, out _);
            AddComponent(entity, data);
        }
    }
}