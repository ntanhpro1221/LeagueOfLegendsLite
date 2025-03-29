using System;

public static class SharedAnimKeyExtensions {
    public const string StateVarPrefix = "Is_";
    public const string SpeedVarPrefix = "Speed_";

    public static string KeyName(this SharedAnimKey key) => key switch {
        SharedAnimKey.Attack => nameof(SharedAnimKey.Attack)
      , SharedAnimKey.Dead   => nameof(SharedAnimKey.Dead)
      , SharedAnimKey.Idle   => nameof(SharedAnimKey.Idle)
      , SharedAnimKey.Move   => nameof(SharedAnimKey.Move)
      , _                    => throw new UnknownAnimKeyException()
    };

    public static string StateVarName(this SharedAnimKey key)
        => StateVarPrefix + key.KeyName();

    public static string SpeedVarName(this SharedAnimKey key)
        => SpeedVarPrefix + key.KeyName();

    public class UnknownAnimKeyException : Exception {
        public UnknownAnimKeyException() : base("Unknown anim key") { }
    }
}