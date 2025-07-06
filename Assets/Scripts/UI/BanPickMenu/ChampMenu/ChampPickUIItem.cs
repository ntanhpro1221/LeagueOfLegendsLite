using UnityEngine;
using UnityEngine.UI;

public class ChampPickUIItem : MonoBehaviour, ISelectable {
    [SerializeField] private Image _Avatar;
    [SerializeField] private Image _SelectedBorder;

    private ChampionId _CurChamp;

    public void InitAll(ChampionId champId) {
        _CurChamp      = champId;
        _Avatar.sprite = GameSO.Champ[champId].avatar;
    }

    public void OnClick() {
        var champMenu = BanPickMenuUI.Instance.ChampMenu;
        champMenu.SelectedChampion.Value   = _CurChamp;
        champMenu.SelectedChampionUI.Value = this;
    }

    public void Select() => _SelectedBorder.enabled = true;

    public void Deselect() => _SelectedBorder.enabled = false;
}