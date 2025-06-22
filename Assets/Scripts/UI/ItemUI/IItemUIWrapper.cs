using UnityEngine;

public abstract class IItemUIWrapper : MonoBehaviour {
    private ItemUICore _Core;

    public ItemUICore Core {
        get {
            if (_Core == null) _Core = GetComponentInChildren<ItemUICore>(includeInactive: true);
            return _Core;
        }
    }
}