using Unity.Entities;

[UpdateInGroup(typeof(UpdateItemActiveRequestSystemGroup))]
[UpdateAfter(typeof(UpdateItemActiveRequestSystem))]
public partial class ActiveItemWithoutStateSystemGroup : ComponentSystemGroup { }