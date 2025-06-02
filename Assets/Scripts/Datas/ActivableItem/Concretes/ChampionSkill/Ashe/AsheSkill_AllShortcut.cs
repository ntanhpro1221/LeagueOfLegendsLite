#if UNITY_EDITOR
using UnityEngine;

[CreateAssetMenu(fileName = "AsheSkill_AllShortcut", menuName = ASSET_PATH + "Ashe/AsheSkill_AllShortcut")]
public class AsheSkill_AllShortcut : IActivableItemGroupShortcut {
    protected override void CreateAll() {
        CreateAsset<AsheSkill_Passive>(); 
        CreateAsset<AsheSkill_Q>(); 
        CreateAsset<AsheSkill_W>(); 
        CreateAsset<AsheSkill_E>(); 
        CreateAsset<AsheSkill_R>(); 
    }
}
#endif