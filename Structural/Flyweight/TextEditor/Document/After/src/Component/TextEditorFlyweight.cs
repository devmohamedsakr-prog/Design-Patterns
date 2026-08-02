using System;
using System.Collections.Generic;

namespace Flyweight.TextEditor.Document.Component
{
    // Intrinsic State: Shared, immutable character style
    public class CharacterStyle
    {
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public string Color { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderline { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not CharacterStyle cs) return false;
            return FontFamily == cs.FontFamily && FontSize == cs.FontSize && Color == cs.Color 
                && IsBold == cs.IsBold && IsItalic == cs.IsItalic && IsUnderline == cs.IsUnderline;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FontFamily, FontSize, Color, IsBold, IsItalic, IsUnderline);
        }

        public override string ToString() => $"[{FontFamily} {FontSize}pt {Color}]";
    }

    // Flyweight Factory: Creates and caches CharacterStyle flyweights
    public class CharacterStyleFactory
    {
        private Dictionary<string, CharacterStyle> _stylePool = new();

        public CharacterStyle GetStyle(string fontFamily, int fontSize, string color, bool bold = false, bool italic = false, bool underline = false)
        {
            var key = $"{fontFamily}_{fontSize}_{color}_{bold}_{italic}_{underline}";
            
            if (!_stylePool.ContainsKey(key))
            {
                _stylePool[key] = new CharacterStyle
                {
                    FontFamily = fontFamily,
                    FontSize = fontSize,
                    Color = color,
                    IsBold = bold,
                    IsItalic = italic,
                    IsUnderline = underline
                };
            }

            return _stylePool[key];
        }

        public int GetPoolSize() => _stylePool.Count;
        public IReadOnlyDictionary<string, CharacterStyle> GetPool() => _stylePool;
    }

    // Extrinsic State: Per-character unique data
    public class Character
    {
        public char Value { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public CharacterStyle Style { get; set; }

        public override string ToString() => $"'{Value}' @({Row},{Column}) {Style}";
    }

    // Text Document using Flyweight pattern
    public class TextDocument
    {
        private List<Character> _characters = new();
        private CharacterStyleFactory _styleFactory = new();

        public void InsertCharacter(char value, int row, int column, string fontFamily, int fontSize, string color, bool bold = false)
        {
            var style = _styleFactory.GetStyle(fontFamily, fontSize, color, bold);
            var character = new Character { Value = value, Row = row, Column = column, Style = style };
            _characters.Add(character);
        }

        public void InsertText(string text, int row, int startColumn, string fontFamily, int fontSize, string color)
        {
            for (int i = 0; i < text.Length; i++)
            {
                InsertCharacter(text[i], row, startColumn + i, fontFamily, fontSize, color);
            }
        }

        public int GetCharacterCount() => _characters.Count;
        public int GetUniqueStyleCount() => _styleFactory.GetPoolSize();
        public IReadOnlyList<Character> GetCharacters() => _characters;
        public long EstimateMemorySavings() => (long)_characters.Count * 200 - _styleFactory.GetPoolSize() * 200 - _characters.Count * 16;
    }
}
