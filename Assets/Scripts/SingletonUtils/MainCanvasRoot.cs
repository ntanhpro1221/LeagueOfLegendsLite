using NGDtuanh.Singleton;
using UnityEngine;

public class MainCanvasRoot : SceneSingleton<MainCanvasRoot> {
    public static Transform Value => Instance?.transform;
}