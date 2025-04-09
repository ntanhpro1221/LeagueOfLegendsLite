using UnityEngine;

public static class Texture2DExtensions {
    public static Vector2 Size(this Texture2D tex) => new(tex.width, tex.height);
}