using System;
using System.Collections.Generic;

namespace XMLProcessing.After.Context
{
    public interface IXMLNode
    {
        void Accept(IXMLVisitor visitor);
    }

    public interface IXMLVisitor
    {
        void Visit(XMLElement element);
        void Visit(XMLAttribute attr);
        void Visit(XMLText text);
    }

    public class XMLElement : IXMLNode
    {
        public string TagName { get; set; } = "";
        public List<XMLAttribute> Attributes { get; set; } = new();
        public List<IXMLNode> Children { get; set; } = new();

        public void Accept(IXMLVisitor visitor)
        {
            visitor.Visit(this);
            foreach (var attr in Attributes)
                attr.Accept(visitor);
            foreach (var child in Children)
                child.Accept(visitor);
        }
    }

    public class XMLAttribute : IXMLNode
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public void Accept(IXMLVisitor visitor) => visitor.Visit(this);
    }

    public class XMLText : IXMLNode
    {
        public string Content { get; set; } = "";
        public void Accept(IXMLVisitor visitor) => visitor.Visit(this);
    }

    public class XMLValidator : IXMLVisitor
    {
        public List<string> Errors { get; set; } = new();

        public void Visit(XMLElement element)
        {
            if (string.IsNullOrEmpty(element.TagName))
                Errors.Add("Element missing tag name");
        }

        public void Visit(XMLAttribute attr)
        {
            if (string.IsNullOrEmpty(attr.Name) || string.IsNullOrEmpty(attr.Value))
                Errors.Add("Attribute missing name or value");
        }

        public void Visit(XMLText text) { }
    }

    public class XMLTransformer : IXMLVisitor
    {
        public string TransformedXml { get; set; } = "";

        public void Visit(XMLElement element) => TransformedXml += $"<{element.TagName.ToUpper()}>";
        public void Visit(XMLAttribute attr) => TransformedXml += $" {attr.Name}=\"{attr.Value}\"";
        public void Visit(XMLText text) => TransformedXml += text.Content;
    }
}
