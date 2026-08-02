using Xunit;
using Flyweight.TextEditor.Document.Component;

namespace Flyweight.TextEditor.Document.Tests
{
    public class TextEditorFlyweightTests
    {
        [Fact]
        public void CharacterStyle_ShouldBeReusable()
        {
            var factory = new CharacterStyleFactory();
            var style1 = factory.GetStyle("Arial", 12, "#000000");
            var style2 = factory.GetStyle("Arial", 12, "#000000");
            
            Assert.Same(style1, style2); // Same reference
        }

        [Fact]
        public void CharacterStyle_ShouldBeUniqueDifferentParams()
        {
            var factory = new CharacterStyleFactory();
            var style1 = factory.GetStyle("Arial", 12, "#000000");
            var style2 = factory.GetStyle("Arial", 14, "#000000");
            
            Assert.NotSame(style1, style2);
        }

        [Fact]
        public void FactoryPool_ShouldGrowCorrectly()
        {
            var factory = new CharacterStyleFactory();
            Assert.Equal(0, factory.GetPoolSize());
            
            factory.GetStyle("Arial", 12, "#000000");
            Assert.Equal(1, factory.GetPoolSize());
            
            factory.GetStyle("Courier", 10, "#FF0000");
            Assert.Equal(2, factory.GetPoolSize());
        }

        [Fact]
        public void TextDocument_ShouldInsertCharacters()
        {
            var doc = new TextDocument();
            doc.InsertCharacter('A', 0, 0, "Arial", 12, "#000000");
            doc.InsertCharacter('B', 0, 1, "Arial", 12, "#000000");
            
            Assert.Equal(2, doc.GetCharacterCount());
        }

        [Fact]
        public void TextDocument_ShouldShareStyles()
        {
            var doc = new TextDocument();
            doc.InsertCharacter('A', 0, 0, "Arial", 12, "#000000");
            doc.InsertCharacter('B', 0, 1, "Arial", 12, "#000000");
            doc.InsertCharacter('C', 0, 2, "Arial", 12, "#000000");
            
            Assert.Equal(3, doc.GetCharacterCount());
            Assert.Equal(1, doc.GetUniqueStyleCount()); // All same style
        }

        [Fact]
        public void TextDocument_ShouldCreateMultipleStyles()
        {
            var doc = new TextDocument();
            doc.InsertCharacter('A', 0, 0, "Arial", 12, "#000000");
            doc.InsertCharacter('B', 0, 1, "Arial", 14, "#000000");
            doc.InsertCharacter('C', 0, 2, "Courier", 12, "#FF0000");
            
            Assert.Equal(3, doc.GetCharacterCount());
            Assert.Equal(3, doc.GetUniqueStyleCount()); // All different
        }

        [Fact]
        public void TextDocument_ShouldInsertText()
        {
            var doc = new TextDocument();
            doc.InsertText("Hello", 0, 0, "Arial", 12, "#000000");
            
            Assert.Equal(5, doc.GetCharacterCount());
            Assert.Equal(1, doc.GetUniqueStyleCount());
        }

        [Fact]
        public void TextDocument_ShouldCalculateMemorySavings()
        {
            var doc = new TextDocument();
            doc.InsertText("Lorem ipsum dolor sit amet", 0, 0, "Arial", 12, "#000000");
            
            var savings = doc.EstimateMemorySavings();
            Assert.True(savings > 0);
        }

        [Fact]
        public void CharacterStyle_ShouldPreserveAllProperties()
        {
            var factory = new CharacterStyleFactory();
            var style = factory.GetStyle("Courier", 14, "#FF0000", true, true, true);
            
            Assert.Equal("Courier", style.FontFamily);
            Assert.Equal(14, style.FontSize);
            Assert.Equal("#FF0000", style.Color);
            Assert.True(style.IsBold);
            Assert.True(style.IsItalic);
            Assert.True(style.IsUnderline);
        }

        [Fact]
        public void TextDocument_ShouldHandlargeDocuments()
        {
            var doc = new TextDocument();
            var text = new string('A', 10000);
            doc.InsertText(text, 0, 0, "Arial", 12, "#000000");
            
            Assert.Equal(10000, doc.GetCharacterCount());
            Assert.Equal(1, doc.GetUniqueStyleCount());
            Assert.True(doc.EstimateMemorySavings() > 1000000);
        }

        [Fact]
        public void Factory_ShouldCacheStyles()
        {
            var factory = new CharacterStyleFactory();
            for (int i = 0; i < 100; i++)
            {
                factory.GetStyle("Arial", 12, "#000000");
            }
            
            Assert.Equal(1, factory.GetPoolSize()); // Only one style cached
        }
    }
}
