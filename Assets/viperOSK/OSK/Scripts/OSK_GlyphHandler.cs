////////////////////////////////////////////////////////////////////////////////
//////
///
///////////////////////////////////////////////////////////////////////////////



using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace viperOSK
{

    public static class OSK_GlyphHandler
    {

        // -------- Public API ----------------------------------------------------

        /// Build glyph→enum assignments from a profile.
        /// Cultures are processed in order; duplicates are deduped (case-collapsed if requested).
        public static List<(string glyph, TEnum keycode)> BuildAssignments<TEnum>(OSK_LanguagePackage profile)
            where TEnum : struct, Enum
        {
            if (profile == null) throw new ArgumentNullException(nameof(profile));

            var allGlyphs = new List<string>(512);
            var seenByCodePoint = new HashSet<int>();

            // Precompute custom ranges
            var extraIncludes = ToIntRanges(profile.extraIncludeRanges);
            var excludes = ToIntRanges(profile.excludeRanges);

            foreach (var cultureName in profile.cultures.Where(c => !string.IsNullOrWhiteSpace(c)))
            {
                var script = ResolvePrimaryScript(new CultureInfo(cultureName));
                var baseRanges = GetRangesForScript(script);

                // Union = base + extraIncludes; then subtract excludes
                var merged = MergeRanges(baseRanges, extraIncludes);
                var filtered = ExcludeRanges(merged, excludes);

                var glyphs = EnumerateLetterGlyphs(
                    filtered,
                    profile.includeUppercase,
                    profile.includeLowercase,
                    profile.collapseCase,
                    profile.preferLowercase,
                    profile.unifyGreekFinalSigma ? script : (Script?)null
                );

                // Dedup across cultures by code point
                foreach (var g in glyphs)
                {
                    var cp = ToCodePoint(g);
                    if (cp >= 0 && seenByCodePoint.Add(cp))
                        allGlyphs.Add(g);
                }
            }

            // Find enum slots GLYPH_0001.. in numeric order
            var slots = GetGlyphEnumSlots<TEnum>();
            int n = Math.Min(allGlyphs.Count, slots.Count);
            var result = new List<(string glyph, TEnum keycode)>(n);
            for (int i = 0; i < n; i++) result.Add((allGlyphs[i], slots[i]));
            return result;
        }

        /// Optional: build a case-insensitive lookup (α and Α map to same slot).
        public static Dictionary<string, TEnum> BuildLookup<TEnum>(OSK_LanguagePackage profile)
            where TEnum : struct, Enum
        {
            var pairs = BuildAssignments<TEnum>(profile);
            var dict = new Dictionary<string, TEnum>(StringComparer.OrdinalIgnoreCase);
            foreach (var (glyph, code) in pairs)
                if (!dict.ContainsKey(glyph)) dict[glyph] = code;
            return dict;
        }

        // -------- Internals -----------------------------------------------------

        private enum Script { Latin, Greek, Cyrillic, Arabic, Armenian }

        private struct Range { public int Start, End; public Range(int s, int e) { Start = s; End = e; } }

        private static Script ResolvePrimaryScript(CultureInfo culture)
        {
            switch (culture.TwoLetterISOLanguageName)
            {
                case "el": return Script.Greek;
                case "ru":
                case "uk":
                case "bg":
                case "sr":
                case "mk": return Script.Cyrillic;
                case "ar": return Script.Arabic;
                case "hy": return Script.Armenian;
                default: return Script.Latin;
            }
        }

        private static IReadOnlyList<Range> GetRangesForScript(Script script)
        {
            switch (script)
            {
                case Script.Greek:
                    return new[] { new Range(0x0370, 0x03FF), new Range(0x1F00, 0x1FFF) };
                case Script.Cyrillic:
                    return new[] {
                    new Range(0x0400, 0x04FF), new Range(0x0500, 0x052F),
                    new Range(0x2DE0, 0x2DFF), new Range(0xA640, 0xA69F), new Range(0x1C80, 0x1C8F)
                };
                case Script.Arabic:
                    return new[] { new Range(0x0600, 0x06FF), new Range(0x0750, 0x077F), new Range(0x08A0, 0x08FF) };
                case Script.Armenian:
                    return new[] { new Range(0x0530, 0x058F), new Range(0xFB13, 0xFB17) };
                case Script.Latin:
                default:
                    return new[] {
                    new Range(0x0041, 0x007A), new Range(0x00C0, 0x00FF),
                    new Range(0x0100, 0x017F), new Range(0x0180, 0x024F)
                };
            }
        }

        private static List<Range> ToIntRanges(List<HexRange> hexRanges)
        {
            var list = new List<Range>();
            if (hexRanges == null) return list;
            foreach (var h in hexRanges)
            {
                if (TryParseHex(h.startHex, out var s) && TryParseHex(h.endHex, out var e) && s <= e)
                    list.Add(new Range(s, e));
            }
            return list;
        }

        private static bool TryParseHex(string hex, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(hex)) return false;
            var t = hex.Trim();
            if (t.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) t = t.Substring(2);
            return int.TryParse(t, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value);
        }

        private static List<Range> MergeRanges(IReadOnlyList<Range> a, List<Range> b)
        {
            var all = new List<Range>();
            if (a != null) all.AddRange(a);
            if (b != null) all.AddRange(b);
            if (all.Count <= 1) return all;

            all.Sort((x, y) => x.Start.CompareTo(y.Start));
            var merged = new List<Range>();
            var cur = all[0];
            for (int i = 1; i < all.Count; i++)
            {
                var r = all[i];
                if (r.Start <= cur.End + 1) cur.End = Math.Max(cur.End, r.End);
                else { merged.Add(cur); cur = r; }
            }
            merged.Add(cur);
            return merged;
        }

        private static List<Range> ExcludeRanges(List<Range> source, List<Range> excludes)
        {
            if (excludes == null || excludes.Count == 0) return source;
            var result = new List<Range>();
            foreach (var s in source)
            {
                int start = s.Start;
                foreach (var ex in excludes)
                {
                    if (ex.End < start || ex.Start > s.End) continue; // no overlap
                    if (ex.Start > start)
                    {
                        result.Add(new Range(start, Math.Min(ex.Start - 1, s.End)));
                    }
                    start = Math.Max(start, ex.End + 1);
                    if (start > s.End) break;
                }
                if (start <= s.End) result.Add(new Range(start, s.End));
            }
            return result;
        }

        private static List<string> EnumerateLetterGlyphs(
            List<Range> ranges,
            bool includeUppercase,
            bool includeLowercase,
            bool collapseCase,
            bool preferLowercase,
            Script? scriptForSpecials)
        {
            var list = new List<string>(256);

            // For collapseCase we’ll dedupe with OrdinalIgnoreCase + optional canonicalization.
            var seen = collapseCase ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                                    : new HashSet<string>(StringComparer.Ordinal);

            foreach (var r in ranges)
            {
                for (int cp = r.Start; cp <= r.End; cp++)
                {
                    if (!IsValidCodePoint(cp) || IsSurrogate(cp)) continue;

                    var s = char.ConvertFromUtf32(cp);
                    var cat = CharUnicodeInfo.GetUnicodeCategory(s, 0);
                    if (!IsLetter(cat)) continue;

                    if (!includeUppercase && IsUppercase(cat)) continue;
                    if (!includeLowercase && IsLowercase(cat)) continue;

                    // Canonicalize (e.g., Greek final sigma)
                    s = Canonicalize(s, scriptForSpecials);

                    // Collapse case: prefer lowercase representative
                    if (collapseCase && preferLowercase)
                    {
                        var lower = s.ToLowerInvariant();
                        s = (lower.Length == s.Length) ? lower : s;
                    }

                    if (seen.Add(s))
                        list.Add(s);
                }
            }
            return list;
        }

        private static string Canonicalize(string s, Script? script)
        {
            if (script == Script.Greek && s.Length == 1)
            {
                // Map final sigma ς (0x03C2) to σ (0x03C3)
                if (s[0] == '\u03C2') return "\u03C3";
            }
            return s;
        }

        private static bool IsValidCodePoint(int cp) => (uint)cp <= 0x10FFFF;
        private static bool IsSurrogate(int cp) => cp >= 0xD800 && cp <= 0xDFFF;

        private static bool IsLetter(UnicodeCategory c) =>
            c == UnicodeCategory.UppercaseLetter ||
            c == UnicodeCategory.LowercaseLetter ||
            c == UnicodeCategory.TitlecaseLetter ||
            c == UnicodeCategory.ModifierLetter ||
            c == UnicodeCategory.OtherLetter;

        private static bool IsUppercase(UnicodeCategory c) =>
            c == UnicodeCategory.UppercaseLetter || c == UnicodeCategory.TitlecaseLetter;

        private static bool IsLowercase(UnicodeCategory c) =>
            c == UnicodeCategory.LowercaseLetter;

        private static int ToCodePoint(string s)
        {
            if (string.IsNullOrEmpty(s)) return -1;
            return char.ConvertToUtf32(s, 0);
        }

        private static List<TEnum> GetGlyphEnumSlots<TEnum>() where TEnum : struct, Enum
        {
            var values = (TEnum[])Enum.GetValues(typeof(TEnum));
            var pairs = new List<(int order, TEnum value)>();
            foreach (var v in values)
            {
                var name = v.ToString();
                if (!name.StartsWith("GLYPH_", StringComparison.Ordinal)) continue;
                if (TryParseGlyphSuffix(name, out int n))
                    pairs.Add((n, v));
            }
            pairs.Sort((a, b) => a.order.CompareTo(b.order));
            return pairs.Select(p => p.value).ToList();
        }

        private static bool TryParseGlyphSuffix(string name, out int n)
        {
            n = 0;
            int idx = name.IndexOf('_');
            if (idx < 0 || idx + 1 >= name.Length) return false;
            return int.TryParse(name.Substring(idx + 1), NumberStyles.None, CultureInfo.InvariantCulture, out n);
        }
    }

}
