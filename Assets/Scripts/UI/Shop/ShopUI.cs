using System.Collections.Generic;
using NGDtuanh.Collections;
using NGDtuanh.Singleton;
using NGDtuanh.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : SceneSingleton<ShopUI>, IFilteredDropHandler<ItemUI> {
    [SerializeField] private GameObject _Holder;

    public bool Visible {
        get => _Holder.activeSelf;
        set => _Holder.SetActive(value);
    }

    [Header("------ITEM LISTS------")]
    [SerializeField] private Transform _ItemHolder;

    [SerializeField] private ItemUIShop _ItemPrefab;

    [Header("------SELL HOVER------")]
    [SerializeField] private GameObject _SellHoverToggler;

    [SerializeField] private TextMeshProUGUI _SellHoverSell;
    [SerializeField] private Image           _SellHoverTrash;
    [SerializeField] private Sprite          _SellHoverTrashNormal;
    [SerializeField] private Sprite          _SellHoverTrashHighlighted;

    [field: Space]
    [field: SerializeField] public ShopInspectorUI Inspector { get; private set; }

    [SerializeField] private TextMeshProUGUI _CoinText;

    public readonly Bindable<float_Q3> Coin = new((in float_Q3 oldVal, in float_Q3 newVal) => {
        Instance._CoinText.text = $"<sprite name=coin> {newVal:int}";
    });

    public readonly Bindable<Strum.Items.Fields<bool>> Buyable = new((in Strum.Items.Fields<bool> oldVal, in Strum.Items.Fields<bool> newVal) => {
        foreach (var itemUI in Instance._CachedItemUIs)
            itemUI.UpdateState(newVal[itemUI.CurItem]);
    });

    #region ITEM LIST

    private readonly List<ItemUIShop> _CachedItemUIs = new();

    private void InitItemList() {
        _CachedItemUIs.Capacity = GameSO.Item.Count;
        foreach (var (itemId, itemData) in GameSO.Item) {
            // Instantiate itemUI
            var itemUI = Instantiate(_ItemPrefab, _ItemHolder).GetComponent<ItemUIShop>();

            // Init itemUI
            itemUI.InitAll(itemId, itemData);

            // Add to list
            _CachedItemUIs.Add(itemUI);
        }
    }

    #endregion

    private void InitToggleShopBtn() {
        LazyInput_Battle.Input.InGame.ToggleShop.performed += _ => _Holder.FlipActiveSelf();
    }

    protected override void Awake() {
        base.Awake();

        InitItemList();
        InitToggleShopBtn();

        Coin.ForceAssignAndUpdate(Coin.Value);
        Buyable.ForceAssignAndUpdate(Buyable.Value);
    }

    public void OnItemDrop(ItemUI item) {
        PlayerRequestHub.Instance.SetSellItemAt(item.MySlot);

        if (Inspector.SelectedItemUI.Equals(item)) {
            Inspector.SelectedSlot.Value   = null;
            Inspector.SelectedItemUI.Value = null;
        }
    }

    public void OnItemEnter(ItemUI item) => _SellHoverTrash.sprite = _SellHoverTrashHighlighted;

    public void OnItemExit(ItemUI item) => _SellHoverTrash.sprite = _SellHoverTrashNormal;

    public void OnItemBeginDrag(ItemUI item) {
        _SellHoverToggler.gameObject.SetActive(true);
        _SellHoverSell.text = $"Sell For: {GameSO.Item[item.CurItem].settings.sell:int}";
    }

    public void OnItemEndDrag(ItemUI item) {
        _SellHoverToggler.gameObject.SetActive(false);
        _SellHoverTrash.sprite = _SellHoverTrashNormal;
    }
}