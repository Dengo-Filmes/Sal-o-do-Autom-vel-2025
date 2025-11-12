//////////////////////////////////////////////////////////////
/// (c) vipercode corp 2025
/// viperOSK v3.6
/// Utility for a resizable square in UI
/// 
/////////////////////////////////////////////////////////////



using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // New Input System
#endif

namespace viperOSK_Examples
{ 

[DefaultExecutionOrder(1000)]
[RequireComponent(typeof(RectTransform))]
public class ResizableRect4 : MonoBehaviour
{
    [Serializable] public class RectTransformEvent : UnityEvent<RectTransform> { }

    public Canvas canvas;                         // Auto-detected if left null
    public float handleSize = 14f;                // px
    public float handleOffset = 8f;               // px (outside the rect)
    public Vector2 minSize = new Vector2(24, 24); // px
    public bool forceCenterAnchors = true;        // keeps math stable
    public bool holdShiftToLockAspect = true;     // keep aspect with Shift
    public Color handleColor = new Color(1, 1, 1, 0.95f);
    public Sprite handleSprite;                   // optional

    [Header("Events")]
    public RectTransformEvent OnResizeComplete;   // assign in Inspector

    RectTransform target;
    RectTransform parentRect;
    Camera eventCamera;
    float startAspect = 1f;

    enum Corner { NW, NE, SE, SW }

    void Awake()
    {
        target = GetComponent<RectTransform>();
        if (!canvas) canvas = GetComponentInParent<Canvas>();
        if (!canvas) { Debug.LogError("[ResizableRect4] No Canvas found in parents."); enabled = false; return; }

        parentRect = target.parent as RectTransform;
        eventCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null :
                      (canvas.worldCamera ? canvas.worldCamera : Camera.main);

        if (forceCenterAnchors)
        {
            Vector2 size = target.rect.size;
            Vector2 pos = target.anchoredPosition;
            target.anchorMin = target.anchorMax = new Vector2(0.5f, 0.5f);
            target.sizeDelta = size;
            target.anchoredPosition = pos;
        }

        startAspect = Mathf.Max(0.0001f, target.rect.size.x / Mathf.Max(0.0001f, target.rect.size.y));
        CreateHandles();
    }

    void CreateHandles()
    {
        // Remove any previous handles created by this script
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var ch = transform.GetChild(i);
            if (ch.GetComponent<_CornerHandle>() != null) Destroy(ch.gameObject);
        }

        // Four corner handles, positioned just outside the rect
        MakeHandle(Corner.NW, new Vector2(0f, 1f), new Vector2(-handleOffset, handleOffset));
        MakeHandle(Corner.NE, new Vector2(1f, 1f), new Vector2(handleOffset, handleOffset));
        MakeHandle(Corner.SE, new Vector2(1f, 0f), new Vector2(handleOffset, -handleOffset));
        MakeHandle(Corner.SW, new Vector2(0f, 0f), new Vector2(-handleOffset, -handleOffset));
    }

