using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NGDtuanh.Utils;
using NGDtuanh.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace NGDtuanh.PropertySet.Editor {
    [CustomPropertyDrawer(typeof(PropertySet<,>), true)]
    public class PropertySetDrawer : PropertyDrawer {
        #region CONFIG UI VAR

        private static readonly float  LineHeight          = EditorGUIUtility.singleLineHeight;
        private static readonly float  LineSpace           = 2;
        private static readonly float  ValueIndent         = 16;
        private static readonly float  LabelHeight         = LineHeight + LineSpace + 3;
        private static readonly float  FindBoxWidth        = 200;
        private static readonly string SearchBoxName       = "LmaoSearchBox";
        private static readonly float  NotFoundValueHeight = LineHeight;

        private static readonly Padding ValuePadding = new(
            left: ValueIndent
          , right: 3
          , top: 3
          , bot: 3);

        private static readonly Padding ValueNotFinalPadding = new(
            left: ValueIndent
          , right: 0
          , top: 0
          , bot: 0);

        #endregion

        private ScriptReloadDetector _ScriptReloadDetector;

        private class SerializedCombo {
            public readonly SerializedProperty _Values;
            public readonly SerializedProperty _IsVisible;
            public readonly SerializedProperty _VisibleCount;
            public readonly SerializedProperty _SearchText;
            public readonly Type               KeyType;
            public readonly bool               IsFinalType;

            public SerializedCombo(SerializedProperty property, FieldInfo fieldInfo) {
                _Values       = property.FindPropertyRelative(nameof(_Values));
                _IsVisible    = property.FindPropertyRelative(nameof(_IsVisible));
                _VisibleCount = property.FindPropertyRelative(nameof(_VisibleCount));
                _SearchText   = property.FindPropertyRelative(nameof(_SearchText));
                var thisType = GetTruePropertySetType(fieldInfo);
                KeyType     = thisType!.GenericTypeArguments[0];
                IsFinalType = !thisType.EqualsWithoutGeneric(thisType!.GenericTypeArguments[1]);
            }


            private static readonly Type PropertySetType = typeof(PropertySet<,>);

            private static Type GetTruePropertySetType(FieldInfo fieldInfo) {
                Type result = fieldInfo.FieldType;
                while (!result.EqualsWithoutGeneric(PropertySetType)) {
                    if (result!.IsArray) result = result.GetElementType();
                    else result                 = result.BaseType;
                }

                return result;
            }

            public class GetHeight : SerializedCombo {
                private readonly SerializedProperty _RootProperty;
                private readonly SerializedProperty _Dirty;
                private readonly string[]           _TrueKeyNames;

                // JUST INIT ON DIRTY
                private     SerializedProperty _PrevKeyNames;
                private     SerializedProperty _PrevKeyValues;
                private     int[]              _TrueKeyValues;
                private new string             _SearchText;
                private     List<bool>         _IsNewItem;

                public int Count     => _Values.arraySize;
                public int TrueCount => _TrueKeyNames.Length;

                public GetHeight(SerializedProperty property, FieldInfo fieldInfo) : base(property, fieldInfo) {
                    _RootProperty = property;
                    _Dirty        = property.FindPropertyRelative(nameof(_Dirty));
                    _TrueKeyNames = EnumDataHub.GetData(KeyType).Names;
                }

                private void InitDirtyVars() {
                    _PrevKeyNames  = _RootProperty.FindPropertyRelative(nameof(_PrevKeyNames));
                    _PrevKeyValues = _RootProperty.FindPropertyRelative(nameof(_PrevKeyValues));
                    _TrueKeyValues = EnumDataHub.GetData(KeyType).Values;
                    _SearchText    = base._SearchText.stringValue;
                    _IsNewItem     = Enumerable.Repeat(false, _Values.arraySize).ToList();
                }

                public void SyncEnumKey(ref ScriptReloadDetector scriptReloadDetector) {
                    if (!scriptReloadDetector.IsReloaded_Update() && !_Dirty.boolValue) return;
                    _Dirty.boolValue = false;
                    InitDirtyVars();

                    // add element (if old size < new size)
                    PushBackToSize(TrueCount);

                    // restore value of old enum key (if possible)
                    for (int i = 0; i < TrueCount; ++i) {
                        int matchID = _PrevKeyNames.IndexOf((item, id) =>
                            !_IsNewItem[id]
                         && item.stringValue.Equals(_TrueKeyNames[i]));

                        if (matchID == -1 || matchID == i) continue;

                        Swap(i, matchID);
                    }

                    // restore value of old enum value (if possible and not be restored by name yet)
                    for (int i = 0; i < TrueCount; ++i) {
                        if (_PrevKeyNames.GetArrayElementAtIndex(i).stringValue == _TrueKeyNames[i]) continue;

                        int matchID = _PrevKeyValues.IndexOf((item, id) =>
                            !_IsNewItem[id]
                         && item.intValue == _TrueKeyValues[i]);

                        if (matchID == -1 || matchID == i) continue;

                        if (matchID < TrueCount
                         && _TrueKeyNames[matchID].Equals(_PrevKeyNames.GetArrayElementAtIndex(matchID).stringValue))
                            continue;

                        Swap(i, matchID);
                    }

                    // remove element (if old size > new size)
                    PopBackToSize(TrueCount);

                    // reassign some data
                    _VisibleCount.intValue = 0;
                    for (int i = 0; i < TrueCount; ++i) {
                        // visible item
                        bool visible = 
                            _SearchText == ""
                         || _TrueKeyNames[i].Contains(_SearchText, StringComparison.OrdinalIgnoreCase);
                        _IsVisible.GetArrayElementAtIndex(i).boolValue = visible;
                        if (visible) ++_VisibleCount.intValue;
                        
                        // prev key name
                        _PrevKeyNames.GetArrayElementAtIndex(i).stringValue = _TrueKeyNames[i];

                        // prev kay value
                        _PrevKeyValues.GetArrayElementAtIndex(i).intValue = _TrueKeyValues[i];
                    }
                }

                private void Swap(int leftId, int rightId) {
                    _Values.Swap(leftId, rightId);
                    _PrevKeyNames.Swap(leftId, rightId);
                    _PrevKeyValues.Swap(leftId, rightId);
                    _IsVisible.Swap(leftId, rightId);
                    _IsNewItem.Swap(leftId, rightId);
                }

                private void PushBackToSize(int size) {
                    while (Count < size) {
                        _Values.PushBack();
                        _PrevKeyNames.PushBack();
                        _PrevKeyValues.PushBack();
                        _IsVisible.PushBack();
                        _IsNewItem.PushBack(true);
                    }
                }

                private void PopBackToSize(int size) {
                    while (Count > size) {
                        _Values.PopBack();
                        _PrevKeyNames.PopBack();
                        _PrevKeyValues.PopBack();
                        _IsVisible.PopBack();
                        _IsNewItem.PopBack();
                    }
                }
            }

            public class OnGUI : SerializedCombo {
                public readonly int                  TrueCount;
                public readonly SerializedProperty[] IsVisibleElements;

                // just init when draw
                public SerializedProperty[]     ValueElements;
                public float[]                  Height;
                public string[]                 KeyNames;

                public OnGUI(SerializedProperty property, FieldInfo fieldInfo) : base(property, fieldInfo) {
                    TrueCount         = EnumDataHub.GetData(KeyType).Count;
                    IsVisibleElements = _IsVisible.ToArray(SerializedPropertyExtensions.Self.Getter);
                }

                public void FullInit() {
                    ValueElements     = new SerializedProperty[_Values.arraySize];
                    Height            = new float[_Values.arraySize];
                    for (int i = 0; i < _Values.arraySize; ++i) {
                        if (IsVisibleElements[i].boolValue) {
                            ValueElements[i] = _Values.GetArrayElementAtIndex(i);
                            Height[i]        = EditorGUI.GetPropertyHeight(ValueElements[i]);
                        }
                    }

                    KeyNames = EnumDataHub.GetData(KeyType).Names;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float height = LabelHeight;

            if (property.isExpanded) {
                SerializedCombo.GetHeight combo = new(property, fieldInfo);

                // Reassign values if there is some enum's script change
                combo.SyncEnumKey(ref _ScriptReloadDetector);

                float totalVisibleHeight = 0;
                for (int i = 0; i < combo.TrueCount; ++i)
                    if (combo._IsVisible.GetArrayElementAtIndex(i).boolValue)
                        totalVisibleHeight += EditorGUI.GetPropertyHeight(combo._Values.GetArrayElementAtIndex(i));

                height +=
                    LineSpace * combo._VisibleCount.intValue.SubToZero(1)
                  + totalVisibleHeight
                  + ValuePadding.vertical
                        .IfOnly(combo.IsFinalType)
                  + ValueNotFinalPadding.vertical
                        .IfOnly(combo.Count > 0 && !combo.IsFinalType)
                  + NotFoundValueHeight
                        .IfOnly(combo._VisibleCount.intValue == 0 && combo.Count != 0);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            var valuePosition = position;

            // LABEL
            Vector2 labelRealSize = GUI.skin.label.CalcSize(label);
            labelRealSize.x += 10;
            Rect labelRect      = valuePosition.With_Size(labelRealSize.x, LabelHeight)
                // .With_SizeKeepCenter(Axis.Vertical, labelRealSize.y)
                ;
            bool beforeExpanded = property.isExpanded;
            property.isExpanded = EditorGUI.Foldout(
                labelRect
              , property.isExpanded
              , fieldInfo.FieldType.IsArray
                    ? label.text
                    : property.displayName
              , true);

            // FIELD
            if (property.isExpanded && beforeExpanded) {
                SerializedCombo.OnGUI combo = new(property, fieldInfo);

                // FIELD BOX
                GUI.Box(valuePosition.CutBySize(Direction.Top, LabelHeight)
                  , GUIContent.none, EditorStyles.helpBox);

                // FIELD INDENT
                if (combo.IsFinalType)
                    valuePosition                            = valuePosition.With_Padding(ValuePadding);
                else if (combo.TrueCount != 0) valuePosition = valuePosition.With_Padding(ValueNotFinalPadding);

                if (combo._VisibleCount.intValue != 0) { // FIELD ELEMENTS
                    combo.FullInit();

                    Rect fieldRect = valuePosition;
                    fieldRect.height = LabelHeight;
                    for (int i = 0; i < combo.TrueCount; ++i) {
                        if (!combo.IsVisibleElements[i].boolValue) continue;
                        fieldRect.y      += fieldRect.height;
                        fieldRect.height =  combo.Height[i];
                        EditorGUI.PropertyField(
                            fieldRect
                          , combo.ValueElements[i]
                          , new GUIContent(combo.KeyNames[i])
                          , true);
                        fieldRect.height += LineSpace;
                    }
                }
                else {
                    EditorGUI.LabelField(
                        valuePosition
                            .CutBySize(Direction.Top, LabelHeight)
                            .With_SizeKeepCenter(Axis.Vertical, NotFoundValueHeight)
                      , $"Not found any item with name \"{combo._SearchText.stringValue}\"");
                }

                // Search box, clear button and find button rect
                float findButtonWidth = GUI.skin.label.CalcSize(new("Clear")).x + 4;
                Rect  findToolRect    = labelRect.With_SizeKeepCenter(Axis.Vertical, labelRealSize.y);
                findToolRect.xMin = labelRect.xMax;
                findToolRect.xMax = position.xMax;
                Rect[] findToolRectList = findToolRect.DivideByAllSizes(
                    axis: Axis.Horizontal
                  , alignLeftTopAmount: 0
                  , space: 5
                  , FindBoxWidth, findButtonWidth, findButtonWidth);
                var (findBoxRect, findBtnRect, clearBtnRect) = (findToolRectList[0], findToolRectList[1], findToolRectList[2]);
                
                // Search box
                GUI.SetNextControlName(SearchBoxName);
                combo._SearchText.stringValue = EditorGUI.TextField(findBoxRect, combo._SearchText.stringValue);
                
                // Clear button
                if (combo._SearchText.stringValue.Equals(""))
                    GUI.enabled = false;
                if (GUI.Button(clearBtnRect, "Clear", EditorStyles.miniButtonMid)) {
                    // unfocus search box to clear text buffer
                    combo._SearchText.stringValue = "";
                    if (GUI.GetNameOfFocusedControl() == SearchBoxName)
                        GUIUtility.keyboardControl = 0;

                    try {
                        // change element's visibility
                        for (int i = 0; i < combo.TrueCount; ++i) combo.IsVisibleElements[i].boolValue = true;
                        combo._VisibleCount.intValue = combo.TrueCount;
                    }
                    catch {
                        Debug.Log(combo._IsVisible == null);
                        Debug.Log(combo.TrueCount + " | " + combo._VisibleCount.intValue + " | " + combo.IsVisibleElements?.Length + " | " + combo._IsVisible?.arraySize);
                    }
                }

                GUI.enabled = true;
                
                // Find button
                string cachedSearchText = combo._SearchText.stringValue;
                if (GUI.Button(findBtnRect, "Find", EditorStyles.miniButtonMid)) {
                    combo._VisibleCount.intValue = 0;
                    for (int i = 0; i < combo.TrueCount; ++i) {
                        combo.IsVisibleElements[i].boolValue = cachedSearchText == "" || combo.KeyNames[i].Contains(cachedSearchText, StringComparison.OrdinalIgnoreCase);
                        if (combo.IsVisibleElements[i].boolValue) ++combo._VisibleCount.intValue;
                    }
                }
            }

            EditorGUI.EndProperty();
        }
    }
}