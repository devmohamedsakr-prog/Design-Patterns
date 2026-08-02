using System;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Document.Pdf.Context
{
    /// <summary>
    /// Product: Immutable PDF document constructed via builder.
    /// Demonstrates: Step-by-step construction of complex document with multiple sections.
    /// </summary>
    public class PdfDocument
    {
        public string Title { get; }
        public string Author { get; }
        public string Subject { get; }
        public int PageSize { get; } // 1=A4, 2=Letter
        public int Orientation { get; } // 1=Portrait, 2=Landscape
        public IReadOnlyList<string> Pages { get; }
        public IReadOnlyList<string> Fonts { get; }
        public IReadOnlyDictionary<string, string> Metadata { get; }
        public bool IsEncrypted { get; }
        public string Compression { get; } // "None", "Deflate", "Lz77"
        public bool HasTableOfContents { get; }
        public IReadOnlyList<string> Bookmarks { get; }

        private PdfDocument(
            string title,
            string author,
            string subject,
            int pageSize,
            int orientation,
            IReadOnlyList<string> pages,
            IReadOnlyList<string> fonts,
            IReadOnlyDictionary<string, string> metadata,
            bool isEncrypted,
            string compression,
            bool hasTableOfContents,
            IReadOnlyList<string> bookmarks)
        {
            Title = title;
            Author = author;
            Subject = subject;
            PageSize = pageSize;
            Orientation = orientation;
            Pages = pages;
            Fonts = fonts;
            Metadata = metadata;
            IsEncrypted = isEncrypted;
            Compression = compression;
            HasTableOfContents = hasTableOfContents;
            Bookmarks = bookmarks;
        }

        public static PdfBuilder Builder => new PdfBuilder();

        public override string ToString()
        {
            return $"PdfDocument(Title={Title}, Author={Author}, Pages={Pages.Count}, " +
                   $"Size={PageSize}, Orientation={Orientation}, Encrypted={IsEncrypted}, Compression={Compression})";
        }

        /// <summary>
        /// Builder class: Fluent API for constructing PdfDocument.
        /// </summary>
        public class PdfBuilder
        {
            private string _title;
            private string _author;
            private string _subject;
            private int _pageSize = 1; // 1=A4, 2=Letter
            private int _orientation = 1; // 1=Portrait, 2=Landscape
            private readonly List<string> _pages = new();
            private readonly List<string> _fonts = new();
            private readonly Dictionary<string, string> _metadata = new();
            private bool _isEncrypted = false;
            private string _compression = "Deflate";
            private bool _hasTableOfContents = false;
            private readonly List<string> _bookmarks = new();

            public PdfBuilder Title(string title)
            {
                if (string.IsNullOrWhiteSpace(title))
                    throw new ArgumentException("Title cannot be null or empty", nameof(title));
                _title = title;
                return this;
            }

            public PdfBuilder Author(string author)
            {
                if (string.IsNullOrWhiteSpace(author))
                    throw new ArgumentException("Author cannot be null or empty", nameof(author));
                _author = author;
                return this;
            }

            public PdfBuilder Subject(string subject)
            {
                if (string.IsNullOrWhiteSpace(subject))
                    throw new ArgumentException("Subject cannot be null or empty", nameof(subject));
                _subject = subject;
                return this;
            }

            /// <summary>
            /// Set page size: 1=A4, 2=Letter.
            /// </summary>
            public PdfBuilder PageSize(int size)
            {
                if (!new[] { 1, 2 }.Contains(size))
                    throw new ArgumentException("PageSize must be 1 (A4) or 2 (Letter)", nameof(size));
                _pageSize = size;
                return this;
            }

            /// <summary>
            /// Set orientation: 1=Portrait, 2=Landscape.
            /// </summary>
            public PdfBuilder Orientation(int orientation)
            {
                if (!new[] { 1, 2 }.Contains(orientation))
                    throw new ArgumentException("Orientation must be 1 (Portrait) or 2 (Landscape)", nameof(orientation));
                _orientation = orientation;
                return this;
            }

            /// <summary>
            /// Add a page with content.
            /// </summary>
            public PdfBuilder AddPage(string content)
            {
                if (string.IsNullOrWhiteSpace(content))
                    throw new ArgumentException("Content cannot be null or empty", nameof(content));
                _pages.Add(content);
                return this;
            }

            /// <summary>
            /// Add a font for use in the document.
            /// </summary>
            public PdfBuilder AddFont(string fontName)
            {
                if (string.IsNullOrWhiteSpace(fontName))
                    throw new ArgumentException("FontName cannot be null or empty", nameof(fontName));
                if (!_fonts.Contains(fontName))
                    _fonts.Add(fontName);
                return this;
            }

            /// <summary>
            /// Add metadata key-value pair.
            /// </summary>
            public PdfBuilder AddMetadata(string key, string value)
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Key and value cannot be null or empty");
                _metadata[key] = value;
                return this;
            }

            /// <summary>
            /// Enable PDF encryption.
            /// </summary>
            public PdfBuilder EnableEncryption()
            {
                _isEncrypted = true;
                return this;
            }

            /// <summary>
            /// Set compression: "None", "Deflate", or "Lz77".
            /// </summary>
            public PdfBuilder SetCompression(string compression)
            {
                if (!new[] { "None", "Deflate", "Lz77" }.Contains(compression))
                    throw new ArgumentException("Compression must be 'None', 'Deflate', or 'Lz77'", nameof(compression));
                _compression = compression;
                return this;
            }

            /// <summary>
            /// Enable table of contents generation.
            /// </summary>
            public PdfBuilder IncludeTableOfContents()
            {
                _hasTableOfContents = true;
                return this;
            }

            /// <summary>
            /// Add a bookmark entry.
            /// </summary>
            public PdfBuilder AddBookmark(string bookmarkLabel)
            {
                if (string.IsNullOrWhiteSpace(bookmarkLabel))
                    throw new ArgumentException("BookmarkLabel cannot be null or empty", nameof(bookmarkLabel));
                _bookmarks.Add(bookmarkLabel);
                return this;
            }

            public PdfDocument Build()
            {
                if (string.IsNullOrWhiteSpace(_title))
                    throw new InvalidOperationException("Title is required");
                if (string.IsNullOrWhiteSpace(_author))
                    throw new InvalidOperationException("Author is required");
                if (_pages.Count == 0)
                    throw new InvalidOperationException("At least one page is required");

                return new PdfDocument(
                    _title,
                    _author,
                    _subject,
                    _pageSize,
                    _orientation,
                    _pages.AsReadOnly(),
                    _fonts.AsReadOnly(),
                    new Dictionary<string, string>(_metadata),
                    _isEncrypted,
                    _compression,
                    _hasTableOfContents,
                    _bookmarks.AsReadOnly()
                );
            }
        }
    }
}
