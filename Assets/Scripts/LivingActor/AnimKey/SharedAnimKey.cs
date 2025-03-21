using System;

public enum SharedAnimKey {
    Attack = 0
  , Dead   = 1
  , Idle   = 2
  , Move   = 3
}

public static class SharedAnimKeyExtensions {
    public static class Names {
        public const string Attack = "Attack";
        public const string Dead   = "Dead";
        public const string Idle   = "Idle";
        public const string Move   = "Move";
    }

    public static string ToAnimName(this SharedAnimKey key) => key switch {
        SharedAnimKey.Attack => Names.Attack
      , SharedAnimKey.Dead   => Names.Dead
      , SharedAnimKey.Idle   => Names.Idle
      , SharedAnimKey.Move   => Names.Move
      , _                    => throw new Exception("Unknown anim key")
    };
}