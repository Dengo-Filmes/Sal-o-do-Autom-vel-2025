using UnityEngine;
using System.Collections.Generic;
using System;

namespace viperOSK
{

    [CreateAssetMenu(fileName = "viperOSK_LanguagePackage", menuName = "ScriptableObjects/viperOSK_LanguagePackage", order = 1)]
    public class OSK_LanguagePackage: ScriptableObject
    {
        [SerializeField]
        [TextArea(15, 6)]
        public string keyboardLayout;

        [Space]

        [SerializeField]
        [TextArea(15, 6)]
        public string altKeyboardLayout;

        [Space]
        public OSK_AccentAssetObj accentPackage;

        [Space]

        [Tooltip("Culture codes in priority order, e.g., el, ru, hy, ar, en")]
        public List<string> cultures = new List<string> { "el" };

        [Header("Case & Canonicalization")]
        [Tooltip("Uppercase and lowercase use the same GLYPH slot")]
        public bool collapseCase = true;

        [Tooltip("Prefer lowercase as the stored representative when collapsing case")]
        public bool preferLowercase = true;

        [Tooltip("Map Greek final sigma (ς U+03C2) to σ U+03C3 so they share one glyph slot")]
        public bool unifyGreekFinalSigma = true;

        [Header("Letter Filtering")]
        [Tooltip("Include uppercase letters where applicable")]
        public bool includeUppercase = true;

        [Tooltip("Include lowercase letters where applicable")]
        public bool includeLowercase = true;

        [Header("Custom Ranges (Hex)")]
        [Tooltip("Extra ranges to include (e.g., FB50–FDFF for Arabic Presentation Forms)")]
        public List<HexRange> extraIncludeRanges = new List<HexRange>();

        [Tooltip("Ranges to exclude (e.g., 1F00–1FFF to drop Greek Extended)")]
        public List<HexRange> excludeRanges = new List<HexRange>();
    }

    [Serializable]
    public struct HexRange
    {
        [Tooltip("Start (hex), e.g., 0370 or 0x0370")]
        public string startHex;
        [Tooltip("End (hex), inclusive, e.g., 03FF or 0x03FF")]
        public string endHex;
    }
}

