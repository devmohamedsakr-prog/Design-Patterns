using System;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.Document.Template.Context
{
    /// <summary>
    /// Product: Document template with deep copy clone capability.
    /// Demonstrates: Prototype pattern for cloning complex document templates.
    /// </summary>
    public class DocumentTemplate
    {
        public string Name { get; set; }
        public PageLayout PageLayout { get; set; }
        public Typography Typography { get; set; }
        public Margins Margins { get; set; }
        public Header Header { get; set; }
        public Footer Footer { get; set; }
        public IList<StyleRule> StyleRules { get; set; }
        public bool HasWatermark { get; set; }
        public string WatermarkText { get; set; }

        public DocumentTemplate()
        {
            StyleRules = new List<StyleRule>();
            PageLayout = new PageLayout();
            Typography = new Typography();
            Margins = new Margins();
            Header = new Header();
            Footer = new Footer();
        }

        /// <summary>
        /// Deep copy clone of this template.
        /// </summary>
        public DocumentTemplate Clone()
        {
            var clone = new DocumentTemplate
            {
                Name = this.Name,
                HasWatermark = this.HasWatermark,
                WatermarkText = this.WatermarkText,
                PageLayout = this.PageLayout?.Clone(),
                Typography = this.Typography?.Clone(),
                Margins = this.Margins?.Clone(),
                Header = this.Header?.Clone(),
                Footer = this.Footer?.Clone()
            };

            foreach (var style in this.StyleRules)
            {
                clone.StyleRules.Add(style.Clone());
            }

            return clone;
        }

        public override string ToString()
        {
            return $"DocumentTemplate(Name={Name}, PageSize={PageLayout?.Size}, " +
                   $"Orientation={PageLayout?.Orientation}, Styles={StyleRules.Count})";
        }
    }

    /// <summary>
    /// Page layout configuration.
    /// </summary>
    public class PageLayout
    {
        public string Size { get; set; } // A4, Letter, Legal
        public string Orientation { get; set; } // Portrait, Landscape
        public int Width { get; set; }
        public int Height { get; set; }

        public PageLayout Clone()
        {
            return new PageLayout
            {
                Size = this.Size,
                Orientation = this.Orientation,
                Width = this.Width,
                Height = this.Height
            };
        }

        public override string ToString() => $"PageLayout({Size}, {Orientation}, {Width}x{Height})";
    }

    /// <summary>
    /// Typography settings.
    /// </summary>
    public class Typography
    {
        public string DefaultFont { get; set; }
        public int DefaultFontSize { get; set; }
        public string LineHeight { get; set; } // Single, Double, Custom
        public string TextColor { get; set; }
        public IList<string> AvailableFonts { get; set; }

        public Typography()
        {
            AvailableFonts = new List<string>();
        }

        public Typography Clone()
        {
            var clone = new Typography
            {
                DefaultFont = this.DefaultFont,
                DefaultFontSize = this.DefaultFontSize,
                LineHeight = this.LineHeight,
                TextColor = this.TextColor
            };

            foreach (var font in this.AvailableFonts)
            {
                clone.AvailableFonts.Add(font);
            }

            return clone;
        }

        public override string ToString() =>
            $"Typography({DefaultFont}, {DefaultFontSize}pt, LineHeight={LineHeight})";
    }

    /// <summary>
    /// Margin settings.
    /// </summary>
    public class Margins
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public Margins Clone()
        {
            return new Margins
            {
                Top = this.Top,
                Bottom = this.Bottom,
                Left = this.Left,
                Right = this.Right
            };
        }

        public override string ToString() => $"Margins(T={Top}, B={Bottom}, L={Left}, R={Right})";
    }

    /// <summary>
    /// Header configuration.
    /// </summary>
    public class Header
    {
        public string Title { get; set; }
        public string LogoPath { get; set; }
        public string Alignment { get; set; } // Left, Center, Right
        public int Height { get; set; }
        public IList<string> ContentLines { get; set; }

        public Header()
        {
            ContentLines = new List<string>();
        }

        public Header Clone()
        {
            var clone = new Header
            {
                Title = this.Title,
                LogoPath = this.LogoPath,
                Alignment = this.Alignment,
                Height = this.Height
            };

            foreach (var line in this.ContentLines)
            {
                clone.ContentLines.Add(line);
            }

            return clone;
        }

        public override string ToString() => $"Header(Title={Title}, Height={Height}px)";
    }

    /// <summary>
    /// Footer configuration.
    /// </summary>
    public class Footer
    {
        public string PageNumberFormat { get; set; } // "Page X", "X of Y"
        public string CenterText { get; set; }
        public string Alignment { get; set; }
        public int Height { get; set; }
        public IList<string> ContentLines { get; set; }

        public Footer()
        {
            ContentLines = new List<string>();
        }

        public Footer Clone()
        {
            var clone = new Footer
            {
                PageNumberFormat = this.PageNumberFormat,
                CenterText = this.CenterText,
                Alignment = this.Alignment,
                Height = this.Height
            };

            foreach (var line in this.ContentLines)
            {
                clone.ContentLines.Add(line);
            }

            return clone;
        }

        public override string ToString() =>
            $"Footer(PageFormat={PageNumberFormat}, Center={CenterText}, Height={Height}px)";
    }

    /// <summary>
    /// Style rule for formatting.
    /// </summary>
    public class StyleRule
    {
        public string Selector { get; set; }
        public string FontWeight { get; set; }
        public string FontStyle { get; set; }
        public string Color { get; set; }
        public string BackgroundColor { get; set; }
        public int Padding { get; set; }

        public StyleRule Clone()
        {
            return new StyleRule
            {
                Selector = this.Selector,
                FontWeight = this.FontWeight,
                FontStyle = this.FontStyle,
                Color = this.Color,
                BackgroundColor = this.BackgroundColor,
                Padding = this.Padding
            };
        }

        public override string ToString() =>
            $"StyleRule({Selector}, Weight={FontWeight}, Color={Color})";
    }

    /// <summary>
    /// Template registry for managing prototypes.
    /// </summary>
    public class DocumentTemplateRegistry
    {
        private readonly Dictionary<string, DocumentTemplate> _templates =
            new Dictionary<string, DocumentTemplate>();

        public void RegisterTemplate(string name, DocumentTemplate template)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            _templates[name] = template;
        }

        public DocumentTemplate GetTemplate(string name)
        {
            if (!_templates.ContainsKey(name))
                throw new KeyNotFoundException($"Template '{name}' not found");

            return _templates[name];
        }

        public DocumentTemplate CloneTemplate(string name)
        {
            if (!_templates.ContainsKey(name))
                throw new KeyNotFoundException($"Template '{name}' not found");

            return _templates[name].Clone();
        }

        public bool HasTemplate(string name) => _templates.ContainsKey(name);

        public int TemplateCount => _templates.Count;
    }
}
