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
      , bool  ignoreLostHealthEffect = false) {
        SetHealth(maxHealth, curHealth, curArmor, ignoreLostHealthEffect);
        SetMana(maxMana, curMana);
        SetLevel(curLevel);
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
    [SerializeField] private Image Armor;
    [SerializeField] private Image LostHealth;
    [SerializeField] private float LostHealthDuration;

    private TweenerCore<float, float, FloatOptions> LostHealthTween;

    private void SetHealth(
        float maxHealth
      , float curHealth
      , float curArmor
      , bool  ignoreLostHealthEffect) {

        float maxHealthWithArmor = Mathf.Max(maxHealth, curHealth + curArmor);
        float prevArmorFill      = Armor.fillAmount;
        Health.fillAmount = curHealth              / maxHealthWithArmor;
        Armor.fillAmount  = (curHealth + curArmor) / maxHealthWithArmor;

        if (ignoreLostHealthEffect) {
            if (LostHealthTween.IsActive()) LostHealthTween.Complete();
            LostHealth.fillAmount = Armor.fillAmount;
        }
        else if (Mathf.Abs(prevArmorFill - Armor.fillAmount) > Mathf.Epsilon)
            LostHealthTween.ChangeValues(prevArmorFill, Armor.fillAmount).Restart();
    }

    #endregion

    #region MANA
    
    [Space]
    [Header("MANA")]
    [SerializeField] private Image Mana;

    private void SetMana(float maxMana, float curMana) 
        => Mana.fillAmount = curMana / maxMana;
    
    #endregion

    #region LEVEL

    [Space]
    [Header("LEVEL")]
    [SerializeField] private TextMeshProUGUI Level;

    private void SetLevel(int level)
        => Level.text = level.ToString();
    
    #endregion
}