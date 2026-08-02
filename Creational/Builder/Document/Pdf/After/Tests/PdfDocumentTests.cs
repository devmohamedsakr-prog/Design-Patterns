using Xunit;
using Builder.Document.Pdf.Context;
using System;

namespace Builder.Document.Pdf.Tests
{
    public class PdfDocumentTests
    {
        [Fact]
        public void Builder_CreateBasicPdf_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("Report")
                .Author("John Doe")
                .AddPage("Page 1 content")
                .Build();

            Assert.Equal("Report", pdf.Title);
            Assert.Equal("John Doe", pdf.Author);
            Assert.Single(pdf.Pages);
            Assert.Equal("Page 1 content", pdf.Pages[0]);
        }

        [Fact]
        public void Builder_MultiplePages_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("Multi-page Report")
                .Author("Jane Smith")
                .AddPage("Introduction")
                .AddPage("Chapter 1")
                .AddPage("Chapter 2")
                .AddPage("Conclusion")
                .Build();

            Assert.Equal(4, pdf.Pages.Count);
            Assert.Equal("Introduction", pdf.Pages[0]);
            Assert.Equal("Conclusion", pdf.Pages[3]);
        }

        [Fact]
        public void Builder_WithPageSize_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("Letter Size")
                .Author("Author")
                .PageSize(2) // Letter
                .AddPage("Content")
                .Build();

            Assert.Equal(2, pdf.PageSize);
        }

        [Fact]
        public void Builder_WithOrientation_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("Landscape")
                .Author("Author")
                .Orientation(2) // Landscape
                .AddPage("Wide content")
                .Build();

            Assert.Equal(2, pdf.Orientation);
        }

        [Fact]
        public void Builder_WithFonts_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("Fonts")
                .Author("Author")
                .AddFont("Arial")
                .AddFont("Times New Roman")
                .AddFont("Courier")
                .AddPage("Content")
                .Build();

            Assert.Equal(3, pdf.Fonts.Count);
            Assert.Contains("Arial", pdf.Fonts);
        }

        [Fact]
        public void Builder_DuplicateFonts_AddedOnce()
        {
            var pdf = PdfDocument.Builder
                .Title("Duplicate Fonts")
                .Author("Author")
                .AddFont("Arial")
                .AddFont("Arial")
                .AddFont("Times New Roman")
                .AddPage("Content")
                .Build();

            Assert.Equal(2, pdf.Fonts.Count);
        }

        [Fact]
        public void Builder_WithMetadata_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("Metadata")
                .Author("Author")
                .AddMetadata("Keywords", "report, analysis")
                .AddMetadata("Creator", "MyApp v1.0")
                .AddPage("Content")
                .Build();

            Assert.Equal(2, pdf.Metadata.Count);
            Assert.Equal("report, analysis", pdf.Metadata["Keywords"]);
        }

        [Fact]
        public void Builder_WithEncryption_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("Encrypted")
                .Author("Author")
                .EnableEncryption()
                .AddPage("Secret content")
                .Build();

            Assert.True(pdf.IsEncrypted);
        }

        [Fact]
        public void Builder_WithCompression_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("Compressed")
                .Author("Author")
                .SetCompression("Lz77")
                .AddPage("Content")
                .Build();

            Assert.Equal("Lz77", pdf.Compression);
        }

        [Fact]
        public void Builder_WithTableOfContents_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("TOC")
                .Author("Author")
                .IncludeTableOfContents()
                .AddPage("Content")
                .Build();

            Assert.True(pdf.HasTableOfContents);
        }

        [Fact]
        public void Builder_WithBookmarks_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("Bookmarks")
                .Author("Author")
                .AddBookmark("Chapter 1")
                .AddBookmark("Chapter 2")
                .AddBookmark("Appendix")
                .AddPage("Content")
                .Build();

            Assert.Equal(3, pdf.Bookmarks.Count);
            Assert.Contains("Chapter 1", pdf.Bookmarks);
        }

        [Fact]
        public void Builder_ComplexPdf_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("Complex Report")
                .Author("Expert Author")
                .Subject("Advanced Analytics")
                .PageSize(1) // A4
                .Orientation(2) // Landscape
                .AddPage("Title Page")
                .AddPage("Executive Summary")
                .AddPage("Analysis")
                .AddFont("Arial")
                .AddFont("Times New Roman")
                .AddMetadata("Keywords", "analytics, report")
                .EnableEncryption()
                .SetCompression("Deflate")
                .IncludeTableOfContents()
                .AddBookmark("Summary")
                .AddBookmark("Analysis")
                .Build();

            Assert.Equal("Complex Report", pdf.Title);
            Assert.Equal(3, pdf.Pages.Count);
            Assert.Equal(2, pdf.Fonts.Count);
            Assert.True(pdf.IsEncrypted);
            Assert.True(pdf.HasTableOfContents);
            Assert.Equal(2, pdf.Bookmarks.Count);
        }

        [Fact]
        public void Builder_MissingTitle_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                PdfDocument.Builder
                    .Author("Author")
                    .AddPage("Content")
                    .Build()
            );

            Assert.Contains("Title is required", exception.Message);
        }

        [Fact]
        public void Builder_MissingAuthor_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                PdfDocument.Builder
                    .Title("Report")
                    .AddPage("Content")
                    .Build()
            );

            Assert.Contains("Author is required", exception.Message);
        }

        [Fact]
        public void Builder_NoPages_ThrowsException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                PdfDocument.Builder
                    .Title("Report")
                    .Author("Author")
                    .Build()
            );

            Assert.Contains("At least one page is required", exception.Message);
        }

        [Fact]
        public void Builder_InvalidPageSize_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                PdfDocument.Builder
                    .Title("Report")
                    .Author("Author")
                    .PageSize(5)
                    .AddPage("Content")
                    .Build()
            );

            Assert.Contains("PageSize must be", exception.Message);
        }

        [Fact]
        public void Builder_InvalidOrientation_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                PdfDocument.Builder
                    .Title("Report")
                    .Author("Author")
                    .Orientation(3)
                    .AddPage("Content")
                    .Build()
            );

            Assert.Contains("Orientation must be", exception.Message);
        }

        [Fact]
        public void Builder_InvalidCompression_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                PdfDocument.Builder
                    .Title("Report")
                    .Author("Author")
                    .SetCompression("Invalid")
                    .AddPage("Content")
                    .Build()
            );

            Assert.Contains("Compression must be", exception.Message);
        }

        [Fact]
        public void Builder_NullTitle_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                PdfDocument.Builder.Title(null)
            );

            Assert.Contains("Title cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_EmptyAuthor_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                PdfDocument.Builder.Author("")
            );

            Assert.Contains("Author cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_NullPageContent_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                PdfDocument.Builder.AddPage(null)
            );

            Assert.Contains("Content cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_InvalidFont_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                PdfDocument.Builder.AddFont("")
            );

            Assert.Contains("FontName cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Builder_IsImmutable_Collections()
        {
            var pdf = PdfDocument.Builder
                .Title("Report")
                .Author("Author")
                .AddPage("Page 1")
                .AddFont("Arial")
                .AddBookmark("Chapter")
                .Build();

            Assert.Throws<NotSupportedException>(() =>
            {
                ((System.Collections.Generic.List<string>)pdf.Pages).Add("Page 2");
            });
        }

        [Fact]
        public void Builder_FluentChaining_Success()
        {
            var pdf = PdfDocument.Builder
                .Title("Fluent")
                .Author("Author")
                .Subject("Test")
                .PageSize(1)
                .Orientation(1)
                .AddFont("Arial")
                .AddPage("Content")
                .EnableEncryption()
                .Build();

            Assert.NotNull(pdf);
            Assert.True(pdf.IsEncrypted);
        }

        [Fact]
        public void PdfDocument_ToString_ContainsRelevantInfo()
        {
            var pdf = PdfDocument.Builder
                .Title("Test Report")
                .Author("Test Author")
                .AddPage("Content")
                .EnableEncryption()
                .Build();

            var str = pdf.ToString();
            Assert.Contains("Test Report", str);
            Assert.Contains("Test Author", str);
        }

        [Fact]
        public void Builder_DefaultValues_Applied()
        {
            var pdf = PdfDocument.Builder
                .Title("Default")
                .Author("Author")
                .AddPage("Content")
                .Build();

            Assert.Equal(1, pdf.PageSize); // A4
            Assert.Equal(1, pdf.Orientation); // Portrait
            Assert.Equal("Deflate", pdf.Compression);
            Assert.False(pdf.IsEncrypted);
            Assert.False(pdf.HasTableOfContents);
        }
    }
}
