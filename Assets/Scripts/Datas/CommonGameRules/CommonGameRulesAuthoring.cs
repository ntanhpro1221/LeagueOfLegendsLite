using Unity.Entities;
using UnityEngine;

public struct CommonGameRulesData : IComponentData {
    public float rotateSpeed;
    public float scaleMoveSpeed;
}

public class CommonGameRulesAuthoring : MonoBehaviour {
    public float rotateSpeed    = 1;
    public float scaleMoveSpeed = 0.1f;

    private class Baker : Baker<CommonGameRulesAuthoring> {
        public override void Bake(CommonGameRulesAuthoring authoring) {
            var eneity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(eneity, new CommonGameRulesData {
                rotateSpeed    = authoring.rotateSpeed
              , scaleMoveSpeed = authoring.scaleMoveSpeed
            });
        }
    }
}