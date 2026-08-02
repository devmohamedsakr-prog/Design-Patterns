using Xunit;
using Flyweight.WebBrowser.DOM.Component;

namespace Flyweight.WebBrowser.DOM.Tests
{
    public class WebBrowserFlyweightTests
    {
        [Fact]
        public void CSSStyleFactory_ShouldCreatePredefinedStyles()
        {
            var factory = new CSSStyleFactory();
            Assert.Equal(3, factory.GetPoolSize());
        }

        [Fact]
        public void CSSStyleFactory_ShouldReuseCSSStyle()
        {
            var factory = new CSSStyleFactory();
            var style1 = factory.GetStyle("text");
            var style2 = factory.GetStyle("text");
            
            Assert.Same(style1, style2);
        }

        [Fact]
        public void CSSStyle_ShouldHaveCorrectProperties()
        {
            var factory = new CSSStyleFactory();
            var btnStyle = factory.GetStyle("btn");
            
            Assert.Equal("btn", btnStyle.ClassName);
            Assert.Equal("#FFFFFF", btnStyle.Color);
            Assert.Equal("#0066CC", btnStyle.BackgroundColor);
        }

        [Fact]
        public void DOMTree_ShouldCreateElements()
        {
            var dom = new DOMTree();
            var elem = dom.CreateElement("div", "text", "Hello");
            
            Assert.NotNull(elem);
            Assert.Equal("Hello", elem.Content);
        }

        [Fact]
        public void DOMTree_ShouldReuseStyles()
        {
            var dom = new DOMTree();
            var elem1 = dom.CreateElement("p", "text", "Para 1");
            var elem2 = dom.CreateElement("p", "text", "Para 2");
            var elem3 = dom.CreateElement("p", "text", "Para 3");
            
            Assert.Equal(3, dom.GetElementCount());
            Assert.Same(elem1.Style, elem2.Style); // Same style reference
            Assert.Same(elem2.Style, elem3.Style);
        }

        [Fact]
        public void DOMTree_ShouldCreateWithDifferentStyles()
        {
            var dom = new DOMTree();
            dom.CreateElement("h1", "header", "Title");
            dom.CreateElement("button", "btn", "Click");
            dom.CreateElement("p", "text", "Text");
            
            Assert.Equal(3, dom.GetElementCount());
            Assert.True(dom.GetUniqueStyleCount() >= 3);
        }

        [Fact]
        public void DOMTree_ShouldSetAttributes()
        {
            var dom = new DOMTree();
            var elem = dom.CreateElement("input", "text", "");
            dom.SetAttribute(elem.ElementId, "type", "password");
            dom.SetAttribute(elem.ElementId, "placeholder", "Enter password");
            
            Assert.Equal(2, elem.Attributes.Count);
            Assert.Equal("password", elem.Attributes["type"]);
        }

        [Fact]
        public void DOMTree_ShouldCalculateMemorySavings()
        {
            var dom = new DOMTree();
            for (int i = 0; i < 1000; i++)
            {
                dom.CreateElement("p", "text", $"Paragraph {i}");
            }
            
            var savings = dom.EstimateMemorySavings();
            Assert.True(savings > 0);
        }

        [Fact]
        public void DOMTree_ShouldGenerateUniqueElementIds()
        {
            var dom = new DOMTree();
            var elem1 = dom.CreateElement("div", "text", "");
            var elem2 = dom.CreateElement("div", "text", "");
            
            Assert.NotEqual(elem1.ElementId, elem2.ElementId);
        }

        [Fact]
        public void CSSStyleFactory_ShouldAllowCustomStyles()
        {
            var factory = new CSSStyleFactory();
            var customStyle = new CSSStyle 
            { 
                ClassName = "custom",
                Color = "#FF00FF",
                FontSize = 20,
                Padding = "20px"
            };
            
            factory.AddStyle("custom", customStyle);
            Assert.Equal(4, factory.GetPoolSize());
        }

        [Fact]
        public void LargeDOM_ShouldHandleManyElements()
        {
            var dom = new DOMTree();
            for (int i = 0; i < 10000; i++)
            {
                dom.CreateElement("span", "text", $"Item {i}");
            }
            
            Assert.Equal(10000, dom.GetElementCount());
            Assert.True(dom.EstimateMemorySavings() > 1000000);
        }
    }
}