    void MakeHandle(Corner c, Vector2 anchor, Vector2 offset)
    {
        var go = new GameObject($"Handle_{c}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(_CornerHandle));
        go.layer = gameObject.layer;

        var rt = go.GetComponent<RectTransform>();
        rt.SetParent(transform, false);
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(handleSize, handleSize);
        rt.anchoredPosition = offset;

        var img = go.GetComponent<Image>();
        img.color = handleColor;
#if UNITY_EDITOR
        if (!handleSprite)
        {
            var builtin = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            if (builtin) img.sprite = builtin;
        }
        else img.sprite = handleSprite;
#else
        if (handleSprite) img.sprite = handleSprite;
#endif

        var h = go.GetComponent<_CornerHandle>();
        h.Init(this, target, parentRect, eventCamera, c, minSize, () => holdShiftToLockAspect, () => startAspect);
    }

    // Called by handles when a drag ends
    internal void RaiseResizeComplete()
    {
        OnResizeComplete?.Invoke(target);
    }

    // ---- Handle behaviour ----
    private class _CornerHandle : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IEndDragHandler
    {
        ResizableRect4 owner;
        RectTransform target;
        RectTransform parentRect;
        Camera cam;
        Corner corner;
        Vector2 minSize;
        Func<bool> shiftAspectGetter;
        Func<float> aspectGetter;

        Vector2 startMouseLocal;
        Vector2 startSize;
        Vector2 startPos;
        bool dragging;

        public void Init(
            ResizableRect4 owner, RectTransform target, RectTransform parentRect, Camera cam,
            Corner corner, Vector2 minSize, Func<bool> shiftAspectGetter, Func<float> aspectGetter)
        {
            this.owner = owner;
            this.target = target;
            this.parentRect = parentRect;
            this.cam = cam;
            this.corner = corner;
            this.minSize = minSize;
            this.shiftAspectGetter = shiftAspectGetter;
            this.aspectGetter = aspectGetter;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            LocalPointInParent(eventData.position, out startMouseLocal);
            startSize = target.rect.size;
            startPos = target.anchoredPosition;
            dragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            if (!LocalPointInParent(eventData.position, out var nowLocal)) return;

            Vector2 delta = nowLocal - startMouseLocal;
            float dx = delta.x, dy = delta.y;

            float w = startSize.x, h = startSize.y;
            Vector2 pos = startPos;

            switch (corner)
            {
                case Corner.NE:
                    w = Mathf.Max(minSize.x, startSize.x + dx);
                    h = Mathf.Max(minSize.y, startSize.y + dy);
                    pos += new Vector2((w - startSize.x) * 0.5f, (h - startSize.y) * 0.5f);
                    break;
                case Corner.NW:
                    w = Mathf.Max(minSize.x, startSize.x - dx);
                    h = Mathf.Max(minSize.y, startSize.y + dy);
                    pos += new Vector2(-(w - startSize.x) * 0.5f, (h - startSize.y) * 0.5f);
                    break;
                case Corner.SE:
                    w = Mathf.Max(minSize.x, startSize.x + dx);
                    h = Mathf.Max(minSize.y, startSize.y - dy);
                    pos += new Vector2((w - startSize.x) * 0.5f, -(h - startSize.y) * 0.5f);
                    break;
                case Corner.SW:
                    w = Mathf.Max(minSize.x, startSize.x - dx);
                    h = Mathf.Max(minSize.y, startSize.y - dy);
                    pos += new Vector2(-(w - startSize.x) * 0.5f, -(h - startSize.y) * 0.5f);
                    break;
            }

            // Optional aspect lock with Shift (supports both input systems)
            if (shiftAspectGetter != null && shiftAspectGetter.Invoke() && IsShiftHeld())
            {
                float aspect = Mathf.Max(0.0001f, aspectGetter != null ? aspectGetter.Invoke() : (startSize.x / Mathf.Max(0.0001f, startSize.y)));
                float hFromW = w / aspect;
                float wFromH = h * aspect;

                if (Mathf.Abs(wFromH - w) < Mathf.Abs(hFromW - h))
                {
                    float wNew = wFromH;
                    pos.x += (wNew - w) * 0.5f * (corner == Corner.NE || corner == Corner.SE ? 1f : -1f);
                    w = Mathf.Max(minSize.x, wNew);
                }
                else
                {
                    float hNew = hFromW;
                    pos.y += (hNew - h) * 0.5f * (corner == Corner.NE || corner == Corner.NW ? 1f : -1f);
                    h = Mathf.Max(minSize.y, hNew);
                }
            }

            target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
            target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
            target.anchoredPosition = pos;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!dragging) return;
            dragging = false;
            owner.RaiseResizeComplete();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            dragging = false;
            owner.RaiseResizeComplete();
        }

        bool LocalPointInParent(Vector2 screen, out Vector2 local)
        {
            var rect = parentRect ? parentRect : target;
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screen, cam, out local);
        }

        // --- Input System / Legacy Input abstraction ---
        static bool IsShiftHeld()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            return kb != null && (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed);
#else
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif
        }
    }
}
}