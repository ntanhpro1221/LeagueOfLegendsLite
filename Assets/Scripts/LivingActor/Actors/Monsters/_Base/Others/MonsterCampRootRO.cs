using Unity.Entities;

public readonly partial struct MonsterCampRootRO : IAspect {
    #pragma warning disable CS0414 // Field is assigned but its value is never used
    private readonly RefRO<Simulate> _Simulate;
    #pragma warning restore CS0414 // Field is assigned but its value is never used

    public readonly Entity MyEntity;

    [Optional] private readonly RefRO<MonsterUnderlingData> _UnderlingData;
    [Optional] private readonly RefRO<MonsterLeaderData>    _LeaderData;

    /// <summary>
    /// Just use it when you know exactly this entity is leader or underling 
    /// </summary>
    public Entity RootUnsafe => _UnderlingData.IsValid
        ? _UnderlingData.ValueRO.leader
        : MyEntity;

    public bool TryGetRoot(out Entity root) {
        if (_UnderlingData.IsValid) {
            root = _UnderlingData.ValueRO.leader;
            return true;
        }

        if (_LeaderData.IsValid) {
            root = MyEntity;
            return true;
        }

        root = Entity.Null;
        return false;
    }
}