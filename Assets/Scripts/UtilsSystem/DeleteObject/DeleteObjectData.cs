using NGDtuanh.Singleton;
using UnityEngine;

/// <summary>
/// <see cref="targets"/> will be deleted by <see cref="DeleteObjectSystem"/> if there is a <see cref="DeleteObjectRequest"/>
/// </summary>
public class DeleteObjectData : SceneSingleton<DeleteObjectData> {
    public GameObject[] targets;
}