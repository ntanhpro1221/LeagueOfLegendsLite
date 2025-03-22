using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NGDtuanh.Collections;
using NGDtuanh.Utils;
using NGDtuanh.Utils.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MyCustomPatterns.Collections.Editor {
    public class EnumMapInstanceDrawer {
        #region CONFIG VARS

        public const float FixFoldLabelXPos         = 8;
        public const float FixNotExpandExcessHeight = -10;
        public const float FixCenterSearchBoxYPos   = 1;
        public const float SearchBoxMaxWidth        = 300;
        public const float PlaceHolderPaddingLeft   = 15;
        public const float LabelWidth               = 200;
        public const float OneIndentWidth           = 15;

        public static readonly Padding ElementPadding = new(
            left: 0
          , right: 0
          , top: 1
          , bot: -1);

        private const           string     NoElementNotify            = "Enum is Empty.";
        private const           string     NotFoundElementMatchNotify = "No Enum with name: ";
        private static readonly GUIContent NotifyContent              = new();
        private static readonly GUIStyle   PlaceHolderStyle;

        static EnumMapInstanceDrawer() {
            PlaceHolderStyle                  = new(EditorStyles.label);
            PlaceHolderStyle.normal.textColor = Color.gray;
            PlaceHolderStyle.fontStyle        = FontStyle.Italic;
            PlaceHolderStyle.fontSize         = EditorStyles.label.fontSize;
        }

        #endregion
        
        #region SEARCH INPUT DEBOUNCING VARS

        private const double delayBeforeDoSearch    = 0.35;
        private       int    queueingSearchCallback;
        
        #endregion

        #region MAIN VARS

        internal readonly SerializedProperty  ThisProperty;
        internal readonly AllElementDatas     AllElementDatas;
        internal readonly ElementDatasManager ElementDatasManager;
        internal readonly ReorderableList     ReorderableListNotExpand;
        internal readonly ReorderableList     ReorderableListNotifyOnly;
        internal readonly Type                KeyType;
        internal readonly UnityEditor.Editor  Editor;
        internal readonly GenericMenu         GenericMenu;
        
        internal       GUIContent Label;

        internal ReorderableList ReorderableList { get; private set; }
        internal string          SearchText      { get; private set; } = "";
        private bool IsExpanded {
            get => ThisProperty.isExpanded;
            set => ThisProperty.isExpanded = value;
        }
        
        #endregion

        public EnumMapInstanceDrawer(SerializedProperty property, FieldInfo fieldInfo) {
            var enumMapTypeFinder = new EnumMapTypeFinder(fieldInfo);

            ThisProperty              = property;
            AllElementDatas           = new(property);
            ElementDatasManager       = new(this, property, enumMapTypeFinder.KeyType);
            ReorderableList           = MakeList(AllElementDatas);
            ReorderableListNotExpand  = MakeListNotExpanded();
            ReorderableListNotifyOnly = MakeListNotifyOnly();
            KeyType                   = enumMapTypeFinder.KeyType;
            Editor = ActiveEditorTracker.sharedTracker.activeEditors
                .First(editor => editor.target == property.serializedObject.targetObject);
            GenericMenu = MakeGenericMenu();
        }

        public float GetPropertyHeight() {
            ElementDatasManager.EnsureEnumKeySynced(); // must sync first
            AllElementDatas.CollectionDuplicateFix();  // when remove all elements from a collection and then add again
            
            float height;
            
            if (!IsExpanded) height = ReorderableListNotExpand.GetHeight();

            else if (AllElementDatas.ElementDatas.Count == 0)
                height = ReorderableListNotifyOnly.GetHeight();

            else if (AllElementDatas.ElementDatas.Count != 0 && AllElementDatas.VisibleCount == 0)
                height = ReorderableListNotifyOnly.GetHeight();

            else height = ReorderableList.GetHeight();

            return height;
        }

        public void OnGUI(Rect position, GUIContent label) {
            position.xMin += EditorGUI.indentLevel * OneIndentWidth;
            
            if (Label == null) // not init in constructor because label from get height function is f**king wrong !!
                Label = new(label);
            
            EditorGUI.BeginProperty(position, label, ThisProperty);

            if (!IsExpanded) ReorderableListNotExpand.DoList(position);

            else if (AllElementDatas.ElementDatas.Count == 0)
                DoNotifyList(position, NoElementNotify);

            else if (AllElementDatas.ElementDatas.Count != 0 && AllElementDatas.VisibleCount == 0)
                DoNotifyList(position, NotFoundElementMatchNotify + "'" + SearchText + "'");

            else ReorderableList.DoList(position);

            EditorGUI.EndProperty();
        }
        
        private ReorderableList MakeList(AllElementDatas allElementDatas) {
            var list = new ReorderableList(allElementDatas.ElementDatas, typeof(ElementData)
              , draggable: false
              , displayHeader: true
              , displayAddButton: false
              , displayRemoveButton: false);
            list.footerHeight = 0;
            
            // callback
            list.drawHeaderCallback      += DrawHeaderCallback;
            list.elementHeightCallback   += GetElementHeightCallback;
            list.drawElementCallback     += DrawElementCallback;

            return list;
        }
        
        private ReorderableList MakeListNotExpanded() {
            var list = new ReorderableList(new List<int> { 0 }, typeof(int)
              , draggable: false
              , displayHeader: true
              , displayAddButton: false
              , displayRemoveButton: false);
            list.footerHeight          =  0;
            
            // callback
            list.drawHeaderCallback    += DrawHeaderNotExpandedCallback;
            list.elementHeightCallback += (_) => FixNotExpandExcessHeight;
            list.drawElementCallback   += (_, _, _, _) => { };

            return list;
        }

        private ReorderableList MakeListNotifyOnly() {
            var list = new ReorderableList(new List<int>(), typeof(int)
              , draggable: false
              , displayHeader: true
              , displayAddButton: false
              , displayRemoveButton: false);
            list.footerHeight = 0;

            // callback
            list.drawHeaderCallback    += DrawHeaderCallback;
            list.drawNoneElementCallback   += DrawNoneElementNotifyOnlyCallback;

            return list;
        }

        private GenericMenu MakeGenericMenu() {
            var menu = new GenericMenu();
            menu.AddItem(new("Collapse All"), false, CollapseAllCallback);
            return menu;
        }

        private void DoGenericMenu(in Rect position) {
            if (Event.current.type != EventType.ContextClick
             || !position.Contains(Event.current.mousePosition)) return;
            GenericMenu.ShowAsContext();
            Event.current.Use();
        }
        
        private void DoNotifyList(Rect position, string content) {
            NotifyContent.text = content;
            ReorderableListNotifyOnly.DoList(position);
        }
        
        #region HEADER DRAW
        
        private int curIndent;
        
        private void BeforeDrawHeader(ref Rect position) {
            DoGenericMenu(position);
            
            curIndent             =  EditorGUI.indentLevel;
            EditorGUI.indentLevel =  0;
            position.xMin         += FixFoldLabelXPos;
        }

        private void AfterDrawHeader() {
            EditorGUI.indentLevel = curIndent;
        }

        private void DrawHeaderNotExpandedCallback(Rect position) {
            BeforeDrawHeader(ref position);
            
            IsExpanded = EditorGUI.Foldout(position, IsExpanded, Label, true); 
            
            AfterDrawHeader();
        }

        private void DrawHeaderCallback(Rect position) {
            BeforeDrawHeader(ref position);
            
            var rects = position.DivideByAllSizes(Axis.Horizontal, 0, 0
              , LabelWidth
              , SearchBoxMaxWidth);
            var searchBoxRect = rects[1];
            
            IsExpanded = EditorGUI.Foldout(position.With_Width(position.width - searchBoxRect.width)
              , IsExpanded, Label, true);
            DrawSearchBox(searchBoxRect);
            
            AfterDrawHeader();
        }

        private void CollapseAllCallback() {
            foreach (var item in AllElementDatas.ElementDatas)
                item.Value_InsideWrapper.isExpanded = false;
        }
        
        private void DrawSearchBox(Rect position) {
            // handle search text
            EditorGUI.BeginChangeCheck();
            
            SearchText = EditorGUI.TextField(
                position.Move(Axis.Vertical, FixCenterSearchBoxYPos)
              , "", SearchText, EditorStyles.toolbarSearchField);

            if (EditorGUI.EndChangeCheck()) {
                ++queueingSearchCallback;
                DelayInvoker.Invoke(DoSearchCallBack, delayBeforeDoSearch);
            }
            
            // draw placeholder
            if (string.IsNullOrEmpty(SearchText)) {
                Rect placeHolderRect = position;
                placeHolderRect.xMin += PlaceHolderPaddingLeft;
                EditorGUI.LabelField(placeHolderRect, "Find Key...", PlaceHolderStyle);
            }
        }
        
        #endregion
        
        // TODO: Improve searching with suffix tree :))
        private void DoSearchCallBack() {
            if (--queueingSearchCallback != 0) return;

            var enumNames = EnumDataHub.GetData(KeyType).Names;
            var elements  = AllElementDatas.ElementDatas;
            AllElementDatas.VisibleCount = 0;
            for (int i = 0; i < elements.Count; ++i) {
                elements[i].Visible =
                    string.IsNullOrEmpty(SearchText)
                 || enumNames[i].Contains(SearchText, StringComparison.OrdinalIgnoreCase);
                if (elements[i].Visible) ++AllElementDatas.VisibleCount;
            }

            ReorderableList = MakeList(AllElementDatas); // this helps reset list's height but leaves a pointer trash :((
            Editor.Repaint();
        }

        private float GetElementHeightCallback(int index) {
            return AllElementDatas.ElementDatas[index].GetHeight();
        }

        private void DrawElementCallback(Rect position, int index, bool isActive, bool isFocused) {
            if (!IsExpanded) return;
            AllElementDatas.ElementDatas[index].Draw(position);
        }

        private void DrawNoneElementNotifyOnlyCallback(Rect position) {
            EditorGUI.LabelField(position, NotifyContent);
        }
    }
}