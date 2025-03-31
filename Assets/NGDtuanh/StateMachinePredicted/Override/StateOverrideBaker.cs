using System;
using Unity.Entities;
using UnityEngine;

namespace NGDtuanh.Entities.StateMachine {
    public abstract class StateOverrideBaker<TAuthoring> : Baker<TAuthoring>
        where TAuthoring : Component, IStateOverrideAuthoring {
        public override void Bake(TAuthoring authoring) {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            foreach (StateOverrideAt overrideAt in Enum.GetValues(typeof(StateOverrideAt)))
                if (authoring.OverrideAts.HasFlag(overrideAt))
                    AddComponent(entity, authoring.GetOverrideTag(overrideAt));
        }
    }
}