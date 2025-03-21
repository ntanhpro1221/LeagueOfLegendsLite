using System;
using System.Collections.Generic;
using NGDtuanh.Utils;
using NGDtuanh.Utils.Editor;
using UnityEditor;

namespace MyCustomPatterns.Collections.Editor {
    public class ElementsDirtyData {
        private readonly SerializedProperty       KeyNames_Serial;
        private readonly List<SerializedProperty> PrevKeyNames_Serial;

        public readonly List<string> PrevKeyNames;
        public readonly List<int?>   PrevKeyValues;
        public readonly string[]     TrueKeyNames;
        public readonly int[]        TrueKeyValues;

        public ElementsDirtyData(SerializedProperty property, Type keyType) {
            KeyNames_Serial = property.FindPropertyRelative(EnumMapPropertyName.KeyNames);
            var keys = property.FindPropertyRelative(EnumMapPropertyName.Keys);

            var trueEnumData = EnumDataHub.GetData(keyType);
            PrevKeyNames_Serial = new(trueEnumData.Count);
            PrevKeyNames        = new(trueEnumData.Count);
            PrevKeyValues       = new(trueEnumData.Count);
            for (int i = 0; i < KeyNames_Serial.arraySize; ++i) { // yes it is not enumData.Count
                PrevKeyNames_Serial.Add(KeyNames_Serial.GetArrayElementAtIndex(i));
                PrevKeyNames.Add(PrevKeyNames_Serial[i].stringValue);
                PrevKeyValues.Add(keys.GetArrayElementAtIndex(i).intValue);
            }

            TrueKeyNames  = trueEnumData.Names;
            TrueKeyValues = trueEnumData.Values;
        }

        public void PushBackToSize(int size) {
            while (KeyNames_Serial.arraySize < size) {
                KeyNames_Serial.PushBack();
                PrevKeyNames_Serial.PushBack(KeyNames_Serial.Back());
                PrevKeyNames.PushBack(null);
                PrevKeyValues.PushBack(null);
            }
        }

        public void PopBackToSize(int size) {
            while (KeyNames_Serial.arraySize > size) {
                KeyNames_Serial.PopBack();
                PrevKeyNames_Serial.PopBack();
                PrevKeyNames.PopBack();
                PrevKeyValues.PopBack();
            }
        }

        public void Swap(int leftId, int rightId) {
            KeyNames_Serial.Swap(leftId, rightId);
            // PrevKeyNames_Serial.Swap(leftId, rightId); // YES, DO NOT swap id because it already swapped with keynames_serial!!
            PrevKeyNames.Swap(leftId, rightId);
            PrevKeyValues.Swap(leftId, rightId);
        }

        public void SyncNewestData() {
            for (int i = 0; i < TrueKeyNames.Length; ++i)
                PrevKeyNames_Serial[i].stringValue = TrueKeyNames[i];
        }
    }
}