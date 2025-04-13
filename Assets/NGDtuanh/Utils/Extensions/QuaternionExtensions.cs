using UnityEngine;

public static class QuaternionExtensions {
    public static Vector4 Value(this Quaternion quat) => new Vector4(
        quat.x
      , quat.y
      , quat.z
      , quat.w);
}