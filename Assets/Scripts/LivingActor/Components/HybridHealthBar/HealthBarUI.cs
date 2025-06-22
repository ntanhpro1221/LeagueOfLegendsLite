using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour {
    public struct UpdateData {
        public float_Q3 maxHealth;
        public float_Q3 curHealth;
        public float_Q3 healthRegen;
        public float_Q3 curArmor;
        public float_Q3 maxMana;
        public float_Q3 curMana;
        public float_Q3 manaRegen;
        public int      curLevel;
        public float_Q3 curExp;
        public float_Q3 requiredExp;
        public bool     ignoreLostHealthEffect;
    }

    public void UpdateUI(in UpdateData updateData) {
        SetHealth(
            maxHealth: updateData.maxHealth
          , curHealth: updateData.curHealth
          , healthRegen: updateData.healthRegen
          , curArmor: updateData.curArmor
          , ignoreLostHealthEffect: updateData.ignoreLostHealthEffect);
        SetMana(
            maxMana: updateData.maxMana
          , curMana: updateData.curMana
          , manaRegen: updateData.manaRegen);
        SetLevel(
            updateData.curLevel
          , updateData.curExp
          , updateData.requiredExp);
    }

    private void Awake() {
        if (LostHealth != null)
            LostHealthTween = LostHealth
                .DOFillAmount(Armor.fillAmount, LostHealthDuration)
                .SetEase(Ease.InCubic)
                .SetAutoKill(false);
    }

#region HEALTH

    [Header("HEALTH")]
    [SerializeField] private Image Health;

    [SerializeField] private Image           Armor;
    [SerializeField] private Image           LostHealth;
    [SerializeField] private float           LostHealthDuration;
    [SerializeField] private TextMeshProUGUI HealthText;
    [SerializeField] private TextMeshProUGUI HealthRegenText;

    private float prevMaxHealthArmorFill;

    private TweenerCore<float, float, FloatOptions> LostHealthTween;

    private void SetHealth(
        float_Q3 maxHealth
      , float_Q3 curHealth
      , float_Q3 healthRegen
      , float_Q3 curArmor
      , bool     ignoreLostHealthEffect) {

        float maxHealthWithArmor    = Mathf.Max(maxHealth, curHealth + curArmor);
        float curMaxHealthArmorFill = 0;

        if (Health != null) curMaxHealthArmorFill = Health.fillAmount = curHealth              / maxHealthWithArmor;
        if (Armor  != null) curMaxHealthArmorFill = Armor.fillAmount  = (curHealth + curArmor) / maxHealthWithArmor;

        if (LostHealthTween != null) {
            if (ignoreLostHealthEffect) {
                if (LostHealthTween.IsActive()) LostHealthTween.Complete();
                // No need to check null because losthealthtween != null
                LostHealth.fillAmount = curMaxHealthArmorFill;
            } else if (Mathf.Abs(prevMaxHealthArmorFill - curMaxHealthArmorFill) > Mathf.Epsilon)
                LostHealthTween.ChangeValues(prevMaxHealthArmorFill, curMaxHealthArmorFill).Restart();
        }

        if (HealthText      != null) HealthText.text      = $"{curHealth:int} / {maxHealth:int}";
        if (HealthRegenText != null) HealthRegenText.text = $"+{healthRegen:int}";

        prevMaxHealthArmorFill = curMaxHealthArmorFill;
    }

#endregion

#region MANA

    [Space]
    [Header("MANA")]
    [SerializeField] private Image Mana;

    [SerializeField] private TextMeshProUGUI ManaText;
    [SerializeField] private TextMeshProUGUI ManaRegenText;

    private void SetMana(
        float_Q3 maxMana
      , float_Q3 curMana
      , float_Q3 manaRegen) {
        if (Mana          != null) Mana.fillAmount    = curMana / Mathf.Max(1, maxMana);
        if (ManaText      != null) ManaText.text      = $"{curMana:int} / {maxMana:int}";
        if (ManaRegenText != null) ManaRegenText.text = $"+{manaRegen:int}";
    }

#endregion

#region LEVEL

    [Space]
    [Header("LEVEL")]
    [SerializeField] private TextMeshProUGUI Level;

    [SerializeField] private Image Exp;
    [SerializeField] private float MinScale = 0;
    [SerializeField] private float MaxScale = 1;

    private void SetLevel(
        int      level
      , float_Q3 curExp
      , float_Q3 requiredExp) {
        if (Level != null) Level.text     = level.ToString();
        if (Exp   != null) Exp.fillAmount = Mathf.Lerp(MinScale, MaxScale, curExp / requiredExp);
    }

#endregion

    private void OnDestroy() {
        LostHealthTween?.Kill();
    }
}