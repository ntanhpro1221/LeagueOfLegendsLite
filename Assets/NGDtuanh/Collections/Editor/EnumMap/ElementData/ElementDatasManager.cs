using System;
using NGDtuanh.Utils.Editor;
using UnityEditor;

namespace NGDtuanh.Collections.Editor {
    public class ElementDatasManager {
        private readonly EnumMapInstanceDrawer Drawer;
        private readonly SerializedProperty    EditorSessionCode;
        private readonly SerializedProperty    KeySynced;
        private readonly Type                  KeyType;

        public ElementDatasManager(EnumMapInstanceDrawer drawer, SerializedProperty property, Type keyType) {
            Drawer            = drawer;
            EditorSessionCode = property.FindPropertyRelative(EnumMapPropertyName.EditorSessionCode);
            KeySynced         = property.FindPropertyRelative(EnumMapPropertyName.KeySynced);
            KeyType           = keyType;
        }

        // TODO: You can even cache the swap result (eg. true index of a must be b so i will swap(a, b))
        // TODO: to avoid calculate multiples times per one enum type
        public void EnsureEnumKeySynced() {
            if (!ScriptReloadDetector.IsReloaded(EditorSessionCode)) return;
            // !!session code must be updated after enum change check, not here
            
            var  elementDirtyData = new ElementsDirtyData(Drawer.ThisProperty, KeyType);
            var  allElementDatas  = Drawer.AllElementDatas;
            bool myEnumChanged    = EnumDataHub.IsMyEnumChanged(KeyType, EditorSessionCode, elementDirtyData.PrevKeyNames);
            ScriptReloadDetector.SyncMySessionCode(EditorSessionCode);
            
            if (!myEnumChanged) return;
            KeySynced.boolValue = false;
            
            int trueCount = elementDirtyData.TrueKeyNames.Length;

            // add element (if old size < new size)
            allElementDatas.PushBackToSize(trueCount);
            elementDirtyData.PushBackToSize(trueCount);
            
            // restore value of old enum key (if possible)
            for (int i = 0; i < trueCount; ++i) {
                int matchID = elementDirtyData.PrevKeyNames.IndexOf(elementDirtyData.TrueKeyNames[i]);

                if (matchID == -1 || matchID == i) continue;

                allElementDatas.Swap(i, matchID);
                elementDirtyData.Swap(i, matchID);
            }

            // restore value of old enum value (if possible and not be restored by name yet)
            for (int i = 0; i < trueCount; ++i) {
                if (elementDirtyData.PrevKeyNames[i] == elementDirtyData.TrueKeyNames[i]) continue;

                int matchID = elementDirtyData.PrevKeyValues.IndexOf(elementDirtyData.TrueKeyValues[i]);

                if (matchID == -1 || matchID == i) continue;

                if (matchID                                < trueCount
                 && elementDirtyData.PrevKeyNames[matchID] == elementDirtyData.TrueKeyNames[matchID])
                    continue;

                allElementDatas.Swap(i, matchID);
                elementDirtyData.Swap(i, matchID);
            }

            // remove element (if old size > new size)
            allElementDatas.PopBackToSize(trueCount);
            elementDirtyData.PopBackToSize(trueCount);

            // sync new data
            allElementDatas.SyncNewestData(KeyType, Drawer.SearchText);
            elementDirtyData.SyncNewestData();
        }
    }
}