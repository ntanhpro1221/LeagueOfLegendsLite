using System;

public struct PathId {
    public float3_Q3 start;
    public float3_Q3 end;
    public int       code;

    public PathId(float3_Q3 start, float3_Q3 end) => code = HashCode.Combine(
        (this.start = start).GetHashCode()
      , (this.end = end).GetHashCode());
}