using Xunit;
using Decorator.Window.Frame.Component;

namespace Decorator.Window.Frame.Tests
{
    public class WindowDecoratorTests
    {
        [Fact]
        public void SimpleWindow_ShouldInitializeCorrectly()
        {
            var window = new SimpleWindow("MainWindow");
            Assert.Equal("MainWindow", window.Title);
            Assert.Equal(800, window.Width);
            Assert.Equal(600, window.Height);
        }

        [Fact]
        public void SimpleWindow_ShouldCalculateSize()
        {
            var window = new SimpleWindow("Test");
            Assert.Equal(480000, window.GetSize()); // 800 * 600
        }

        [Fact]
        public void ScrollbarsDecorator_ShouldAddScrollbars()
        {
            var window = new SimpleWindow("Editor");
            var decorated = new ScrollbarsDecorator(window);
            Assert.True(decorated.ShowHorizontalScrollbar);
            Assert.True(decorated.ShowVerticalScrollbar);
        }

        [Fact]
        public void ScrollbarsDecorator_ShouldIncreaseSize()
        {
            var window = new SimpleWindow("Editor");
            var decorated = new ScrollbarsDecorator(window);
            Assert.True(decorated.GetSize() > window.GetSize());
        }

        [Fact]
        public void TitleBarDecorator_ShouldAddButtons()
        {
            var window = new SimpleWindow("App");
            var decorated = new TitleBarDecorator(window);
            Assert.True(decorated.ShowMinimizeButton);
            Assert.True(decorated.ShowMaximizeButton);
            Assert.True(decorated.ShowCloseButton);
        }

        [Fact]
        public void ThemeDecorator_ShouldApplyDarkTheme()
        {
            var window = new SimpleWindow("Dark");
            var decorated = new ThemeDecorator(window, "Dark");
            Assert.Equal("Dark", decorated.Theme);
            Assert.Contains("1e1e1e", decorated.BackgroundColor);
        }

        [Fact]
        public void ThemeDecorator_ShouldApplyLightTheme()
        {
            var window = new SimpleWindow("Light");
            var decorated = new ThemeDecorator(window, "Light");
            Assert.Equal("Light", decorated.Theme);
            Assert.Contains("ffffff", decorated.BackgroundColor);
        }

        [Fact]
        public void ShadowDecorator_ShouldAddShadow()
        {
            var window = new SimpleWindow("Shadow");
            var decorated = new ShadowDecorator(window);
            Assert.Equal(10, decorated.BlurRadius);
            Assert.Equal("#000000", decorated.ShadowColor);
        }

        [Fact]
        public void BorderDecorator_ShouldAddBorder()
        {
            var window = new SimpleWindow("Border");
            var decorated = new BorderDecorator(window);
            Assert.Equal(2, decorated.BorderWidth);
            Assert.Equal("Solid", decorated.BorderStyle);
        }

        [Fact]
        public void ChainedDecorators_ShouldStackMultiple()
        {
            var window = new SimpleWindow("Complex");
            var decorated = new BorderDecorator(
                new ShadowDecorator(
                    new ThemeDecorator(
                        new TitleBarDecorator(
                            new ScrollbarsDecorator(window)))));
            Assert.NotNull(decorated);
            Assert.Contains("BorderDecorator", decorated.ToString());
        }

        [Fact]
        public void DecoratedWindow_ShouldPreserveTitleAndDimensions()
        {
            var window = new SimpleWindow("PreserveTest");
            var decorated = new ScrollbarsDecorator(window);
            Assert.Equal("PreserveTest", decorated.Title);
            Assert.Equal(800, decorated.Width);
            Assert.Equal(600, decorated.Height);
        }
    }
}
