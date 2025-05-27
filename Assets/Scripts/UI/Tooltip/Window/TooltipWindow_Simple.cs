public class TooltipWindow_Simple : ITooltipWindow {
    public void Init(string mainText) => _MainText.text = mainText;
}