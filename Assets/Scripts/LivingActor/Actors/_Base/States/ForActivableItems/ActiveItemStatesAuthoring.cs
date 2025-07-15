using UnityEngine;

public class ActiveItemStatesAuthoring : MonoBehaviour {
    private class Baker : ExtendBaker<ActiveItemStatesAuthoring> {
        public override void Bake(ActiveItemStatesAuthoring authoring) {
            GetDynamicEntity(out var entity);

            // All states and state data
            AddComponent<ItemCommonStateData>(entity);

            AddComponentDisabled<ItemActiveAnalyzingState>(entity);
            AddComponentDisabled<Skill_Q_State>(entity);
            AddComponentDisabled<Skill_W_State>(entity);
            AddComponentDisabled<Skill_E_State>(entity);
            AddComponentDisabled<Skill_R_State>(entity);
            AddComponentDisabled<ItemCommonState>(entity);

            // Others
            AddComponentDisabled<ActiveItemWithoutState_Request>(entity);
            AddComponent<ItemActiveRequestData>(entity);
        }
    }
}