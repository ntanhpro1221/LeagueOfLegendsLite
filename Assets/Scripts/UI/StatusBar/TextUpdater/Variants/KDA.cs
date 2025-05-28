using TMPro;

public static partial class TextUpdater {
    public struct KDA : ITextUpdater {
        public int kill;
        public int dead;
        public int assist;

        public readonly void Update(TextMeshProUGUI target) =>
            target.text = $"{kill}/{dead}/{assist}";
    }
}