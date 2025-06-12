using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EffectIconUI))]
public class HealthBarUI : MonoBehaviour {
    public struct UpdateData {
        public float maxHealth;
        public float curHealth;
        public float curArmor;
        public float maxMana;
        public float curMana;
        public int   curLevel;
        public float curExp;
        public float requiredExp;
        public bool  ignoreLostHealthEffect;
    }

    public void UpdateUI(in UpdateData updateData) {
        SetHealth(
            updateData.maxHealth
          , updateData.curHealth
          , updateData.curArmor
          , updateData.ignoreLostHealthEffect);
        SetMana(
            updateData.maxMana
          , updateData.curMana);
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

    private float prevMaxHealthArmorFill;

    private TweenerCore<float, float, FloatOptions> LostHealthTween;

    private void SetHealth(
        float maxHealth
      , float curHealth
      , float curArmor
      , bool  ignoreLostHealthEffect) {

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

        if (HealthText != null) HealthText.text = $"{(int)curHealth} / {(int)maxHealth}";

        prevMaxHealthArmorFill = curMaxHealthArmorFill;
    }

#endregion

#region MANA

    [Space]
    [Header("MANA")]
    [SerializeField] private Image Mana;

    [SerializeField] private TextMeshProUGUI ManaText;

    private void SetMana(float maxMana, float curMana) {
        if (Mana     != null) Mana.fillAmount = curMana / Mathf.Max(1, maxMana);
        if (ManaText != null) ManaText.text   = $"{(int)curMana} / {(int)maxMana}";
    }

#endregion

#region LEVEL

    [Space]
    [Header("LEVEL")]
    [SerializeField] private TextMeshProUGUI Level;

    [SerializeField] private Image Exp;
    [SerializeField] private float MinScale = 0;
    [SerializeField] private float MaxScale = 1;

    private void SetLevel(int level, float curExp, float requiredExp) {
        if (Level != null) Level.text     = level.ToString();
        if (Exp   != null) Exp.fillAmount = Mathf.Lerp(MinScale, MaxScale, curExp / requiredExp);
    }

#endregion

    private void OnDestroy() {
        LostHealthTween?.Kill();
    }
}