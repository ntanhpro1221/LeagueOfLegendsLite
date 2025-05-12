using Pathfinding;

public class PathHolderForWaypoint {
    private static readonly PathHolderForWaypoint Instance = new();

    public static void Claim(Path   path) => path.Claim(Instance);
    public static void Release(Path path) => path.Release(Instance);
}