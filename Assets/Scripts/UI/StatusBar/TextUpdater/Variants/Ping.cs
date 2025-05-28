using TMPro;

public static partial class TextUpdater {
    public struct Ping : ITextUpdater {
        /// <summary>
        /// in milliseconds
        /// </summary>
        public float rtt;

        public readonly void Update(TextMeshProUGUI target) =>
            target.text = $"{(int)rtt} ms";
    }
}