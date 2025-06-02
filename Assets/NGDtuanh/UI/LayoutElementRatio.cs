using System;
using UnityEngine;
using UnityEngine.UI;

namespace NGDtuanh.UI {
    [AddComponentMenu("Layout/Layout Element Ratio", 140)]
    [ExecuteAlways]
    public class LayoutElementRatio : LayoutElement {
        private const float INF = 1e9f;

        public override float flexibleWidth  => 0;
        public override float flexibleHeight => 0;

        public enum AspectMode {
            WidthControlsHeight
          , HeightControlsWidth
        }

#region MAIN PROPERTIES

        [SerializeField] private AspectMode m_AspectMode;
        [SerializeField] private float      m_Padding;
        [SerializeField] private float      m_AspectRatio = 1;

        public AspectMode aspectMode {
            get => m_AspectMode;
            set {
                if (SetPropertyUtility.SetStruct(ref m_AspectMode, value)) SetDirty();
            }
        }

        public float padding {
            get => m_Padding;
            set {
                if (SetPropertyUtility.SetStruct(ref m_Padding, value)) SetDirty();
            }
        }

        public float aspectRatio {
            get => m_AspectRatio;
            set {
                if (SetPropertyUtility.SetStruct(ref m_AspectRatio, value)) SetDirty();
            }
        }

#endregion

#region RECT TRANSFORM

        [System.NonSerialized] private RectTransform m_Rect;

        private RectTransform m_CachedRect {
            get {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private bool m_DoesParentGroupExist;

        private RectTransform                   m_ParentRect;
        private HorizontalOrVerticalLayoutGroup m_ParentGroup;

        private void UpdateParentInfo(bool force) {
            if (!force && m_DoesParentGroupExist)
                return;

            m_ParentRect = m_CachedRect.parent as RectTransform;
            m_ParentGroup = m_ParentRect == null
                ? null
                : m_ParentRect.GetComponent<HorizontalOrVerticalLayoutGroup>();

            m_DoesParentGroupExist = m_ParentGroup ? true : false;

            if (!m_DoesParentGroupExist)
                Debug.LogWarning($"NGDtuanh: {nameof(LayoutElementRatio)} must be direct child of {nameof(HorizontalOrVerticalLayoutGroup)} when in scene. Otherwise, just remove it for better performance!");
        }

        protected override void OnTransformParentChanged() {
            base.OnTransformParentChanged();

            UpdateParentInfo(true);
        }

        protected override void OnEnable() {
            base.OnEnable();

            UpdateParentInfo(false);
        }

#endregion

#if UNITY_EDITOR
        
        protected override void Reset() {
            base.Reset();

            // Just suggest aspect mode in first time be added to game object
            UpdateParentInfo(false);
            if (m_DoesParentGroupExist)
                aspectMode = m_ParentGroup is HorizontalLayoutGroup
                    ? AspectMode.HeightControlsWidth
                    : AspectMode.WidthControlsHeight;
        }
        
#endif

        public override void CalculateLayoutInputHorizontal() {
            if (m_AspectMode == AspectMode.WidthControlsHeight) {
                preferredWidth = INF;
                minWidth       = 0;
            } else {
                preferredWidth = 0;
                minWidth = (
                        (m_DoesParentGroupExist
                            ? m_ParentRect.rect.height - m_ParentGroup.padding.vertical
                            : 0)
                      - m_Padding)
                  * m_AspectRatio;
            }
        }

        public override void CalculateLayoutInputVertical() {
            if (m_AspectMode == AspectMode.HeightControlsWidth) {
                preferredHeight = INF;
                minHeight       = 0;
            } else {
                preferredHeight = 0;
                minHeight = (
                        (m_DoesParentGroupExist
                            ? m_ParentRect.rect.width - m_ParentGroup.padding.horizontal
                            : 0)
                      - m_Padding)
                  / m_AspectRatio;
            }
        }
    }
}