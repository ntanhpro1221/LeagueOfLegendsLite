using System;
using Unity.Physics;

[Flags]
public enum LayerId : uint {
    All           = ~0u
  , Default       = 1u << 0
  , TransparentFX = 1u << 1
  , IgnoreRaycast = 1u << 2
  , Water         = 1u << 4
  , UI            = 1u << 5
  , Ground        = 1u << 6
  , Actor         = 1u << 7
  , Wall          = 1u << 8
  , ActorDetector = 1u << 9
}

public static class LayerHelpers {
    public static CollisionFilter ToFilter(this LayerId targetMask) => new() {
        BelongsTo    = (uint)LayerId.All
      , CollidesWith = (uint)targetMask
    };
}