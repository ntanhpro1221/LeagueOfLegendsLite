using Unity.Entities;
using Unity.NetCode;

namespace NGDtuanh.Entities.StateMachine {
    [GhostEnabledBit]
    public struct StateNeedEnterTag : IComponentData, IEnableableComponent { }
}