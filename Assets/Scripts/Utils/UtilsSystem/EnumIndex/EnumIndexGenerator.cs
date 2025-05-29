#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NGDtuanh.Utils.Editors;
using UnityEditor;

[InitializeOnLoad]
public static class EnumIndexGenerator {
    static EnumIndexGenerator() {
        var enumNames = GetAllEnumNames();

        string content =
            $@"using System;
using NGDtuanh.BubleAsset;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;

public struct EnumIndexData : IComponentData {{
{string.Join("\n", enumNames.Select(pathName =>
    $"\tpublic BlobAssetReference<BubleEnMap<{pathName.Item1}, int>> _{pathName.Item2}Ref;"))}

{string.Join("\n", enumNames.Select(pathName =>
    $"\tpublic ref BubleEnMap<{pathName.Item1}, int> {pathName.Item2} => ref _{pathName.Item2}Ref.Value;"))}
}}

public class EnumIndexAuthoring : MonoBehaviour {{
    private class Baker : Baker<EnumIndexAuthoring> {{
        public override void Bake(EnumIndexAuthoring authoring) {{
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var data   = new EnumIndexData();

{string.Join("\n", enumNames.Select(pathName =>
    $"\t\t\tCreateEnumIndex(out data._{pathName.Item2}Ref);"))}

            AddComponent(entity, data);
        }}

        private void CreateEnumIndex<TKey>(out BlobAssetReference<BubleEnMap<TKey, int>> result)
            where TKey : unmanaged, Enum {{
            GetIndexMap<TKey>().CreateBlobAssetReferenceInBaker(out result, this, out _);
        }}
    }}
    public static CovEnumMap<TKey, int> GetIndexMap<TKey>() where TKey : unmanaged, Enum {{
        var enumMap = new CovEnumMap<TKey, int>();
        int curId   = -1;
        foreach (var key in enumMap.Keys)
            enumMap[key] = ++curId;
        return enumMap;
    }}
}}";

        string path = Path.Combine(AssetHelper.GetScriptPathWithoutFileName(nameof(EnumIndexGenerator)), "EnumIndexAuthoring.cs");

        AssetHelper.SafeWriteToFile(path, content);
    }

    private static List<(string, string)> GetAllEnumNames() => Assembly
        .GetExecutingAssembly().GetTypes()
        .Where(item => Attribute.IsDefined(item, typeof(GenerateIndexAttribute)))
        .Select(item => {
            var names = new List<string>();

            do names.Add(item.Name);
            while ((item = item.DeclaringType) != null);

            names.Reverse();
            return (string.Join('.', names), string.Join('_', names));
        }).ToList();
}
#endif