using Unity.Entities;

public readonly partial struct HealthAspectRO : IAspect {
    private readonly RefRO<HealthData> _HealthData;

    public float_Q3 CurHealth => _HealthData.ValueRO.value;

    public bool IsDead => _HealthData.ValueRO.value <= 0;
}