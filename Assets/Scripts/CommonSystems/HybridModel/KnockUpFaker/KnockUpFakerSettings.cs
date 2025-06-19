using NGDtuanh.Singleton;
using UnityEngine;

public class KnockUpFakerSettings : SceneSingleton<KnockUpFakerSettings> {
    [SerializeField] private float gravity;

    public static float Gravity     => Instance.gravity;
}