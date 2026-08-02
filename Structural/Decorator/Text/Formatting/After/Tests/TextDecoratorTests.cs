using Xunit;
using Decorator.Text.Formatting.Component;

namespace Decorator.Text.Formatting.Tests
{
    public class TextDecoratorTests
    {
        [Fact]
        public void PlainTextFormatter_ShouldReturnPlainText()
        {
            var formatter = new PlainTextFormatter("Hello World");
            Assert.Equal("Hello World", formatter.Format());
        }

        [Fact]
        public void PlainTextFormatter_ShouldCalculateLength()
        {
            var formatter = new PlainTextFormatter("Test");
            Assert.Equal(4, formatter.GetLength());
        }

        [Fact]
        public void EncryptionDecorator_ShouldEncryptText()
        {
            var formatter = new PlainTextFormatter("ABC");
            var encrypted = new EncryptionDecorator(formatter);
            var result = encrypted.Format();
            Assert.Contains("Caesar", result);
        }

        [Fact]
        public void EncryptionDecorator_ShouldPreserveNonLetters()
        {
            var formatter = new PlainTextFormatter("A1B2C3");
            var encrypted = new EncryptionDecorator(formatter);
            var result = encrypted.Format();
            Assert.Contains("1", result);
            Assert.Contains("2", result);
            Assert.Contains("3", result);
        }

        [Fact]
        public void HighlightDecorator_ShouldHighlightText()
        {
            var formatter = new PlainTextFormatter("Important");
            var highlighted = new HighlightDecorator(formatter, "yellow");
            var result = highlighted.Format();
            Assert.Contains("HIGHLIGHT", result);
            Assert.Contains("yellow", result);
        }

        [Fact]
        public void MarkdownDecorator_ShouldApplyBold()
        {
            var formatter = new PlainTextFormatter("Bold Text");
            var markdown = new MarkdownDecorator(formatter, "bold");
            Assert.Equal("**Bold Text**", markdown.Format());
        }

        [Fact]
        public void MarkdownDecorator_ShouldApplyItalic()
        {
            var formatter = new PlainTextFormatter("Italic");
            var markdown = new MarkdownDecorator(formatter, "italic");
            Assert.Equal("*Italic*", markdown.Format());
        }

        [Fact]
        public void MarkdownDecorator_ShouldApplyCode()
        {
            var formatter = new PlainTextFormatter("Code");
            var markdown = new MarkdownDecorator(formatter, "code");
            Assert.Equal("`Code`", markdown.Format());
        }

        [Fact]
        public void MarkdownDecorator_ShouldApplyStrikethrough()
        {
            var formatter = new PlainTextFormatter("Strike");
            var markdown = new MarkdownDecorator(formatter, "strikethrough");
            Assert.Equal("~~Strike~~", markdown.Format());
        }

        [Fact]
        public void CompressionDecorator_ShouldCompressText()
        {
            var formatter = new PlainTextFormatter("This is a test");
            var compressed = new CompressionDecorator(formatter);
            var result = compressed.Format();
            Assert.Contains("COMPRESSED", result);
        }

        [Fact]
        public void CompressionDecorator_ShouldReduceSize()
        {
            var formatter = new PlainTextFormatter("This is a very long text with many spaces");
            var compressed = new CompressionDecorator(formatter);
            Assert.True(compressed.GetLength() < formatter.GetLength());
        }

        [Fact]
        public void ChainedDecorators_ShouldCompose()
        {
            var formatter = new PlainTextFormatter("Chain");
            var decorated = new CompressionDecorator(
                new MarkdownDecorator(
                    new HighlightDecorator(
                        new EncryptionDecorator(formatter), "red"), "bold"));
            var result = decorated.Format();
            Assert.NotNull(result);
        }

        [Fact]
        public void MultipleDecorators_ShouldStackCorrectly()
        {
            var text = "Stacked";
            var formatter = new PlainTextFormatter(text);
            var decorated = new HighlightDecorator(formatter, "blue");
            Assert.Contains("HIGHLIGHT", decorated.Format());
        }

        [Fact]
        public void DecoratedFormatter_ShouldPreserveOriginalLength()
        {
            var formatter = new PlainTextFormatter("Preserve");
            var decorated = new HighlightDecorator(formatter);
            Assert.Equal(formatter.GetLength(), decorated.GetLength());
        }
    }
}
