using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace NGDtuanh.Entities.StateMachine {
    /// <summary>
    /// 1. Query for <see cref="StateNeedEnterTag"/> and perform state enter logic.<br/>
    /// 2. Disable <see cref="StateNeedEnterTag"/>.<br/>
    /// (Run after <see cref="StateExitSystemGroup"/>)<br/>
    /// </summary>
    [UpdateInGroup(typeof(StateMachineSystemGroup))]
    [UpdateAfter(typeof(StateExitSystemGroup))]
    public partial class StateEnterSystemGroup : ComponentSystemGroup { }
}
