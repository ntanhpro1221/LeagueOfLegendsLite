using Unity.Entities;

/// <summary>
/// Used for custom normal attack behaviour
/// </summary>
[UpdateInGroup(typeof(HandleNormalAttackSystemGroup), OrderFirst = true)]
[UpdateAfter(typeof(NormalAttackSystemGroup))] // Just to ensure
public partial class BeforeNormalAttackSystemGroup : ComponentSystemGroup { }