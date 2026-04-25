using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace F8Framework.Core
{
    [AddComponentMenu("F8Framework/UI/TextExtension")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public sealed class TextExtension : BaseMeshEffect, IPointerClickHandler
    {
        public enum GradientMode
        {
            HorizontalTwoColor,
            HorizontalThreeColor,
            VerticalTwoColor,
            VerticalThreeColor,
            FourCorner
        }

        public enum ColorBlendMode
        {
            Multiply,
            Override,
            Additive
        }

        [Serializable]
        public sealed class HyperlinkClickEvent : UnityEvent<string>
        {
        }

        [Serializable]
        private sealed class LinkInfo
        {
            public string Id;
            public int CharStart;
            public int CharLength;
            public readonly List<Rect> Boxes = new List<Rect>();

            public bool ContainsChar(int charIndex)
            {
                return charIndex >= CharStart && charIndex < CharStart + CharLength;
            }

            public bool ContainsLocalPoint(Vector2 localPoint)
            {
                for (int i = 0; i < Boxes.Count; i++)
                {
                    if (Boxes[i].Contains(localPoint))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private static readonly Regex LinkRegex = new Regex(
            @"(?is)<link=(?<id>.*?)>(?<text>.*?)</link>|\[link=(?<id2>.*?)\](?<text2>.*?)\[/link\]",
            RegexOptions.Compiled);

        private static readonly List<UIVertex> SharedVertices = new List<UIVertex>(256);
        private static readonly List<UIVertex> SharedBaseVertices = new List<UIVertex>(256);
        private static readonly List<int> SharedLineStartCharIndices = new List<int>(16);
        private static readonly List<int> SharedLineVisibleCharCounts = new List<int>(16);
        
        [SerializeField] private Text _text;
        
        [SerializeField] private float _characterSpacing;
        [SerializeField] private float _lineSpacing;
        
        [SerializeField] private bool _enableVertexGradient;
        [SerializeField] private GradientMode _gradientMode = GradientMode.FourCorner;
        [SerializeField] private ColorBlendMode _gradientBlendMode = ColorBlendMode.Multiply;
        [SerializeField] private Color _topLeft = Color.white;
        [SerializeField] private Color _topRight = Color.white;
        [SerializeField] private Color _bottomLeft = Color.white;
        [SerializeField] private Color _bottomRight = Color.white;
        [SerializeField] private Color _startColor = Color.white;
        [SerializeField] private Color _middleColor = Color.white;
        [SerializeField] private Color _endColor = Color.white;
        [SerializeField, Range(-1f, 1f)] private float _gradientOffsetX;
        [SerializeField, Range(-1f, 1f)] private float _gradientOffsetY;
        [SerializeField, Range(0.05f, 0.95f)] private float _threeColorPivot = 0.5f;
        
        [SerializeField] private bool _enableShadow;
        [SerializeField] private Color _shadowColor = Color.black;
        [SerializeField] private Vector2 _shadowOffset = new Vector2(1f, -1f);
        [SerializeField] private bool _enableSoftShadow;
        [SerializeField] private Color _softShadowColor = Color.black;
        [SerializeField] private Vector2 _softShadowOffset = new Vector2(1f, -1f);
        [SerializeField] private Vector2 _softShadowBlur = new Vector2(1.5f, 1.5f);
        [SerializeField, Range(4, 24)] private int _softShadowSamples = 8;
        
        [SerializeField] private bool _enableOutline;
        [SerializeField] private Color _outlineColor = Color.black;
        [SerializeField] private Vector2 _outlineDistance = new Vector2(1f, 1f);
        [SerializeField] private bool _enableSoftOutline;
        [SerializeField] private Color _softOutlineColor = Color.black;
        [SerializeField] private Vector2 _softOutlineDistance = new Vector2(1.5f, 1.5f);
        [SerializeField, Range(4, 24)] private int _softOutlineSamples = 8;
        
        [SerializeField] private bool _enableHyperlink;
        [SerializeField] private bool _tintHyperlink;
        [SerializeField] private Color _hyperlinkColor = new Color(0.2352941f, 0.5803922f, 1f, 1f);
        [SerializeField] private bool _openUrlOnClick;
        [SerializeField] private HyperlinkClickEvent _onHyperlinkClick = new HyperlinkClickEvent();

        private readonly List<LinkInfo> _links = new List<LinkInfo>();
        private readonly List<LinkInfo> _parsedLinksBuffer = new List<LinkInfo>();
        private readonly StringBuilder _renderedTextBuilder = new StringBuilder(256);
        [TextArea(3, 8)]
        [SerializeField] private string _sourceText = string.Empty;
        private string _renderedText = string.Empty;
        private bool _isApplyingProcessedText;
        private bool _hasInitializedSourceText;

        public HyperlinkClickEvent OnHyperlinkClick => _onHyperlinkClick;

        protected override void Awake()
        {
            base.Awake();
            CacheReferences();
            CaptureInitialText();
            RefreshRenderedText(force: true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            CacheReferences();
            RefreshRenderedText(force: true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (ShouldRestoreSourceText(Application.isPlaying))
            {
                RestoreSourceTextIfNeeded();
            }
        }

        protected override void OnDestroy()
        {
            if (ShouldRestoreSourceText(Application.isPlaying))
            {
                RestoreSourceTextIfNeeded();
            }

            base.OnDestroy();
        }

        private void LateUpdate()
        {
            CacheReferences();
            if (_text == null || _isApplyingProcessedText)
            {
                return;
            }

            string currentText = _text.text ?? string.Empty;
            if (currentText != _renderedText)
            {
                _sourceText = currentText;
                RefreshRenderedText(force: true);
            }
            else if (!_enableHyperlink && currentText != _sourceText)
            {
                _sourceText = currentText;
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            CacheReferences();
            if (_text == null || vh.currentVertCount == 0)
            {
                return;
            }

            SharedVertices.Clear();
            vh.GetUIVertexStream(SharedVertices);
            int baseVertexCount = SharedVertices.Count;
            if (baseVertexCount == 0)
            {
                return;
            }

            if (!Mathf.Approximately(_characterSpacing, 0f) || !Mathf.Approximately(_lineSpacing, 0f))
            {
                ApplySpacing(SharedVertices, 0, baseVertexCount);
            }

            if (_enableVertexGradient)
            {
                ApplyGradient(SharedVertices, 0, baseVertexCount);
            }

            if (_enableHyperlink && _tintHyperlink && _links.Count > 0)
            {
                ApplyHyperlinkTint(SharedVertices, 0, baseVertexCount);
            }

            BuildLinkBoxes(SharedVertices, baseVertexCount);

            float bestFitScale = GetBestFitScale();
            SharedBaseVertices.Clear();
            SharedBaseVertices.AddRange(SharedVertices);
            SharedVertices.Clear();

            if (_enableSoftShadow)
            {
                AppendSoftEffectCopies(
                    SharedVertices,
                    SharedBaseVertices,
                    0,
                    baseVertexCount,
                    _softShadowOffset * bestFitScale,
                    _softShadowBlur * bestFitScale,
                    _softShadowColor,
                    _softShadowSamples);
            }

            if (_enableShadow)
            {
                AppendEffectCopies(SharedVertices, SharedBaseVertices, 0, baseVertexCount, _shadowOffset * bestFitScale, _shadowColor);
            }

            if (_enableSoftOutline)
            {
                AppendSoftEffectCopies(
                    SharedVertices,
                    SharedBaseVertices,
                    0,
                    baseVertexCount,
                    Vector2.zero,
                    _softOutlineDistance * bestFitScale,
                    _softOutlineColor,
                    _softOutlineSamples);
            }

            if (_enableOutline)
            {
                Vector2 scaledOutlineDistance = _outlineDistance * bestFitScale;
                AppendEffectCopies(SharedVertices, SharedBaseVertices, 0, baseVertexCount, new Vector2(scaledOutlineDistance.x, 0f), _outlineColor);
                AppendEffectCopies(SharedVertices, SharedBaseVertices, 0, baseVertexCount, new Vector2(-scaledOutlineDistance.x, 0f), _outlineColor);
                AppendEffectCopies(SharedVertices, SharedBaseVertices, 0, baseVertexCount, new Vector2(0f, scaledOutlineDistance.y), _outlineColor);
                AppendEffectCopies(SharedVertices, SharedBaseVertices, 0, baseVertexCount, new Vector2(0f, -scaledOutlineDistance.y), _outlineColor);
            }

            SharedVertices.AddRange(SharedBaseVertices);

            vh.Clear();
            vh.AddUIVertexTriangleStream(SharedVertices);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_enableHyperlink || _links.Count == 0 || _text == null)
            {
                return;
            }

            RectTransform rectTransform = _text.rectTransform;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out Vector2 localPoint))
            {
                return;
            }

            for (int i = 0; i < _links.Count; i++)
            {
                LinkInfo link = _links[i];
                if (!link.ContainsLocalPoint(localPoint))
                {
                    continue;
                }

                _onHyperlinkClick.Invoke(link.Id);
                if (_openUrlOnClick)
                {
                    Application.OpenURL(link.Id);
                }

                return;
            }
        }

        public void SetSourceText(string sourceText)
        {
            _sourceText = sourceText ?? string.Empty;
            RefreshRenderedText(force: true);
        }

        private void CacheReferences()
        {
            _text ??= GetComponent<Text>();
        }

        private void CaptureInitialText()
        {
            if (_text == null)
            {
                return;
            }

            if (!_hasInitializedSourceText)
            {
                if (string.IsNullOrEmpty(_sourceText))
                {
                    _sourceText = _text.text ?? string.Empty;
                }

                _hasInitializedSourceText = true;
            }

            _renderedText = _text.text ?? string.Empty;
        }

        private void RefreshRenderedText(bool force)
        {
            if (_text == null)
            {
                return;
            }

            if (!_enableHyperlink)
            {
                _links.Clear();
                if (force && _text.text != _sourceText)
                {
                    ApplyTextWithoutFeedback(_sourceText);
                }

                _renderedText = _text.text ?? string.Empty;
                graphic?.SetVerticesDirty();
                return;
            }

            string source = _sourceText ?? string.Empty;
            _parsedLinksBuffer.Clear();
            string rendered = BuildRenderedText(source, _parsedLinksBuffer);
            _links.Clear();
            _links.AddRange(_parsedLinksBuffer);

            if (force || _text.text != rendered)
            {
                ApplyTextWithoutFeedback(rendered);
            }

            _renderedText = rendered;
            graphic?.SetVerticesDirty();
        }

        private void RestoreSourceTextIfNeeded()
        {
            if (_text == null || _isApplyingProcessedText || !_enableHyperlink)
            {
                return;
            }

            if (_text.text == _renderedText && _sourceText != _renderedText)
            {
                ApplyTextWithoutFeedback(_sourceText);
            }
        }

        private void ApplyTextWithoutFeedback(string value)
        {
            if (_text == null)
            {
                return;
            }

            _isApplyingProcessedText = true;
            _text.text = value ?? string.Empty;
            _isApplyingProcessedText = false;
        }

        private string BuildRenderedText(string source, List<LinkInfo> parsedLinks)
        {
            _renderedTextBuilder.Clear();
            if (_renderedTextBuilder.Capacity < source.Length)
            {
                _renderedTextBuilder.Capacity = source.Length;
            }

            int previousIndex = 0;
            MatchCollection matches = LinkRegex.Matches(source);
            for (int i = 0; i < matches.Count; i++)
            {
                Match match = matches[i];
                if (!match.Success)
                {
                    continue;
                }

                string plainSegment = source.Substring(previousIndex, match.Index - previousIndex);
                _renderedTextBuilder.Append(plainSegment);

                string id = GetGroupValue(match, "id");
                if (string.IsNullOrEmpty(id))
                {
                    id = GetGroupValue(match, "id2");
                }

                string displayText = GetGroupValue(match, "text");
                if (string.IsNullOrEmpty(displayText))
                {
                    displayText = GetGroupValue(match, "text2");
                }

                int linkCharStart = _renderedTextBuilder.Length;
                _renderedTextBuilder.Append(displayText);
                parsedLinks.Add(new LinkInfo
                {
                    Id = id,
                    CharStart = linkCharStart,
                    CharLength = _renderedTextBuilder.Length - linkCharStart
                });
                previousIndex = match.Index + match.Length;
            }

            if (previousIndex < source.Length)
            {
                _renderedTextBuilder.Append(source, previousIndex, source.Length - previousIndex);
            }

            return _renderedTextBuilder.ToString();
        }

        private static string GetGroupValue(Match match, string groupName)
        {
            Group group = match.Groups[groupName];
            return group.Success ? group.Value : string.Empty;
        }

        private void ApplySpacing(List<UIVertex> verts, int start, int end)
        {
            if (_text == null || _text.cachedTextGenerator == null)
            {
                return;
            }

            BuildLineCharData();
            if (SharedLineStartCharIndices.Count == 0)
            {
                return;
            }

            float horizontalAlignmentFactor = GetHorizontalAlignmentFactor();
            float verticalAlignmentFactor = GetVerticalAlignmentFactor();
            float totalVerticalSpacing = Mathf.Max(0, SharedLineStartCharIndices.Count - 1) * _lineSpacing;
            int lineIndex = 0;
            int charSlotInLine = 0;
            int quadIndex = 0;
            bool insideTag = false;
            string renderedText = _text.text ?? string.Empty;

            for (int charIndex = 0; charIndex < renderedText.Length && start + quadIndex * 6 + 5 < end; charIndex++)
            {
                while (lineIndex + 1 < SharedLineStartCharIndices.Count && charIndex >= SharedLineStartCharIndices[lineIndex + 1])
                {
                    lineIndex++;
                    charSlotInLine = 0;
                }

                char c = renderedText[charIndex];
                if (c == '<')
                {
                    insideTag = true;
                    continue;
                }

                if (insideTag)
                {
                    if (c == '>')
                    {
                        insideTag = false;
                    }

                    continue;
                }

                if (c == '\n' || c == '\r')
                {
                    continue;
                }

                float totalCharacterSpacing = Mathf.Max(0, SharedLineVisibleCharCounts[lineIndex] - 1) * _characterSpacing;
                float baseOffsetX = -totalCharacterSpacing * horizontalAlignmentFactor;
                float baseOffsetY = totalVerticalSpacing * verticalAlignmentFactor - lineIndex * _lineSpacing;
                Vector3 glyphOffset = new Vector3(baseOffsetX + charSlotInLine * _characterSpacing, baseOffsetY, 0f);

                if (CharacterGeneratesQuad(c))
                {
                    int glyphStartVertex = start + quadIndex * 6;
                    OffsetGlyph(verts, glyphStartVertex, glyphOffset);
                    charSlotInLine++;
                    quadIndex++;
                }
            }
        }

        private void ApplyGradient(List<UIVertex> verts, int start, int end)
        {
            Rect rect = _text.rectTransform.rect;
            Vector2 min = -Vector2.Scale(rect.size, _text.rectTransform.pivot);
            Vector2 max = min + rect.size;

            for (int i = start; i < end; i++)
            {
                UIVertex vertex = verts[i];
                Color gradientColor = EvaluateGradient(vertex.position, min, max);
                vertex.color = BlendColor(vertex.color, gradientColor, _gradientBlendMode);
                verts[i] = vertex;
            }
        }

        private void ApplyHyperlinkTint(List<UIVertex> verts, int start, int end)
        {
            if (_links.Count == 0)
            {
                return;
            }

            ForEachLinkedGlyph(verts, start, end, (link, glyphStartVertex) =>
            {
                for (int j = 0; j < 6; j++)
                {
                    UIVertex vertex = verts[glyphStartVertex + j];
                    vertex.color = BlendColor(vertex.color, _hyperlinkColor, ColorBlendMode.Override);
                    verts[glyphStartVertex + j] = vertex;
                }
            });
        }

        private void BuildLinkBoxes(List<UIVertex> verts, int baseVertexCount)
        {
            for (int i = 0; i < _links.Count; i++)
            {
                _links[i].Boxes.Clear();
            }

            if (!_enableHyperlink || _links.Count == 0)
            {
                return;
            }

            ForEachLinkedGlyph(verts, 0, baseVertexCount, (link, glyphStartVertex) =>
            {
                Rect glyphRect = GetGlyphRect(verts, glyphStartVertex);
                MergeBox(link.Boxes, glyphRect);
            });
        }

        private void ForEachLinkedGlyph(List<UIVertex> verts, int start, int end, Action<LinkInfo, int> action)
        {
            if (action == null || _text == null || _links.Count == 0)
            {
                return;
            }

            string renderedText = _text.text ?? string.Empty;
            bool insideTag = false;
            int quadIndex = 0;
            int linkIndex = 0;
            LinkInfo currentLink = _links[0];

            for (int charIndex = 0; charIndex < renderedText.Length && start + quadIndex * 6 + 5 < end; charIndex++)
            {
                char c = renderedText[charIndex];
                if (c == '<')
                {
                    insideTag = true;
                    continue;
                }

                if (insideTag)
                {
                    if (c == '>')
                    {
                        insideTag = false;
                    }

                    continue;
                }

                if (!CharacterGeneratesQuad(c))
                {
                    continue;
                }

                while (currentLink != null && charIndex >= currentLink.CharStart + currentLink.CharLength)
                {
                    linkIndex++;
                    currentLink = linkIndex < _links.Count ? _links[linkIndex] : null;
                }

                if (currentLink != null && charIndex >= currentLink.CharStart)
                {
                    action(currentLink, start + quadIndex * 6);
                }

                quadIndex++;
            }
        }

        private void BuildLineCharData()
        {
            SharedLineStartCharIndices.Clear();
            SharedLineVisibleCharCounts.Clear();

            TextGenerator generator = _text.cachedTextGenerator;
            IList<UILineInfo> lines = generator.lines;
            if (lines == null || lines.Count == 0)
            {
                return;
            }

            string renderedText = _text.text ?? string.Empty;
            for (int i = 0; i < lines.Count; i++)
            {
                SharedLineStartCharIndices.Add(lines[i].startCharIdx);
            }

            for (int i = 0; i < SharedLineStartCharIndices.Count; i++)
            {
                int startCharIndex = SharedLineStartCharIndices[i];
                int endCharIndex = i + 1 < SharedLineStartCharIndices.Count
                    ? SharedLineStartCharIndices[i + 1]
                    : renderedText.Length;

                SharedLineVisibleCharCounts.Add(CountVisibleCharactersInRange(renderedText, startCharIndex, endCharIndex));
            }
        }

        private static int CountVisibleCharactersInRange(string text, int startIndex, int endIndex)
        {
            int count = 0;
            bool insideTag = false;

            for (int i = startIndex; i < endIndex && i < text.Length; i++)
            {
                char c = text[i];
                if (c == '<')
                {
                    insideTag = true;
                    continue;
                }

                if (insideTag)
                {
                    if (c == '>')
                    {
                        insideTag = false;
                    }

                    continue;
                }

                if (c == '\n' || c == '\r')
                {
                    continue;
                }

                if (CharacterGeneratesQuad(c))
                {
                    count++;
                }
            }

            return count;
        }

        private static Rect GetGlyphRect(List<UIVertex> verts, int startIndex)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            for (int i = 0; i < 6; i++)
            {
                Vector3 position = verts[startIndex + i].position;
                minX = Mathf.Min(minX, position.x);
                minY = Mathf.Min(minY, position.y);
                maxX = Mathf.Max(maxX, position.x);
                maxY = Mathf.Max(maxY, position.y);
            }

            return Rect.MinMaxRect(minX, minY, maxX, maxY);
        }

        private static void MergeBox(List<Rect> boxes, Rect glyphRect)
        {
            const float lineTolerance = 2f;

            for (int i = 0; i < boxes.Count; i++)
            {
                Rect existing = boxes[i];
                bool sameLine = Mathf.Abs(existing.center.y - glyphRect.center.y) <= lineTolerance;
                if (!sameLine)
                {
                    continue;
                }

                boxes[i] = Rect.MinMaxRect(
                    Mathf.Min(existing.xMin, glyphRect.xMin),
                    Mathf.Min(existing.yMin, glyphRect.yMin),
                    Mathf.Max(existing.xMax, glyphRect.xMax),
                    Mathf.Max(existing.yMax, glyphRect.yMax));
                return;
            }

            boxes.Add(glyphRect);
        }

        private static void OffsetGlyph(List<UIVertex> verts, int startIndex, Vector3 offset)
        {
            for (int i = 0; i < 6 && startIndex + i < verts.Count; i++)
            {
                UIVertex vertex = verts[startIndex + i];
                vertex.position += offset;
                verts[startIndex + i] = vertex;
            }
        }

        private static bool CharacterGeneratesQuad(char c)
        {
            return !char.IsWhiteSpace(c) && !char.IsControl(c);
        }

        private static bool ShouldRestoreSourceText(bool isPlaying)
        {
            return !isPlaying;
        }


        private static void AppendEffectCopies(List<UIVertex> target, List<UIVertex> source, int start, int end, Vector2 offset, Color color)
        {
            int requiredCapacity = target.Count + (end - start);
            if (target.Capacity < requiredCapacity)
            {
                target.Capacity = requiredCapacity;
            }

            for (int i = start; i < end; i++)
            {
                UIVertex vertex = source[i];
                Vector3 position = vertex.position;
                position.x += offset.x;
                position.y += offset.y;
                vertex.position = position;

                Color32 resultColor = color;
                resultColor.a = (byte)(resultColor.a * source[i].color.a / 255);
                vertex.color = resultColor;
                target.Add(vertex);
            }
        }

        private static void AppendSoftEffectCopies(
            List<UIVertex> target,
            List<UIVertex> source,
            int start,
            int end,
            Vector2 centerOffset,
            Vector2 blurDistance,
            Color color,
            int sampleCount)
        {
            int clampedSamples = Mathf.Max(1, sampleCount);
            Color sampleColor = color;
            sampleColor.a = Mathf.Clamp01(color.a / Mathf.Sqrt(clampedSamples));

            for (int sampleIndex = 0; sampleIndex < clampedSamples; sampleIndex++)
            {
                float angle = Mathf.PI * 2f * sampleIndex / clampedSamples;
                Vector2 radialOffset = new Vector2(Mathf.Cos(angle) * blurDistance.x, Mathf.Sin(angle) * blurDistance.y);
                AppendEffectCopies(target, source, start, end, centerOffset + radialOffset, sampleColor);
            }
        }

        private Color EvaluateGradient(Vector2 position, Vector2 min, Vector2 max)
        {
            float x01 = Mathf.Approximately(max.x, min.x) ? 0f : Mathf.Clamp01((position.x - min.x) / (max.x - min.x));
            float y01 = Mathf.Approximately(max.y, min.y) ? 0f : Mathf.Clamp01((position.y - min.y) / (max.y - min.y));

            x01 = ApplyGradientOffset(x01, _gradientOffsetX);
            y01 = ApplyGradientOffset(y01, _gradientOffsetY);

            switch (_gradientMode)
            {
                case GradientMode.HorizontalTwoColor:
                    return Color.Lerp(_startColor, _endColor, x01);
                case GradientMode.HorizontalThreeColor:
                    if (x01 <= _threeColorPivot)
                    {
                        float leftT = Mathf.InverseLerp(0f, Mathf.Max(0.0001f, _threeColorPivot), x01);
                        return Color.Lerp(_startColor, _middleColor, leftT);
                    }

                    float rightT = Mathf.InverseLerp(_threeColorPivot, 1f, x01);
                    return Color.Lerp(_middleColor, _endColor, rightT);
                case GradientMode.VerticalTwoColor:
                    return Color.Lerp(_endColor, _startColor, y01);
                case GradientMode.VerticalThreeColor:
                    if (y01 <= _threeColorPivot)
                    {
                        float bottomT = Mathf.InverseLerp(0f, Mathf.Max(0.0001f, _threeColorPivot), y01);
                        return Color.Lerp(_endColor, _middleColor, bottomT);
                    }

                    float topT = Mathf.InverseLerp(_threeColorPivot, 1f, y01);
                    return Color.Lerp(_middleColor, _startColor, topT);
                case GradientMode.FourCorner:
                default:
                    Color bottom = Color.Lerp(_bottomLeft, _bottomRight, x01);
                    Color top = Color.Lerp(_topLeft, _topRight, x01);
                    return Color.Lerp(bottom, top, y01);
            }
        }

        private float GetBestFitScale()
        {
            if (_text == null || !_text.resizeTextForBestFit || _text.fontSize <= 0)
            {
                return 1f;
            }

            int bestFitFontSize = _text.cachedTextGenerator != null
                ? _text.cachedTextGenerator.fontSizeUsedForBestFit
                : 0;

            if (bestFitFontSize <= 0)
            {
                bestFitFontSize = _text.fontSize;
            }

            return Mathf.Max(0.01f, (float)bestFitFontSize / _text.fontSize);
        }

        private float GetHorizontalAlignmentFactor()
        {
            switch (_text.alignment)
            {
                case TextAnchor.LowerCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.UpperCenter:
                    return 0.5f;
                case TextAnchor.LowerRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.UpperRight:
                    return 1f;
                default:
                    return 0f;
            }
        }

        private float GetVerticalAlignmentFactor()
        {
            switch (_text.alignment)
            {
                case TextAnchor.MiddleLeft:
                case TextAnchor.MiddleCenter:
                case TextAnchor.MiddleRight:
                    return 0.5f;
                case TextAnchor.LowerLeft:
                case TextAnchor.LowerCenter:
                case TextAnchor.LowerRight:
                    return 1f;
                default:
                    return 0f;
            }
        }

        private static float ApplyGradientOffset(float normalized, float offset)
        {
            if (Mathf.Approximately(offset, 0f))
            {
                return normalized;
            }

            float biased = normalized - offset * (offset > 0f ? normalized : (1f - normalized));
            return Mathf.Clamp01(biased);
        }

        private static Color BlendColor(Color baseColor, Color effectColor, ColorBlendMode blendMode)
        {
            switch (blendMode)
            {
                case ColorBlendMode.Override:
                {
                    float alpha = Mathf.Max(baseColor.a, effectColor.a);
                    Color color = Color.Lerp(baseColor, effectColor, effectColor.a);
                    color.a = alpha;
                    return color;
                }
                case ColorBlendMode.Additive:
                    return baseColor + effectColor;
                case ColorBlendMode.Multiply:
                default:
                    return baseColor * effectColor;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            CacheReferences();
            _softShadowSamples = Mathf.Max(1, _softShadowSamples);
            _softOutlineSamples = Mathf.Max(1, _softOutlineSamples);

            if (!Application.isPlaying && _text != null)
            {
                string currentText = _text.text ?? string.Empty;

                if (!_enableHyperlink)
                {
                    _sourceText = currentText;
                }
                else if (LinkRegex.IsMatch(currentText))
                {
                    _sourceText = currentText;
                    _hasInitializedSourceText = true;
                }
                else if (string.IsNullOrEmpty(_sourceText))
                {
                    _sourceText = currentText;
                    _hasInitializedSourceText = true;
                }

                RefreshRenderedText(force: true);
            }
        }
#endif
    }
}
