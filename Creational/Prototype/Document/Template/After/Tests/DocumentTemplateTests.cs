using Xunit;
using Prototype.Document.Template.Context;
using System;

namespace Prototype.Document.Template.Tests
{
    public class DocumentTemplateTests
    {
        private DocumentTemplate CreateSampleTemplate()
        {
            var template = new DocumentTemplate
            {
                Name = "Invoice",
                HasWatermark = true,
                WatermarkText = "CONFIDENTIAL"
            };

            template.PageLayout.Size = "A4";
            template.PageLayout.Orientation = "Portrait";
            template.PageLayout.Width = 210;
            template.PageLayout.Height = 297;

            template.Typography.DefaultFont = "Arial";
            template.Typography.DefaultFontSize = 11;
            template.Typography.LineHeight = "Single";
            template.Typography.TextColor = "#000000";
            template.Typography.AvailableFonts.Add("Arial");
            template.Typography.AvailableFonts.Add("Times New Roman");

            template.Margins.Top = 20;
            template.Margins.Bottom = 20;
            template.Margins.Left = 15;
            template.Margins.Right = 15;

            template.Header.Title = "Company Invoice";
            template.Header.Alignment = "Center";
            template.Header.Height = 60;

            template.Footer.PageNumberFormat = "Page X of Y";
            template.Footer.CenterText = "© 2026 Company";
            template.Footer.Height = 40;

            template.StyleRules.Add(new StyleRule
            {
                Selector = "h1",
                FontWeight = "Bold",
                FontStyle = "Normal",
                Color = "#000000"
            });

            return template;
        }

        [Fact]
        public void Clone_CreatesIndependentCopy()
        {
            var original = CreateSampleTemplate();
            var clone = original.Clone();

            Assert.NotSame(original, clone);
            Assert.Equal(original.Name, clone.Name);
            Assert.NotSame(original.PageLayout, clone.PageLayout);
            Assert.NotSame(original.Typography, clone.Typography);
        }

        [Fact]
        public void Clone_ChangeToCloneDoesNotAffectOriginal()
        {
            var original = CreateSampleTemplate();
            var clone = original.Clone();

            clone.Name = "Modified Invoice";
            clone.PageLayout.Size = "Letter";
            clone.Typography.DefaultFontSize = 12;

            Assert.Equal("Invoice", original.Name);
            Assert.Equal("A4", original.PageLayout.Size);
            Assert.Equal(11, original.Typography.DefaultFontSize);
        }

        [Fact]
        public void Clone_DeepCopiesNestedCollections()
        {
            var original = CreateSampleTemplate();
            var clone = original.Clone();

            clone.Typography.AvailableFonts.Add("Courier");
            clone.StyleRules.Add(new StyleRule { Selector = "p" });

            Assert.Equal(2, original.Typography.AvailableFonts.Count);
            Assert.Single(original.StyleRules);
        }

        [Fact]
        public void Clone_PreservesHeaderConfiguration()
        {
            var original = CreateSampleTemplate();
            var clone = original.Clone();

            Assert.Equal("Company Invoice", clone.Header.Title);
            Assert.Equal("Center", clone.Header.Alignment);
            Assert.Equal(60, clone.Header.Height);
        }

        [Fact]
        public void Clone_PreservesFooterConfiguration()
        {
            var original = CreateSampleTemplate();
            var clone = original.Clone();

            Assert.Equal("Page X of Y", clone.Footer.PageNumberFormat);
            Assert.Equal("© 2026 Company", clone.Footer.CenterText);
            Assert.Equal(40, clone.Footer.Height);
        }

        [Fact]
        public void Clone_WithWatermark_Success()
        {
            var original = CreateSampleTemplate();
            var clone = original.Clone();

            Assert.True(clone.HasWatermark);
            Assert.Equal("CONFIDENTIAL", clone.WatermarkText);
        }

        [Fact]
        public void Clone_WithStyleRules_Success()
        {
            var original = CreateSampleTemplate();
            original.StyleRules.Add(new StyleRule { Selector = "p", FontWeight = "Normal" });
            
            var clone = original.Clone();

            Assert.Equal(2, clone.StyleRules.Count);
            Assert.Equal("h1", clone.StyleRules[0].Selector);
            Assert.Equal("p", clone.StyleRules[1].Selector);
        }

        [Fact]
        public void Registry_RegisterAndClone_Success()
        {
            var registry = new DocumentTemplateRegistry();
            var template = CreateSampleTemplate();

            registry.RegisterTemplate("Invoice", template);
            var cloned = registry.CloneTemplate("Invoice");

            Assert.NotSame(template, cloned);
            Assert.Equal("Invoice", cloned.Name);
        }

