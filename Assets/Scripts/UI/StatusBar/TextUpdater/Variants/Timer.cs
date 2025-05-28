using TMPro;
using Unity.NetCode;

public static partial class TextUpdater {
    public struct Timer : ITextUpdater {
        private static readonly NetworkTick _ZeroTick = new(0);

        public NetworkTick curTick;
        public int         tickRate;

        public readonly void Update(TextMeshProUGUI target) {
            // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
            int seconds     = curTick.TicksSince(_ZeroTick) / (tickRate <= 0 ? 60 : tickRate);
            int trueSeconds = seconds                       % 60;

            target.text = $"{seconds / 60}:{(trueSeconds < 10 ? "0" : "")}{trueSeconds}";
        }
    }
}