using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class ChampPickMenuUI : MonoBehaviour {
    [Header("----------LOCKED CHAMP---------")]
    [SerializeField] private GameObject _LockedChampObject;

    [SerializeField] private Image _LockedChampImage;

    [Header("----------CHAMPION LIST--------")]
    [SerializeField] private GameObject _ChampListObject;

    [SerializeField] private Transform       _ItemHolder;
    [SerializeField] private ChampPickUIItem _ItemPrefab;

    [Header("----------BUTTONS-------------")]
    [SerializeField] private ChampMenuButtons _Buttons;

    public readonly Bindable<ChampPickUIItem> SelectedChampionUI = new((in ChampPickUIItem oldVal, in ChampPickUIItem newVal) => {
        oldVal?.Deselect();
        newVal?.Select();
    });

    public readonly Bindable<ChampionId?> SelectedChampion = new((in ChampionId? oldVal, in ChampionId? newVal) => {
        var menu = BanPickMenuUI.Instance.ChampMenu;
        if (menu._ChampListObject.activeSelf) {
            menu._Buttons.UpdateState(newVal == null
                ? ChampMenuButtons.State.NotSelectedAnything
                : ChampMenuButtons.State.Selected);
        }
    });

    public readonly Bindable<ChampionId?> LockedChampion = new((in ChampionId? oldVal, in ChampionId? newVal) => {
        Debug.Log($"Locked champion {newVal.ToString()}");
    });

    private void InitChampList() {
        foreach (var champId in GameSO.Champ.Keys) Instantiate(_ItemPrefab, _ItemHolder).InitAll(champId);
    }

    private void Awake() {
        InitChampList();
    }

    public void OnClick_LockChamp() {
        if (SelectedChampion.Value == null) {
            Debug.LogError("NGDtuanh ChampPickMenuUI: somehow lock champ button was clicked when there is no champion selected!");
            return;
        }

        _LockedChampObject.SetActive(true);
        _ChampListObject.SetActive(false);

        var champ = SelectedChampion.Value.Value;
        _LockedChampImage.sprite = GameSO.Champ[champ].avatar;
        _Buttons.UpdateState(ChampMenuButtons.State.Locked);

        World.DefaultGameObjectInjectionWorld.EntityManager.SendRpc(new LockChampRpc { champId = champ });
    }

    public void OnClick_SelectAnotherChamp() {
        _LockedChampObject.SetActive(false);
        _ChampListObject.SetActive(true);

        // To reset button state UI
        _Buttons.UpdateState(SelectedChampion.Value == null
            ? ChampMenuButtons.State.NotSelectedAnything
            : ChampMenuButtons.State.Selected);
    }
}