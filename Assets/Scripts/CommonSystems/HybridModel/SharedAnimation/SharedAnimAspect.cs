using Unity.Entities;

public readonly partial struct SharedAnimAspect : IAspect {
    private readonly RefRW<SharedAnimData> _AnimData;

    public void SetAnim(SharedAnimKey newAnim) => _AnimData.ValueRW.curAnim = newAnim;
}