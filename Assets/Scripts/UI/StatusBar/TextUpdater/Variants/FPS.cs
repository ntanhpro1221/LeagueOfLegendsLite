using TMPro;

public static partial class TextUpdater {
    public struct FPS : ITextUpdater {
        public float deltaTime;

        public readonly void Update(TextMeshProUGUI target) =>
            target.text = $"FPS: {(int)(1 / deltaTime)}";
    }
}