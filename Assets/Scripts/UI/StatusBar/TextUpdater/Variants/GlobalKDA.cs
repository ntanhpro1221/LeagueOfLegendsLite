using TMPro;

public static partial class TextUpdater {
    public struct GlobalKDA : ITextUpdater {
        public int kill;
        public int dead;

        public readonly void Update(TextMeshProUGUI target) =>
            target.text = $"<color=#5baef3>{kill}</color> vs <color=#f25437>{dead}</color>";
    }
}