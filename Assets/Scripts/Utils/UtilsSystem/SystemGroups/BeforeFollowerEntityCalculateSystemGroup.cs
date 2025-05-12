using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation, WorldSystemFilterFlags.ServerSimulation)]
public partial class BeforeFollowerEntityCalculateSystemGroup :  ComponentSystemGroup { }