using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour {
    public void UpdateUI(
        float maxHealth
      , float curHealth
      , float curArmor
      , float maxMana
      , float curMana
      , int   curLevel
      , float curExp                 = 5
      , float requiredExp            = 10
      , bool  ignoreLostHealthEffect = false) {
        SetHealth(maxHealth, curHealth, curArmor, ignoreLostHealthEffect);
        SetMana(maxMana, curMana);
        SetLevel(curLevel, curExp, requiredExp);
    }

    private void Awake() {
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

    private TweenerCore<float, float, FloatOptions> LostHealthTween;

    private void SetHealth(
        float maxHealth
      , float curHealth
      , float curArmor
      , bool  ignoreLostHealthEffect) {

        float maxHealthWithArmor = Mathf.Max(maxHealth, curHealth + curArmor);
        float prevArmorFill      = Armor.fillAmount;

        if (Health != null) Health.fillAmount = curHealth              / maxHealthWithArmor;
        if (Armor  != null) Armor.fillAmount  = (curHealth + curArmor) / maxHealthWithArmor;

        if (ignoreLostHealthEffect) {
            if (LostHealthTween.IsActive()) LostHealthTween.Complete();
            if (LostHealth != null) LostHealth.fillAmount = Armor.fillAmount;
        } else if (Mathf.Abs(prevArmorFill - Armor.fillAmount) > Mathf.Epsilon)
            LostHealthTween.ChangeValues(prevArmorFill, Armor.fillAmount).Restart();

        if (HealthText != null) HealthText.text = $"{(int)curHealth} / {(int)maxHealth}";
    }

#endregion

#region MANA

    [Space]
    [Header("MANA")]
    [SerializeField] private Image Mana;

    [SerializeField] private TextMeshProUGUI ManaText;

    private void SetMana(float maxMana, float curMana) {
        if (Mana     != null) Mana.fillAmount = curMana / maxMana;
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
        LostHealthTween.Kill();
    }
}