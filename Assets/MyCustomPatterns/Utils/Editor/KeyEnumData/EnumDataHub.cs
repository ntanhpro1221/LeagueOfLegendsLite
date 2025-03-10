using System;
using System.Collections.Generic;
using System.Linq;

namespace NGDtuanh.Utils.Editor {
    public static class EnumDataHub {
        private static readonly Dictionary<Type, EnumData> Datas = new();

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptReload() => UpdateAllData();

        private static void CorrectData(Type type) =>
            Datas[type].SetData(
                Enum.GetNames(type)
              , Enum.GetValues(type).Cast<int>().ToArray());

        private static void UpdateAllData() {
            foreach (var type in Datas.Keys) CorrectData(type);
        }

        public static EnumData GetData(Type type) {
            if (!Datas.ContainsKey(type)) {
                Datas.Add(type, new());
                CorrectData(type);
            }

            return Datas[type];
        }
    }
}