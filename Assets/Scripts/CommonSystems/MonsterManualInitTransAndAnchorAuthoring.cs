using Unity.Entities;
using UnityEngine;

public struct MonsterManualInitTransAndAnchorTag : IComponentData { }

public class MonsterManualInitTransAndAnchorAuthoring : MonoBehaviour {
    private class Baker : TagBaker<MonsterManualInitTransAndAnchorAuthoring, MonsterManualInitTransAndAnchorTag> { }
}