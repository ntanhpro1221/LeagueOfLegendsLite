using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemBuildIntoHandler : MonoBehaviour {
    [SerializeField] private Transform       _ItemUIRoot;
    [SerializeField] private ItemUIBuildInto _ItemPrefab;

    #region ITEM UI POOL

    private Stack<ItemUIBuildInto> _AvailableItemUI = new Stack<ItemUIBuildInto>();
    private Stack<ItemUIBuildInto> _UsedItemUI      = new Stack<ItemUIBuildInto>();

    private ItemUIBuildInto GetItemUI() {
        if (_AvailableItemUI.Count == 0) _AvailableItemUI.Push(Instantiate(_ItemPrefab, _ItemUIRoot));

        var result = _AvailableItemUI.Pop();
        result.gameObject.SetActive(true);
        _UsedItemUI.Push(result);
        return result;
    }

    private void ReleaseAllItemUI() {
        while (_UsedItemUI.Count > 0) {
            var item = _UsedItemUI.Pop();
            item.gameObject.SetActive(false);
            _AvailableItemUI.Push(item);
        }
    }

    #endregion

    #region BUILD INTO TABLE

    private Dictionary<ItemId, List<ItemId>> _BuildIntoTable;

    private void InitBuildIntoTable() {
        var allItem = GameSO.Item;

        var builder = new Dictionary<ItemId, HashSet<ItemId>>(capacity: allItem.Count);
        foreach (var id in allItem.Keys)
            builder[id] = new HashSet<ItemId>();
        var stack = new Stack<ItemId>();
        foreach (var id in allItem.Keys) {
            stack.Push(id);
            while (stack.Count > 0)
                foreach (var child in allItem[stack.Pop()].recipe.Where(child => !builder[child].Contains(id))) {
                    builder[child].Add(id);
                    stack.Push(child);
                }
        }

        _BuildIntoTable = new Dictionary<ItemId, List<ItemId>>(capacity: allItem.Count);
        foreach (var id in allItem.Keys) _BuildIntoTable.Add(id, new List<ItemId>(builder[id]));
    }

    #endregion

    private void Awake() {
        InitBuildIntoTable();
    }

    public void ShowBuildIntoFor(ItemId itemId) {
        ReleaseAllItemUI();
        foreach (var buildInto in _BuildIntoTable[itemId])
            GetItemUI().InitAll(buildInto);
    }
}