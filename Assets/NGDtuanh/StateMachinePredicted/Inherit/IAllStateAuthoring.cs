using System;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;

namespace NGDtuanh.Entities.StateMachine {
    public abstract class IAllStateAuthoring<TStateType> : MonoBehaviour
        where TStateType : struct, Enum {
        [SerializeField] private EnumMap<TStateType, StateStep> _OverrideState;

        private IStateInheritTag GetInheritTag(TStateType state, StateStep inheritAt, out IStateInheritTag result)
            => result = GetInheritTag(state, inheritAt);

        protected abstract class InheritTagBaker<TAuthoring> : Baker<TAuthoring>
            where TAuthoring : IAllStateAuthoring<TStateType> {
            public override void Bake(TAuthoring authoring) {
                var allStateSteps = Enum.GetValues(typeof(StateStep));

                var entity = GetEntity(TransformUsageFlags.Dynamic);
                foreach (var (state, overrides) in authoring._OverrideState)
                foreach (StateStep step in allStateSteps)
                    if (true != overrides.HasFlag(step)
                     && null != authoring.GetInheritTag(state, step, out var inheritTag))
                        AddComponent(entity, inheritTag.GetType());
                
                MoreBake(authoring);
            }
            
            public abstract void MoreBake(TAuthoring authoring);
        }

        protected abstract IStateInheritTag GetInheritTag(TStateType state, StateStep inheritAt);
    }
}