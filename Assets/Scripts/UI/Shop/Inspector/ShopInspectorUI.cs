using System;
using System.Collections.Generic;
using NGDtuanh.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopInspectorUI : MonoBehaviour {
    [SerializeField] private ItemBuildIntoHandler _BuildInto;
    [SerializeField] private RecipeVisualizer     _Recipe;
    [SerializeField] private Description          _Description;
    [SerializeField] private List<GameObject>     _DisableComponentWhenNoItem;

    public readonly Bindable<ItemId?> SelectedItem = new(static (in ItemId? oldVal, in ItemId? newVal) => {
        var inspector = ShopUI.Instance.Inspector;
        
        bool active = newVal != null;
        
        foreach (var item in inspector._DisableComponentWhenNoItem)
            item.SetActive(active);

        if (active) {
            inspector._BuildInto.ShowBuildIntoFor(newVal.Value);
            inspector._Description.ShowDescriptionFor(newVal.Value);
        }
    });

    public readonly Bindable<SlotItemId?> SelectedSlot = new((in SlotItemId? oldVal, in SlotItemId? newVal) => {
        // Debug.Log("Slot changed");
    });

    public readonly Bindable<ISelectable> SelectedItemUI = new(static (in ISelectable oldVal, in ISelectable newVal) => {
        oldVal?.Deselect();
        newVal?.Select();
        // Debug.Log("UI changed");
    });

    private void Start() {
        SelectedItem.ForceAssignAndUpdate(null);
        SelectedSlot.ForceAssignAndUpdate(null);
        SelectedItemUI.ForceAssignAndUpdate(null);
    }

    public void InspectItem(ItemId itemId, ISelectable itemUI, bool updateRecipe, SlotItemId? slotId = null) {
        if (SelectedItem.ChangeValue(itemId) && updateRecipe)
            _Recipe.ShowRecipeFor(itemId);

        SelectedItem.Value   = itemId;
        SelectedItemUI.Value = itemUI;
        SelectedSlot.Value   = slotId;
    }

    [Serializable]
    public class Description {
        [SerializeField] private Image           _Avatar;
        [SerializeField] private TextMeshProUGUI _NameCost;
        [SerializeField] private TextMeshProUGUI _Description;

        public void ShowDescriptionFor(ItemId itemId) {
            var data = GameSO.Item[itemId];
            _Avatar.sprite    = data.common.avatar;
            _NameCost.text    = $"{data.common.itemName}\n<sprite name=coin> {data.settings.cost:int}";
            _Description.text = $"{data.common.description}\n\n{data.common.details}";
        }
    }
}