public class TooltipWindow_Simple : ITooltipWindow {
    public void UpdateText(string mainText) => _MainText.text = mainText;
}