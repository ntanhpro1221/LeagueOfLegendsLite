#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class IActivableItemGroupShortcut : ScriptableObject {
    public const string ASSET_PATH = IActivableItemDataSO.ASSET_PATH;
    
    private string thisPath;

    private void OnEnable() {
        EditorApplication.delayCall += DelayEnable;
    }

    private void DelayEnable() {
        EditorApplication.delayCall -= DelayEnable;

        var fullPath = AssetDatabase.GetAssetPath(this);
        thisPath = Path.GetDirectoryName(fullPath);
        CreateAll();
        AssetDatabase.DeleteAsset(fullPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    } 

    protected void CreateAsset<T>() where T : ScriptableObject {
        if (Attribute.GetCustomAttribute(typeof(T), typeof(CreateAssetMenuAttribute))
            is not CreateAssetMenuAttribute assetAttr) {
            Debug.LogError($"NGDtuanh: fail to get {nameof(CreateAssetMenuAttribute)} from {nameof(T)}.");
            return;
        }

        var assetPath = Path.Combine(thisPath, assetAttr.fileName + ".asset");
        if (!File.Exists(assetPath)) AssetDatabase.CreateAsset(CreateInstance<T>(), assetPath);
    }

    protected abstract void CreateAll();
}
#endif