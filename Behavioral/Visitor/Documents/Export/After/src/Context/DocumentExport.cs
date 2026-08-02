using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentExport.After.Context
{
    /// <summary>
    /// IDocumentElement: Accepts visitors for traversal
    /// </summary>
    public interface IDocumentElement
    {
        void Accept(IDocumentVisitor visitor);
    }

    /// <summary>
    /// IDocumentVisitor: Visitor interface
    /// </summary>
    public interface IDocumentVisitor
    {
        void Visit(TextElement element);
        void Visit(ImageElement element);
        void Visit(HeadingElement element);
        void Visit(TableElement element);
        void Visit(Document doc);
    }

    /// <summary>
    /// Document elements
    /// </summary>
    public class Document : IDocumentElement
    {
        public string Title { get; set; } = "";
        public List<IDocumentElement> Elements { get; set; } = new();

        public void Accept(IDocumentVisitor visitor)
        {
            visitor.Visit(this);
            foreach (var element in Elements)
                element.Accept(visitor);
        }
    }

    public class TextElement : IDocumentElement
    {
        public string Content { get; set; } = "";
        public void Accept(IDocumentVisitor visitor) => visitor.Visit(this);
    }

    public class HeadingElement : IDocumentElement
    {
        public string Content { get; set; } = "";
        public int Level { get; set; }
        public void Accept(IDocumentVisitor visitor) => visitor.Visit(this);
    }

    public class ImageElement : IDocumentElement
    {
        public string FileName { get; set; } = "";
        public int Width { get; set; }
        public int Height { get; set; }
        public void Accept(IDocumentVisitor visitor) => visitor.Visit(this);
    }

    public class TableElement : IDocumentElement
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<string> Headers { get; set; } = new();
        public void Accept(IDocumentVisitor visitor) => visitor.Visit(this);
    }

    /// <summary>
    /// Concrete visitors
    /// </summary>
    public class HtmlExporter : IDocumentVisitor
    {
        public StringBuilder Html { get; set; } = new();

        public void Visit(Document doc) => Html.AppendLine($"<html><head><title>{doc.Title}</title></head><body>");
        public void Visit(TextElement element) => Html.AppendLine($"<p>{element.Content}</p>");
        public void Visit(HeadingElement element) => Html.AppendLine($"<h{element.Level}>{element.Content}</h{element.Level}>");
        public void Visit(ImageElement element) => Html.AppendLine($"<img src='{element.FileName}' width='{element.Width}' height='{element.Height}'/>");
        public void Visit(TableElement element) => Html.AppendLine($"<table><tr>{"".Join(element.Headers.Select(h => $"<th>{h}</th>"))}</tr></table>");

        public string GetHtml()
        {
            Html.AppendLine("</body></html>");
            return Html.ToString();
        }
    }

    public class PdfExporter : IDocumentVisitor
    {
        public StringBuilder Pdf { get; set; } = new();

        public void Visit(Document doc) => Pdf.AppendLine($"%PDF-1.4\n(Title: {doc.Title})");
        public void Visit(TextElement element) => Pdf.AppendLine($"(Text: {element.Content})");
        public void Visit(HeadingElement element) => Pdf.AppendLine($"(Heading {element.Level}: {element.Content})");
        public void Visit(ImageElement element) => Pdf.AppendLine($"(Image: {element.FileName} {element.Width}x{element.Height})");
        public void Visit(TableElement element) => Pdf.AppendLine($"(Table: {element.Rows}x{element.Columns})");

        public string GetPdf()
        {
            Pdf.AppendLine("%%EOF");
            return Pdf.ToString();
        }
    }

    public class MarkdownExporter : IDocumentVisitor
    {
        public StringBuilder Markdown { get; set; } = new();

        public void Visit(Document doc) => Markdown.AppendLine($"# {doc.Title}\n");
        public void Visit(TextElement element) => Markdown.AppendLine($"{element.Content}\n");
        public void Visit(HeadingElement element) => Markdown.AppendLine($"{"".PadRight(element.Level, '#')} {element.Content}\n");
        public void Visit(ImageElement element) => Markdown.AppendLine($"![{element.FileName}]({element.FileName})\n");
        public void Visit(TableElement element) => Markdown.AppendLine($"| {"".Join(element.Headers, " | ")} |\n");

        public string GetMarkdown() => Markdown.ToString();
    }
}
