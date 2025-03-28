#if UNITY_EDITOR
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
using NGDtuanh.BlobAssetExtend;
using NGDtuanh.Collections;
using Unity.Entities;
using UnityEngine;

public struct EnumIndexData : IComponentData {{
{string.Join("\n", enumNames.Select(name =>
    $"\tpublic BlobAssetReference<BubleEnMap<{name}, int>> _{name}Ref;"))}

{string.Join("\n", enumNames.Select(name =>
    $"\tpublic ref BubleEnMap<{name}, int> {name} => ref _{name}Ref.Value;"))}
}}

public class EnumIndexAuthoring : MonoBehaviour {{
    private class Baker : Baker<EnumIndexAuthoring> {{
        public override void Bake(EnumIndexAuthoring authoring) {{
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var data   = new EnumIndexData();

{string.Join("\n", enumNames.Select(name =>
    $"\t\t\tCreateEnumIndex(out data._{name}Ref);"))}

            AddComponent(entity, data);
        }}

        private void CreateEnumIndex<TKey>(
            out BlobAssetReference<BubleEnMap<TKey, int>> result)
            where TKey : struct, Enum {{
            var enumMap = new CovEnumMap<TKey, int>();
            int                   curId   = -1;
            foreach (var key in enumMap.Keys)
                enumMap[key] = ++curId;
            enumMap.CreateBlobAssetReferenceInBaker(out result, this, out _);
        }}
    }}
}}";

        string path = Path.Combine(AssetHelper.GetScriptPathWithoutFileName(nameof(EnumIndexGenerator)), "EnumIndexAuthoring.cs");

        AssetHelper.SafeWriteToFile(path, content);
    }

    private static List<string> GetAllEnumNames() => Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(item => item.IsEnum && !item.IsNested)
        .Select(item => item.Name)
        .ToList();
}
#endif