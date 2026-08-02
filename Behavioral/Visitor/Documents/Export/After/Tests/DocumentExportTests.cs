using NUnit.Framework;
using DocumentExport.After.Context;

namespace DocumentExport.After.Tests
{
    [TestFixture]
    public class DocumentExportTests
    {
        private Document _doc;

        [SetUp]
        public void Setup()
        {
            _doc = new Document { Title = "Test Doc" };
            _doc.Elements.Add(new HeadingElement { Content = "Title", Level = 1 });
            _doc.Elements.Add(new TextElement { Content = "Sample text" });
            _doc.Elements.Add(new ImageElement { FileName = "test.jpg", Width = 100, Height = 100 });
        }

        [Test]
        public void HtmlExporter_Success() { var exporter = new HtmlExporter(); _doc.Accept(exporter); Assert.That(exporter.GetHtml(), Does.Contain("<html>")); }

        [Test]
        public void PdfExporter_Success() { var exporter = new PdfExporter(); _doc.Accept(exporter); Assert.That(exporter.GetPdf(), Does.Contain("%PDF")); }

        [Test]
        public void MarkdownExporter_Success() { var exporter = new MarkdownExporter(); _doc.Accept(exporter); Assert.That(exporter.GetMarkdown(), Does.Contain("# Test Doc")); }

        [Test]
        public void MultipleFormatExports() 
        { 
            var html = new HtmlExporter(); _doc.Accept(html);
            var pdf = new PdfExporter(); _doc.Accept(pdf);
            var md = new MarkdownExporter(); _doc.Accept(md);
            Assert.That(html.GetHtml().Length, Is.GreaterThan(0));
            Assert.That(pdf.GetPdf().Length, Is.GreaterThan(0));
            Assert.That(md.GetMarkdown().Length, Is.GreaterThan(0));
        }
    }
}
