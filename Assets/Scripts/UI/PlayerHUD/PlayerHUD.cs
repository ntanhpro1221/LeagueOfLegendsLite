using NGDtuanh.Singleton;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(
    typeof(EffectBarUI)
  , typeof(DeadHandler_OwnChamp)
  , typeof(PlayerActivableItemUI))]
[RequireComponent(
    typeof(StatsUI))]
public class PlayerHUD : SceneSingleton<PlayerHUD> {
    [SerializeField]        private Button          _ShopBtn;
    [field: SerializeField] public  TextMeshProUGUI GoldText   { get; private set; }
    [field: SerializeField] public  Tooltip_Simple  ExpTooltip { get; private set; }

    public StatsUI               Stats          { get; private set; }
    public EffectBarUI           EffectBarUI    { get; private set; }
    public PlayerActivableItemUI ActivableItems { get; private set; }
    public DeadHandler_OwnChamp  DeadHandler    { get; private set; }
    public HealthBarUI           HealthBar      { get; private set; }

    protected override void OnTouched() {
        base.OnTouched();

        Stats          = GetComponent<StatsUI>();
        EffectBarUI    = GetComponent<EffectBarUI>();
        ActivableItems = GetComponent<PlayerActivableItemUI>();
        DeadHandler    = GetComponent<DeadHandler_OwnChamp>();
        HealthBar      = GetComponentInChildren<HealthBarUI>();
    }

    protected override void Awake() {
        base.Awake();

        _ShopBtn.onClick.AddListener(() => ShopUI.Instance.Visible = true);
    }

    public void UpdateGold(int gold) {
        GoldText.text = $"{gold.ToString()}";
    }

    public void UpdateExp(in LevelData level, in RequireExpData requireExp) {
        ExpTooltip.Window.UpdateText($"Level: {level.curLevel} | Exp: {level.curExp}/{requireExp.CalcRequireExpForNextLevel(level.curLevel)}");
    }
}