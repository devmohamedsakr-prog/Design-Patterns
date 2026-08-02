using System;
using System.Collections.Generic;

namespace Decorator.Text.Formatting.Component
{
    public abstract class TextFormatter
    {
        public string Text { get; set; }

        public TextFormatter(string text = "")
        {
            Text = text;
        }

        public abstract string Format();
        public abstract int GetLength();
    }

    public class PlainTextFormatter : TextFormatter
    {
        public PlainTextFormatter(string text) : base(text) { }

        public override string Format() => Text;
        public override int GetLength() => Text.Length;
        public override string ToString() => $"PlainText({Text})";
    }

    public abstract class TextDecorator : TextFormatter
    {
        protected TextFormatter _formatter;

        public TextDecorator(TextFormatter formatter)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            Text = formatter.Text;
        }
    }

    public class EncryptionDecorator : TextDecorator
    {
        public string Algorithm { get; set; }
        private int _shift = 3; // Caesar cipher shift

        public EncryptionDecorator(TextFormatter formatter) : base(formatter)
        {
            Algorithm = "Caesar";
        }

        public override string Format()
        {
            var encrypted = "";
            foreach (char c in _formatter.Format())
            {
                if (char.IsLetter(c))
                {
                    char baseChar = char.IsUpper(c) ? 'A' : 'a';
                    encrypted += (char)(baseChar + (c - baseChar + _shift) % 26);
                }
                else
                {
                    encrypted += c;
                }
            }
            return $"[{Algorithm}] {encrypted}";
        }

        public override int GetLength() => _formatter.GetLength();
        public override string ToString() => $"EncryptionDecorator({_formatter}, {Algorithm})";
    }

    public class HighlightDecorator : TextDecorator
    {
        public string HighlightColor { get; set; }

        public HighlightDecorator(TextFormatter formatter, string color = "yellow") : base(formatter)
        {
            HighlightColor = color;
        }

        public override string Format()
        {
            return $"[HIGHLIGHT:{HighlightColor}] {_formatter.Format()} [/HIGHLIGHT]";
        }

        public override int GetLength() => _formatter.GetLength();
        public override string ToString() => $"HighlightDecorator({_formatter}, {HighlightColor})";
    }

    public class MarkdownDecorator : TextDecorator
    {
        public string MarkdownStyle { get; set; }

        public MarkdownDecorator(TextFormatter formatter, string style = "bold") : base(formatter)
        {
            MarkdownStyle = style;
        }

        public override string Format()
        {
            var formatted = _formatter.Format();
            return MarkdownStyle switch
            {
                "bold" => $"**{formatted}**",
                "italic" => $"*{formatted}*",
                "code" => $"`{formatted}`",
                "strikethrough" => $"~~{formatted}~~",
                _ => formatted
            };
        }

        public override int GetLength() => _formatter.GetLength();
        public override string ToString() => $"MarkdownDecorator({_formatter}, {MarkdownStyle})";
    }

    public class CompressionDecorator : TextDecorator
    {
        public CompressionDecorator(TextFormatter formatter) : base(formatter) { }

        public override string Format()
        {
            var text = _formatter.Format();
            // Simple compression simulation (remove spaces)
            var compressed = text.Replace(" ", "");
            return $"[COMPRESSED] {compressed}";
        }

        public override int GetLength() => (int)(_formatter.GetLength() * 0.7);
        public override string ToString() => $"CompressionDecorator({_formatter})";
    }
}
