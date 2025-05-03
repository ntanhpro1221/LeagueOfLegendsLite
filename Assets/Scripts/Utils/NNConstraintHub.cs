using Pathfinding;
using Unity.Mathematics;

public static class NNConstraintHub {
    public static readonly NNConstraint ClosestAsSeenFromAbove = new() {
        distanceMetric = DistanceMetric.ClosestAsSeenFromAbove(math.up())
    };
}
