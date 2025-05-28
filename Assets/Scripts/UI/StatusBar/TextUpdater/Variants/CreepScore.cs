using TMPro;
using Unity.NetCode;

public static partial class TextUpdater {
    public struct CreepScore : ITextUpdater {
        public int creepScore;

        public readonly void Update(TextMeshProUGUI target) =>
            target.text = creepScore.ToString();
    }
}