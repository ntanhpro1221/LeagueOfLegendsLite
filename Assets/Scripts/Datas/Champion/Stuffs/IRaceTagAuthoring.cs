using UnityEngine;

public class IRaceTagAuthoring : MonoBehaviour, IRaceTag {
    public int    TagInt => 0;
    public RaceId Race   => default;
}