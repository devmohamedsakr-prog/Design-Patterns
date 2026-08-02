using System;
using System.Collections.Generic;

namespace Flyweight.WebBrowser.DOM.Component
{
    // Intrinsic State: Shared CSS styles
    public class CSSStyle
    {
        public string ClassName { get; set; }
        public string Margin { get; set; }
        public string Padding { get; set; }
        public string Color { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public string BackgroundColor { get; set; }

        public override string ToString() => $".{ClassName}";
    }

    // Flyweight Factory for CSS styles
    public class CSSStyleFactory
    {
        private Dictionary<string, CSSStyle> _stylePool = new();

        public CSSStyleFactory()
        {
            _stylePool["text"] = new CSSStyle 
            { 
                ClassName = "text", Margin = "0px", Padding = "5px", Color = "#000000", 
                FontFamily = "Arial", FontSize = 14, BackgroundColor = "transparent" 
            };
            _stylePool["btn"] = new CSSStyle 
            { 
                ClassName = "btn", Margin = "5px", Padding = "10px", Color = "#FFFFFF", 
                FontFamily = "Arial", FontSize = 12, BackgroundColor = "#0066CC" 
            };
            _stylePool["header"] = new CSSStyle 
            { 
                ClassName = "header", Margin = "0px", Padding = "15px", Color = "#FFFFFF", 
                FontFamily = "Georgia", FontSize = 24, BackgroundColor = "#333333" 
            };
        }

        public CSSStyle GetStyle(string className)
        {
            return _stylePool.ContainsKey(className) ? _stylePool[className] : null;
        }

        public void AddStyle(string className, CSSStyle style)
        {
            _stylePool[className] = style;
        }

        public int GetPoolSize() => _stylePool.Count;
    }

    // Extrinsic State: Per-element unique data
    public class DOMElement
    {
        public string ElementId { get; set; }
        public string TagName { get; set; }
        public string Content { get; set; }
        public CSSStyle Style { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new();

        public override string ToString() => $"<{TagName} id=\"{ElementId}\">";
    }

    // DOM Tree using Flyweight pattern
    public class DOMTree
    {
        private Dictionary<string, DOMElement> _elements = new();
        private CSSStyleFactory _styleFactory = new();
        private int _elementIdCounter = 0;

        public DOMElement CreateElement(string tagName, string className, string content)
        {
            var elementId = $"elem_{++_elementIdCounter}";
            var style = _styleFactory.GetStyle(className);
            
            var element = new DOMElement
            {
                ElementId = elementId,
                TagName = tagName,
                Content = content,
                Style = style
            };

            _elements[elementId] = element;
            return element;
        }

        public void SetAttribute(string elementId, string attrName, string attrValue)
        {
            if (_elements.ContainsKey(elementId))
                _elements[elementId].Attributes[attrName] = attrValue;
        }

        public int GetElementCount() => _elements.Count;
        public int GetUniqueStyleCount() => _styleFactory.GetPoolSize();
        public long EstimateMemorySavings() => (long)_elements.Count * 5000 - _styleFactory.GetPoolSize() * 5000 - _elements.Count * 512;
        public IReadOnlyDictionary<string, DOMElement> GetElements() => _elements;
    }
}
