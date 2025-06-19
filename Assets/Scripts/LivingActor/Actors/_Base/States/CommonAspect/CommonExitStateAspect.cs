using Unity.Entities;

public readonly partial struct CommonExitStateAspect : IAspect {
    public readonly ActorSharedStateAspect State;
    public readonly HealthAspectRO         Health;
    public readonly AimedTargetAspectRO    Target;
    public readonly CCAspectRO             CC;
}