        [Fact]
        public void Registry_RegisterNullTemplate_ThrowsException()
        {
            var registry = new DocumentTemplateRegistry();

            var exception = Assert.Throws<ArgumentNullException>(() =>
                registry.RegisterTemplate("Template", null)
            );

            Assert.Contains("template", exception.Message);
        }

        [Fact]
        public void Registry_RegisterNullName_ThrowsException()
        {
            var registry = new DocumentTemplateRegistry();
            var template = CreateSampleTemplate();

            var exception = Assert.Throws<ArgumentException>(() =>
                registry.RegisterTemplate(null, template)
            );

            Assert.Contains("Name cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Registry_GetTemplateNotFound_ThrowsException()
        {
            var registry = new DocumentTemplateRegistry();

            var exception = Assert.Throws<KeyNotFoundException>(() =>
                registry.GetTemplate("NonExistent")
            );

            Assert.Contains("not found", exception.Message);
        }

        [Fact]
        public void Registry_MultipleTemplates_Success()
        {
            var registry = new DocumentTemplateRegistry();
            
            var invoice = CreateSampleTemplate();
            invoice.Name = "Invoice";
            
            var quote = CreateSampleTemplate();
            quote.Name = "Quote";

            registry.RegisterTemplate("Invoice", invoice);
            registry.RegisterTemplate("Quote", quote);

            Assert.Equal(2, registry.TemplateCount);
            Assert.True(registry.HasTemplate("Invoice"));
            Assert.True(registry.HasTemplate("Quote"));
        }

        [Fact]
        public void Clone_PreservesMargins()
        {
            var original = CreateSampleTemplate();
            var clone = original.Clone();

            Assert.Equal(original.Margins.Top, clone.Margins.Top);
            Assert.Equal(original.Margins.Bottom, clone.Margins.Bottom);
            Assert.Equal(original.Margins.Left, clone.Margins.Left);
            Assert.Equal(original.Margins.Right, clone.Margins.Right);
        }

        [Fact]
        public void Clone_ModifyMargins_DoesNotAffectOriginal()
        {
            var original = CreateSampleTemplate();
            var clone = original.Clone();

            clone.Margins.Top = 50;

            Assert.Equal(20, original.Margins.Top);
            Assert.Equal(50, clone.Margins.Top);
        }

        [Fact]
        public void Clone_WithComplexStyleRules()
        {
            var original = CreateSampleTemplate();
            original.StyleRules.Add(new StyleRule
            {
                Selector = "table",
                FontWeight = "Normal",
                Color = "#333333",
                BackgroundColor = "#FFFFFF",
                Padding = 10
            });

            var clone = original.Clone();

            Assert.Equal("table", clone.StyleRules[1].Selector);
            Assert.Equal("#333333", clone.StyleRules[1].Color);
            Assert.Equal(10, clone.StyleRules[1].Padding);
        }

        [Fact]
        public void DocumentTemplate_ToString_ContainsInfo()
        {
            var template = CreateSampleTemplate();
            var str = template.ToString();

            Assert.Contains("Invoice", str);
            Assert.Contains("A4", str);
        }

        [Fact]
        public void PageLayout_Clone_Independent()
        {
            var layout = new PageLayout { Size = "A4", Width = 210 };
            var clone = layout.Clone();

            clone.Size = "Letter";
            clone.Width = 216;

            Assert.Equal("A4", layout.Size);
            Assert.Equal(210, layout.Width);
        }

        [Fact]
        public void Typography_Clone_IndependentFonts()
        {
            var typo = new Typography { DefaultFont = "Arial" };
            typo.AvailableFonts.Add("Arial");
            typo.AvailableFonts.Add("Times");

            var clone = typo.Clone();
            clone.AvailableFonts.Add("Courier");

            Assert.Equal(2, typo.AvailableFonts.Count);
            Assert.Equal(3, clone.AvailableFonts.Count);
        }

        [Fact]
        public void Registry_HasTemplate_ChecksCorrectly()
        {
            var registry = new DocumentTemplateRegistry();
            var template = CreateSampleTemplate();

            registry.RegisterTemplate("Template1", template);

            Assert.True(registry.HasTemplate("Template1"));
            Assert.False(registry.HasTemplate("Template2"));
        }

        [Fact]
        public void Clone_ChainedClones_AllIndependent()
        {
            var original = CreateSampleTemplate();
            var clone1 = original.Clone();
            var clone2 = clone1.Clone();

            clone2.Name = "Modified";
            clone2.PageLayout.Size = "Letter";

            Assert.Equal("Invoice", original.Name);
            Assert.Equal("Invoice", clone1.Name);
            Assert.Equal("Modified", clone2.Name);
        }
    }
}
