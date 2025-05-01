using Unity.Entities;
using UnityEngine;

public struct MinionBehaviourConfigData : IComponentData {
    public float_Q3 aggroCD;
    public float_Q3 reachPathDisToleranceSqr;
}

public class MinionBehaviourConfigAuthoring : MonoBehaviour {
    public float_Q3 aggroCD               = 3;
    public float_Q3 reachPathDisTolerance = 10;

    private class Baker : ExtendBaker<MinionBehaviourConfigAuthoring> {
        public override void Bake(MinionBehaviourConfigAuthoring authoring) {
            GetDynamicEntity(out var entity);

            AddComponent(entity, new MinionBehaviourConfigData {
                aggroCD                  = authoring.aggroCD
              , reachPathDisToleranceSqr = authoring.reachPathDisTolerance.Sqr()
            });
        }
    }
}