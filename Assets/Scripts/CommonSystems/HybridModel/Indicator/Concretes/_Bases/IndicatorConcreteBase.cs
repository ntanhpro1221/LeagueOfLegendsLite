using UnityEngine;

public abstract class IndicatorConcreteBase : ScriptableObject {
    public const string ASSET_PATH = "Indicators/";

    [field: SerializeField] protected Material _MainMate { get; private set; }

    public abstract void Enable(IndicatorShower components);

    public abstract void Disable(IndicatorShower components);

    public abstract void UpdateShower(
        IndicatorShower              components
      , in  IndicatorShower.Metadata metadata
      , ref ActivableItemData        itemData);
}