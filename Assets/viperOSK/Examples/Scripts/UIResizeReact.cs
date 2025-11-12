//////////////////////////////////////////////////////////////
/// (c) vipercode corp 2025
/// viperOSK v3.6
/// Utility that detects change in resolution and triggers a UnityEvent
/// 
/////////////////////////////////////////////////////////////



using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace viperOSK_Examples
{
    [ExecuteAlways]
    public class UIResizeReact : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Root canvas of your UI. If left empty, it will be found at runtime.")]
        public Canvas rootCanvas;

        [Tooltip("UI element to stretch to full width and bottom percentage of the screen.")]
        public RectTransform targetToResize;

        [Header("Behavior")]
        [Range(0f, 1f)]
        [Tooltip("Portion of the screen height to occupy from the bottom (e.g., 0.2 = 20%).")]
        public float bottomHeightPercent = 0.20f;

        [Tooltip("If true, base the viewport on the device safe area (notch, rounded corners).")]
        public bool useSafeArea = false;

        public struct ViewportInfo
        {
            public Vector2 sizePixels;   // width,height in pixels
            public Vector2 sizeUnits;    // width,height in canvas UI units (after CanvasScaler)
            public bool isLandscape;
            public ScreenOrientation screenOrientation;

            public RectTransform target;
        }

        public UnityEvent <ViewportInfo> OnViewportChanged;

        private int _lastW, _lastH;
        private ScreenOrientation _lastOrientation;

        void Awake()
        {
            EnsureRefs();
            CacheCurrent();
            Apply();
        }

        void OnEnable()
        {
            EnsureRefs();
            CacheCurrent();
            Apply();
        }

        void OnRectTransformDimensionsChange()
        {
            // Called when screen size or any parent rect changes; great for UI
            TryReactIfChanged();
        }

        void Update()
        {
            // Runtime guard for device rotation & resolution changes
            TryReactIfChanged();
        }

        private void TryReactIfChanged()
        {
            var w = Screen.width;
            var h = Screen.height;
            var o = Screen.orientation;

            if (w != _lastW || h != _lastH || o != _lastOrientation)
            {
                CacheCurrent();
                Apply();
            }
        }

        private void CacheCurrent()
        {
            _lastW = Screen.width;
            _lastH = Screen.height;
            _lastOrientation = Screen.orientation;
        }

        private void EnsureRefs()
        {
            if (rootCanvas == null)
            {
                var c = GetComponentInParent<Canvas>(true);
                if (c != null) rootCanvas = c.rootCanvas;
            }
            if (targetToResize == null)
            {
                targetToResize = GetComponent<RectTransform>();
            }
        }

        private void Apply()
        {
            if (rootCanvas == null) return;

            // 1) Determine the effective viewport rect in pixels
            Rect pixelRect = rootCanvas.pixelRect;
            if (useSafeArea)
            {
                pixelRect = Intersect(pixelRect, Screen.safeArea);
            }

            // 2) Convert to UI units (after CanvasScaler)
            float scale = rootCanvas.scaleFactor <= 0f ? 1f : rootCanvas.scaleFactor;
            Vector2 sizePixels = new Vector2(pixelRect.width, pixelRect.height);
            Vector2 sizeUnits = sizePixels / scale;

            // 3) Resize target: full width & bottom X% height (in anchors so it adapts automatically)
            if (targetToResize != null)
            {
                // Stretch horizontally, stick to bottom, take bottomHeightPercent of parent
                targetToResize.anchorMin = new Vector2(0f, 0f);
                targetToResize.anchorMax = new Vector2(1f, Mathf.Clamp01(bottomHeightPercent));
                targetToResize.pivot = new Vector2(0.5f, 0.5f);
                targetToResize.anchoredPosition = Vector2.zero;
                targetToResize.sizeDelta = Vector2.zero; // with anchors set, sizeDelta should be zero
            }

            // 4) Fire event
            var info = new ViewportInfo
            {
                sizePixels = sizePixels,
                sizeUnits = sizeUnits,
                isLandscape = Screen.width > Screen.height,
                screenOrientation = Screen.orientation,
                target = targetToResize
            };

            OnViewportChanged?.Invoke(info);

#if UNITY_EDITOR
            // Log in Play/Editor
            Debug.Log($"Viewport: {info.sizePixels.x}x{info.sizePixels.y} px | {info.sizeUnits.x}x{info.sizeUnits.y} units | {(info.isLandscape ? "Landscape" : "Portrait")} ({info.screenOrientation})");
#endif
        }

        private static Rect Intersect(Rect a, Rect b)
        {
            float xMin = Mathf.Max(a.xMin, b.xMin);
            float yMin = Mathf.Max(a.yMin, b.yMin);
            float xMax = Mathf.Min(a.xMax, b.xMax);
            float yMax = Mathf.Min(a.yMax, b.yMax);
            if (xMax < xMin || yMax < yMin) return new Rect(0, 0, 0, 0);
            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        // Convenience getters if you want to query sizes elsewhere
        public Vector2 GetViewportSizePixels()
        {
            if (rootCanvas == null) return new Vector2(Screen.width, Screen.height);
            Rect r = rootCanvas.pixelRect;
            if (useSafeArea) r = Intersect(r, Screen.safeArea);
            return new Vector2(r.width, r.height);
        }

        public Vector2 GetViewportSizeUnits()
        {
            Vector2 px = GetViewportSizePixels();
            float scale = (rootCanvas != null && rootCanvas.scaleFactor > 0f) ? rootCanvas.scaleFactor : 1f;
            return px / scale;
        }
    }

}//namespace

