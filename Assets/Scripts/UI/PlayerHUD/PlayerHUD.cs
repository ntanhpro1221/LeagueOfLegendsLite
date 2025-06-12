using NGDtuanh.Singleton;
using TMPro;
using UnityEngine;

[RequireComponent(
    typeof(EffectBarUI)
  , typeof(DeadHandler_OwnChamp)
  , typeof(PlayerActivableItemUI))]
public class PlayerHUD : SceneSingleton<PlayerHUD> {
    [field: SerializeField] public StatsUI         Stats      { get; private set; }
    [field: SerializeField] public TextMeshProUGUI GoldText   { get; private set; }
    [field: SerializeField] public Tooltip_Simple  ExpTooltip { get; private set; }

    public EffectBarUI           EffectBarUI    { get; private set; }
    public PlayerActivableItemUI ActivableItems { get; private set; }
    public DeadHandler_OwnChamp  DeadHandler    { get; private set; }
    public HealthBarUI           HealthBar      { get; private set; }

    protected override void OnTouched() {
        base.OnTouched();

        EffectBarUI    = GetComponent<EffectBarUI>();
        ActivableItems = GetComponent<PlayerActivableItemUI>();
        DeadHandler    = GetComponent<DeadHandler_OwnChamp>();
        HealthBar      = GetComponentInChildren<HealthBarUI>();
    }

    public void UpdateGold(int gold) {
        GoldText.text = $"<sprite name=coin>  {gold.ToString()}";
    }

    public void UpdateExp(in LevelData level, in RequireExpData requireExp) {
        ExpTooltip.Window.UpdateText($"Level: {level.curLevel} | Exp: {level.curExp}/{requireExp.CalcRequireExpForNextLevel(level.curLevel)}");
    }
}