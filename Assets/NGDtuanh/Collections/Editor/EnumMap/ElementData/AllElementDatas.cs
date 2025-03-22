using System;
using System.Collections.Generic;
using NGDtuanh.Collections;
using NGDtuanh.Utils;
using NGDtuanh.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace MyCustomPatterns.Collections.Editor {
    public class AllElementDatas {
        public readonly SerializedProperty Keys;
        public readonly SerializedProperty Values;
        public readonly float              ElementHeightDel;
        public readonly Padding            ElementPadding;
        public readonly List<ElementData>  ElementDatas;

        public int VisibleCount;

        public AllElementDatas(SerializedProperty property) {
            Keys         = property.FindPropertyRelative(EnumMapPropertyName.Keys);
            Values       = property.FindPropertyRelative(EnumMapPropertyName.Values);
            ElementHeightDel = EnumMapInstanceDrawer.ElementPadding.vertical;
            ElementPadding = EnumMapInstanceDrawer.ElementPadding;

            var keyNames = property.FindPropertyRelative(EnumMapPropertyName.KeyNames);
            ElementDatas = new(Keys.arraySize);
            for (int i = 0; i < Keys.arraySize; ++i)
                ElementDatas.Add(new(
                    Keys.GetArrayElementAtIndex(i)
                  , Values.GetArrayElementAtIndex(i)
                  , new GUIContent(keyNames.GetArrayElementAtIndex(i).stringValue)
                  , visible: true
                  , this));
            VisibleCount = Keys.arraySize;
        }

        public void CollectionDuplicateFix() {
            if (Keys.arraySize  >= ElementDatas.Count) return;           // not my task
            if (ElementDatas[0] == ElementDatas[Keys.arraySize]) return; // not duplicate

            while (ElementDatas.Count > Keys.arraySize)
                ElementDatas.PopBack();
        }

        public void PushBackToSize(int size) {
            while (Keys.arraySize < size) {
                Keys.PushBack();
                Values.PushBack();
                ElementDatas.PushBack(new(
                    Keys.Back()
                  , Values.Back()
                  , label: new("")
                  , visible: true
                  , this));
            }
        }

        public void PopBackToSize(int size) {
            while (Keys.arraySize > size) {
                Keys.PopBack();
                Values.PopBack();
                ElementDatas.PopBack();
            }
        }

        public void Swap(int leftId, int rightId) {
            Keys.Swap(leftId, rightId);
            Values.Swap(leftId, rightId);
            // ElementDatas.Swap(leftId, rightId); // YES, DO NOT swap it because it already swapped with Keys and Values, other value will be sync in SyncNewestData function
        }

        public void SyncNewestData(Type keyType, string searchText) {
            var enumData = EnumDataHub.GetData(keyType);
            VisibleCount = 0;
            for (int i = 0; i < enumData.Count; ++i) {
                ElementDatas[i].Key.intValue = enumData.Values[i];
                ElementDatas[i].Label.text   = enumData.Names[i];
                ElementDatas[i].Visible =
                    string.IsNullOrEmpty(searchText)
                 || enumData.Names[i].Contains(searchText, StringComparison.OrdinalIgnoreCase);

                if (ElementDatas[i].Visible) ++VisibleCount;
            }
        }
    }
}