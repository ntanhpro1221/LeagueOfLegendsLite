using Unity.Entities;

[UpdateInGroup(typeof(UpdateItemActiveRequestSystemGroup))]
[UpdateBefore(typeof(UpdateItemActiveRequestSystem))]
public partial class UpdateItemSpecialCondSystemGroup : ComponentSystemGroup { }