using NGDtuanh.Singleton;
using UnityEngine;

public class RotationConfig : SceneSingleton<RotationConfig> {
    /// <summary>
    /// Degree per second
    /// </summary>
    [Tooltip("Degree per second")]
    public float speed = 777;
}