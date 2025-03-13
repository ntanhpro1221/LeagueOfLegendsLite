using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NGDtuanh.Utils.Editor {
    public static class EnumDataHub {
        public static EnumData GetData(Type type) {
            EnsureDataKeyExist(type);
            return Datas[type].Item1;
        }
        
        /// <summary>
        /// You should sync your session code after that
        /// </summary>
        /// <returns></returns>
        public static bool IsMyEnumChanged(Type type, SerializedProperty localEditorSessionCode, List<string> keyNames) {
            // whether result is cached
            EnsureDataKeyExist(type);
            if (Datas[type].Item2.ContainsKey(localEditorSessionCode.hash128Value))
                return Datas[type].Item2[localEditorSessionCode.hash128Value];

            // check key names
            bool result = !Datas[type].Item1.Names.SequenceEqual(keyNames);

            // cached result
            Datas[type].Item2.Add(localEditorSessionCode.hash128Value, result);

            return result;
        }

        # region PRIVATE

        private static readonly Dictionary<Type, (EnumData, Dictionary<Hash128, bool>)> Datas = new();

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptReload() { // Reset all
            Datas.Clear();
        }

        private static void EnsureDataKeyExist(Type type) {
            if (!Datas.ContainsKey(type))
                Datas.Add(type, (new EnumData().SetData(type), new()));
        }

        #endregion
    }
}