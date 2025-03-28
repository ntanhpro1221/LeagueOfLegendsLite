using Unity.Entities;
using UnityEngine;

public struct CommonGameRulesData : IComponentData {
    public float rotateSpeed;
}

public class CommonGameRulesAuthoring : MonoBehaviour {
    public float rotateSpeed    = 1;

    private class Baker : Baker<CommonGameRulesAuthoring> {
        public override void Bake(CommonGameRulesAuthoring authoring) {
            var eneity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(eneity, new CommonGameRulesData {
                rotateSpeed    = authoring.rotateSpeed
            });
        }
    }
}