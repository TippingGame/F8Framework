using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace F8Framework.Core
{
    public enum ScrambleMode
    {
        None,
        All,
        Uppercase,
        Lowercase,
        Numerals,
        Custom
    }

    public class StringTween : BaseTween
    {
        private string from = "";
        private string to = "";
        private string changeValue = "";
        private string originalTo = "";
        private string originalFrom = "";
        private int incrementalLoopCount = 0;
        public bool richTextEnabled = false;
        public ScrambleMode scrambleMode = ScrambleMode.None;
        public string scrambleChars = null;
        public int startValueStrippedLength = 0;
        public int changeValueStrippedLength = 0;

        private static readonly StringBuilder _buffer = new StringBuilder(256);
        private static readonly List<TagInfo> _openedTags = new List<TagInfo>(8);
        
        private struct TagInfo
        {
            public int StartIndex;
            public int EndIndex;
            public bool IsClosing;
            public char TagChar;
            public string FullTag;
        }

        public StringTween(string from, string to, float duration, bool richTextEnabled, ScrambleMode scrambleMode, string scrambleChars, int id)
        {
            this.id = id;
            Init(from, to, duration, richTextEnabled, scrambleMode, scrambleChars);
        }

        internal void Init(string from, string to, float t, bool richTextEnabled, ScrambleMode scrambleMode, string scrambleChars)
        {
            this.from = from ?? "";
            this.to = to ?? "";
            this.originalTo = to ?? "";
            this.originalFrom = from ?? "";
            this.duration = t;
            this.richTextEnabled = richTextEnabled;
            this.scrambleMode = scrambleMode;
            this.scrambleChars = scrambleChars;
            SetChangeValue();
            this.PauseReset = () => this.Init(from, to, t, richTextEnabled, scrambleMode, scrambleChars);
        }

        private void SetChangeValue()
        {
            changeValue = to;
            
            bool emptyStartValue = string.IsNullOrEmpty(from);
            bool emptyChangeValue = string.IsNullOrEmpty(changeValue);
            
            startValueStrippedLength = emptyStartValue ? 0 : StripHtmlTags(from);
            changeValueStrippedLength = emptyChangeValue ? 0 : StripHtmlTags(changeValue);
            
            CheckForOpenTags(ref startValueStrippedLength, from, emptyStartValue);
            CheckForOpenTags(ref changeValueStrippedLength, changeValue, emptyChangeValue);
        }

        private int StripHtmlTags(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            
            int count = 0;
            bool inTag = false;
            
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                
                if (c == '<')
                {
                    inTag = true;
                    continue;
                }
                
                if (c == '>')
                {
                    inTag = false;
                    continue;
                }
                
                if (!inTag)
                {
                    count++;
                }
            }
            
            return count;
        }

        private void CheckForOpenTags(ref int strippedLength, string value, bool isEmpty)
        {
            if (isEmpty || value.Length <= 3) return;
            
            int lastOpenBracket = value.LastIndexOf('<');
            if (lastOpenBracket >= 0 && lastOpenBracket < value.Length - 2)
            {
                char nextChar = value[lastOpenBracket + 1];
                if (nextChar != '/')
                {
                    bool isClosed = false;
                    for (int i = lastOpenBracket + 1; i < value.Length; i++)
                    {
                        if (value[i] == '>')
                        {
                            isClosed = true;
                            break;
                        }
                    }
                    
                    if (!isClosed)
                    {
                        strippedLength++;
                    }
                }
            }
        }

        internal override void Update(float deltaTime)
        {
            if (isPause || IsComplete || IsRecycle)
                return;
                
            if (tempDelay > 0.0f)
            {
                tempDelay -= deltaTime;
                return;
            }
            
            base.Update(deltaTime);
            currentTime += deltaTime;
            
            if (currentTime >= duration)
            {
                this.UpdateValue(true);
                
                bool shouldComplete = !HandleLoop();
                if (shouldComplete)
                    onComplete?.Invoke();
                return;
            }
            
            this.UpdateValue(false);
        }

        internal override void UpdateValue(bool isEnd = false)
        {
            base.UpdateValue(isEnd);
            
            if (isEnd)
            {
                onUpdateString?.Invoke(loopType == LoopType.Yoyo ? from : to);
                return;
            }
            
            float normalizedProgress = currentTime >= duration ? 1.0f : currentTime / duration;
            float curveProgress = GetCurveProgress(normalizedProgress);
            
            string value = EvaluateString(curveProgress);
            onUpdateString?.Invoke(value);
        }

        private string EvaluateString(float progress)
        {
            _buffer.Clear();
            
            int startValueLen = richTextEnabled ? startValueStrippedLength
                : string.IsNullOrEmpty(from) ? 0 : from.Length;
            int changeValueLen = richTextEnabled ? changeValueStrippedLength
                : changeValue.Length;
            
            // 计算当前应该显示的长度
            int len = (int)Math.Round(changeValueLen * progress);
            if (len > changeValueLen) len = changeValueLen;
            else if (len < 0) len = 0;
            
            // 处理打乱字符模式
            if (scrambleMode != ScrambleMode.None)
            {
                AppendString(changeValue, 0, len, richTextEnabled);
                AppendScrambledChars(changeValueLen - len, GetScrambleChars());
                return _buffer.ToString();
            }
            
            // Incremental模式下的特殊处理
            if (loopType == LoopType.Incremental && incrementalLoopCount > 0)
            {
                // 在Incremental模式下，从from字符串开始，然后在from后面添加新的字符
                AppendString(from, 0, from.Length, richTextEnabled);
                
                // 然后，根据进度添加原始字符串的部分
                int originalDisplayLength = richTextEnabled ? StripHtmlTags(originalTo) : originalTo.Length;
                int charsToAdd = (int)Math.Round(originalDisplayLength * progress);
                if (charsToAdd > originalDisplayLength) charsToAdd = originalDisplayLength;
                else if (charsToAdd < 0) charsToAdd = 0;
                
                // 添加原始字符串的前charsToAdd个字符
                AppendString(originalTo, 0, charsToAdd, richTextEnabled);
                
                return _buffer.ToString();
            }
            
            // 正常模式
            if (changeValueLen == 0 && startValueLen > 0)
            {
                int fromCharsToShow = (int)(startValueLen * (1 - progress));
                if (fromCharsToShow > 0)
                {
                    AppendString(from, startValueLen - fromCharsToShow, fromCharsToShow, richTextEnabled);
                }
            }
            else if (startValueLen > changeValueLen)
            {
                AppendString(changeValue, 0, len, richTextEnabled);
                
                int fromRemaining = startValueLen - len;
                if (fromRemaining > 0)
                {
                    AppendString(from, len, fromRemaining, richTextEnabled);
                }
            }
            else
            {
                AppendString(changeValue, 0, len, richTextEnabled);
                
                int diff = startValueLen - changeValueLen;
                int startValueMaxLen = startValueLen;
                if (diff > 0)
                {
                    float perc = (float)len / changeValueLen;
                    startValueMaxLen -= (int)(startValueMaxLen * perc);
                }
                else
                {
                    startValueMaxLen -= len;
                }
                
                if (len < changeValueLen && len < startValueLen)
                {
                    AppendString(from, len,
                        richTextEnabled ? len + startValueMaxLen : startValueMaxLen,
                        richTextEnabled);
                }
            }
            
            return _buffer.ToString();
        }

        private void AppendString(string value, int startIndex, int length, bool richTextEnabled)
        {
            if (!richTextEnabled || string.IsNullOrEmpty(value))
            {
                if (startIndex >= 0 && startIndex + length <= value.Length)
                {
                    _buffer.Append(value, startIndex, length);
                }
                return;
            }
            
            _openedTags.Clear();
            int fullLen = value.Length;
            int visibleChars = 0;
            int i = 0;
            
            for (; i < fullLen && visibleChars < startIndex + length; i++)
            {
                char c = value[i];
                
                if (c == '<')
                {
                    ParseHtmlTag(value, i, fullLen, out TagInfo tagInfo);
                    
                    if (tagInfo.EndIndex > i)
                    {
                        if (visibleChars >= startIndex || i + tagInfo.EndIndex - i + 1 <= startIndex + length)
                        {
                            _buffer.Append(value, i, tagInfo.EndIndex - i + 1);
                            
                            if (!tagInfo.IsClosing)
                            {
                                _openedTags.Add(tagInfo);
                            }
                            else if (_openedTags.Count > 0)
                            {
                                for (int j = _openedTags.Count - 1; j >= 0; j--)
                                {
                                    if (_openedTags[j].TagChar == tagInfo.TagChar)
                                    {
                                        _openedTags.RemoveAt(j);
                                        break;
                                    }
                                }
                            }
                        }
                        
                        i = tagInfo.EndIndex;
                    }
                    else
                    {
                        if (visibleChars >= startIndex && visibleChars < startIndex + length)
                        {
                            _buffer.Append(c);
                            visibleChars++;
                        }
                    }
                }
                else if (visibleChars >= startIndex && visibleChars < startIndex + length)
                {
                    _buffer.Append(c);
                    visibleChars++;
                }
                else if (visibleChars < startIndex)
                {
                    visibleChars++;
                }
            }
            
            CloseAllOpenedTags();
        }

        private void ParseHtmlTag(string value, int start, int fullLen, out TagInfo tagInfo)
        {
            tagInfo = new TagInfo { StartIndex = start };
            
            if (start + 1 >= fullLen)
            {
                tagInfo.EndIndex = start;
                return;
            }
            
            char nextChar = value[start + 1];
            tagInfo.IsClosing = (nextChar == '/');
            
            int tagEnd = value.IndexOf('>', start);
            if (tagEnd == -1)
            {
                tagInfo.EndIndex = start;
                return;
            }
            
            tagInfo.EndIndex = tagEnd;
            tagInfo.FullTag = value.Substring(start, tagEnd - start + 1);
            
            int tagCharIndex = start + (tagInfo.IsClosing ? 2 : 1);
            if (tagCharIndex < fullLen)
            {
                char tagChar = value[tagCharIndex];
                
                if (tagChar == 'c' && tagCharIndex + 5 < fullLen && 
                    value[tagCharIndex + 1] == 'o' && value[tagCharIndex + 2] == 'l' &&
                    value[tagCharIndex + 3] == 'o' && value[tagCharIndex + 4] == 'r')
                {
                    tagInfo.TagChar = 'c';
                }
                else if (tagChar == '#' && tagCharIndex + 1 < fullLen)
                {
                    tagInfo.TagChar = 'c';
                }
                else if (tagChar == 'b' || tagChar == 'i' || tagChar == 's' || 
                         tagChar == 'q' || tagChar == 'm' || tagChar == 'n')
                {
                    tagInfo.TagChar = tagChar;
                }
                else
                {
                    tagInfo.TagChar = '?';
                }
            }
        }

        private void CloseAllOpenedTags()
        {
            for (int i = _openedTags.Count - 1; i >= 0; i--)
            {
                char tagChar = _openedTags[i].TagChar;
                
                switch (tagChar)
                {
                    case 'b':
                        _buffer.Append("</b>");
                        break;
                    case 'i':
                        _buffer.Append("</i>");
                        break;
                    case 'c':
                        _buffer.Append("</color>");
                        break;
                    case 's':
                        _buffer.Append("</size>");
                        break;
                    case 'q':
                        _buffer.Append("</quad>");
                        break;
                    case 'm':
                        _buffer.Append("</material>");
                        break;
                    case '?':
                        _buffer.Append("</>");
                        break;
                    default:
                        _buffer.Append("</");
                        _buffer.Append(tagChar);
                        _buffer.Append(">");
                        break;
                }
            }
            
            _openedTags.Clear();
        }

        private char[] GetScrambleChars()
        {
            switch (scrambleMode)
            {
                case ScrambleMode.Uppercase:
                    return StringTweenExtensions.ScrambledCharsUppercase;
                case ScrambleMode.Lowercase:
                    return StringTweenExtensions.ScrambledCharsLowercase;
                case ScrambleMode.Numerals:
                    return StringTweenExtensions.ScrambledCharsNumerals;
                case ScrambleMode.Custom:
                    return scrambleChars.IsNullOrEmpty() ? StringTweenExtensions.ScrambledCharsAll 
                        : scrambleChars.ToCharArray();
                default:
                    return StringTweenExtensions.ScrambledCharsAll;
            }
        }

        private void AppendScrambledChars(int length, char[] chars)
        {
            if (length <= 0 || chars == null || chars.Length == 0)
                return;
                
            int len = chars.Length;
            for (int i = 0; i < length; ++i)
            {
                _buffer.Append(chars[UnityEngine.Random.Range(0, len)]);
            }
        }
        
        private float GetCurveProgress(float normalizedProgress)
        {
            switch (loopType)
            {
                case LoopType.Yoyo:
                    return Mathf.PingPong(normalizedProgress * 2, 1);
                default:
                    return normalizedProgress;
            }
        }
        
        private bool HandleLoop()
        {
            if (loopType == LoopType.None || tempLoopCount == 0)
            {
                return false;
            }
            else
            {
                if (tempLoopCount > 0)
                {
                    tempLoopCount -= 1;
                }
                
                switch (loopType)
                {
                    case LoopType.Restart:
                        break;
                    case LoopType.Flip:
                        (from, to) = (to, from);
                        break;
                    case LoopType.Incremental:
                        incrementalLoopCount++;
                        from = to;
                        to += originalTo;
                        break;
                    case LoopType.Yoyo:
                        break;
                }
                this.LoopReset();
                return tempLoopCount > 0 || tempLoopCount == -1;
            }
        }
        
        internal override void Reset()
        {
            base.Reset();
            from = "";
            to = "";
            changeValue = "";
            originalTo = "";
            originalFrom = "";
            incrementalLoopCount = 0;
            richTextEnabled = false;
            scrambleMode = ScrambleMode.None;
            scrambleChars = null;
            startValueStrippedLength = 0;
            changeValueStrippedLength = 0;
            
            _buffer.Clear();
            _openedTags.Clear();
        }
        
        public override BaseTween ReplayReset()
        {
            base.ReplayReset();
            
            to = originalTo;
            from = originalFrom;
            incrementalLoopCount = 0;
            
            SetChangeValue();
            return this;
        }
        
        public override BaseTween LoopReset()
        {
            base.LoopReset();
            SetChangeValue();
            return this;
        }
    }
    
    internal static class StringTweenExtensions
    {
        public static readonly char[] ScrambledCharsAll;
        public static readonly char[] ScrambledCharsUppercase;
        public static readonly char[] ScrambledCharsLowercase;
        public static readonly char[] ScrambledCharsNumerals;
        
        static StringTweenExtensions()
        {
            ScrambledCharsAll = new char[62];
            ScrambledCharsUppercase = new char[26];
            ScrambledCharsLowercase = new char[26];
            ScrambledCharsNumerals = new char[10];
            
            for (int i = 0; i < 26; i++)
            {
                ScrambledCharsAll[i] = (char)('A' + i);
                ScrambledCharsUppercase[i] = (char)('A' + i);
                ScrambledCharsAll[i + 26] = (char)('a' + i);
                ScrambledCharsLowercase[i] = (char)('a' + i);
            }
            
            for (int i = 0; i < 10; i++)
            {
                ScrambledCharsAll[i + 52] = (char)('0' + i);
                ScrambledCharsNumerals[i] = (char)('0' + i);
            }
            
            ScrambleChars(ScrambledCharsAll);
            ScrambleChars(ScrambledCharsUppercase);
            ScrambleChars(ScrambledCharsLowercase);
            ScrambleChars(ScrambledCharsNumerals);
        }
        
        private static void ScrambleChars(char[] chars)
        {
            int n = chars.Length;
            for (int i = n - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }
        }
    }
}