using Unity.Physics;

public static class PhysicsLayerHelpers {
    public const uint All           = ~0u;
    public const uint Default       = 1u << 0;
    public const uint TransparentFX = 1u << 1;
    public const uint IgnoreRaycast = 1u << 2;
    public const uint Water         = 1u << 4;
    public const uint UI            = 1u << 5;
    public const uint Ground        = 1u << 6;
    public const uint Actor         = 1u << 7;
    public const uint Wall          = 1u << 8;
    public const uint ActorDetector = 1u << 9;

    public static CollisionFilter GetFilter(uint layers) => new() {
        BelongsTo    = All
      , CollidesWith = layers
    };
}