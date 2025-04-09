using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public readonly partial struct VelocityAspectRO : IAspect {
    private const float MOVE_TOLERANCE = 0.01f;

    private readonly RefRO<PhysicsVelocity> _PhysicsVelocity;

    private float SumVelocityXZ =>
        math.abs(_PhysicsVelocity.ValueRO.Linear.x)
      + math.abs(_PhysicsVelocity.ValueRO.Linear.z);

    public bool IsMoving => SumVelocityXZ > MOVE_TOLERANCE;
}