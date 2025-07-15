using Unity.Entities;

[UpdateInGroup(typeof(HandleEffectSystemGroup), OrderLast = true)]
public partial class AfterHandleEffectSystemGroup : ComponentSystemGroup { }