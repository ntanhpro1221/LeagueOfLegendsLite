using System;

namespace NGDtuanh.Entities.StateMachine {
    public interface IStateOverrideAuthoring {
        StateOverrideAt OverrideAts { get; }
        Type            GetOverrideTag(StateOverrideAt overrideAt);
    }
}