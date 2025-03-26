public static class PhysicsLayerHelper {
    public const uint Default       = 1u << 0;
    public const uint TransparentFX = 1u << 1;
    public const uint IgnoreRaycast = 1u << 2;
    public const uint Water         = 1u << 4;
    public const uint UI            = 1u << 5;
    public const uint Ground        = 1u << 6;
    public const uint GroundRay     = 1u << 7;
    public const uint Actor         = 1u << 8;
    public const uint Wall          = 1u << 9;
    public const uint ActorRay      = 1u << 10;
}