using System;
using System.Collections.Generic;

namespace Decorator.Window.Frame.Component
{
    public abstract class Window
    {
        public string Title { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Window(string title)
        {
            Title = title;
            Width = 800;
            Height = 600;
        }

        public abstract void Draw();
        public abstract int GetSize();
    }

    public class SimpleWindow : Window
    {
        public SimpleWindow(string title) : base(title) { }

        public override void Draw() => Console.WriteLine($"Drawing window: {Title} ({Width}x{Height})");
        public override int GetSize() => Width * Height;

        public override string ToString() => $"Window({Title}, {Width}x{Height})";
    }

    public abstract class WindowDecorator : Window
    {
        protected Window _window;

        public WindowDecorator(Window window) : base(window.Title)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            Width = window.Width;
            Height = window.Height;
        }
    }

    public class ScrollbarsDecorator : WindowDecorator
    {
        public bool ShowHorizontalScrollbar { get; set; }
        public bool ShowVerticalScrollbar { get; set; }

        public ScrollbarsDecorator(Window window) : base(window)
        {
            ShowHorizontalScrollbar = true;
            ShowVerticalScrollbar = true;
        }

        public override void Draw()
        {
            _window.Draw();
            Console.WriteLine("  + Horizontal Scrollbar");
            Console.WriteLine("  + Vertical Scrollbar");
        }

        public override int GetSize() => _window.GetSize() + 50;

        public override string ToString() => $"ScrollbarsDecorator({_window})";
    }

    public class TitleBarDecorator : WindowDecorator
    {
        public bool ShowMinimizeButton { get; set; }
        public bool ShowMaximizeButton { get; set; }
        public bool ShowCloseButton { get; set; }

        public TitleBarDecorator(Window window) : base(window)
        {
            ShowMinimizeButton = true;
            ShowMaximizeButton = true;
            ShowCloseButton = true;
        }

        public override void Draw()
        {
            _window.Draw();
            Console.WriteLine($"  + TitleBar: {Title} [_][□][X]");
        }

        public override int GetSize() => _window.GetSize() + 30;

        public override string ToString() => $"TitleBarDecorator({_window})";
    }

    public class ThemeDecorator : WindowDecorator
    {
        public string Theme { get; set; }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }

        public ThemeDecorator(Window window, string theme = "Light") : base(window)
        {
            Theme = theme;
            BackgroundColor = theme == "Dark" ? "#1e1e1e" : "#ffffff";
            BorderColor = theme == "Dark" ? "#404040" : "#cccccc";
        }

        public override void Draw()
        {
            _window.Draw();
            Console.WriteLine($"  + Theme: {Theme} (BG={BackgroundColor}, Border={BorderColor})");
        }

        public override int GetSize() => _window.GetSize();

        public override string ToString() => $"ThemeDecorator({_window}, {Theme})";
    }

    public class ShadowDecorator : WindowDecorator
    {
        public int BlurRadius { get; set; }
        public string ShadowColor { get; set; }

        public ShadowDecorator(Window window) : base(window)
        {
            BlurRadius = 10;
            ShadowColor = "#000000";
        }

        public override void Draw()
        {
            _window.Draw();
            Console.WriteLine($"  + Shadow: Blur={BlurRadius}, Color={ShadowColor}");
        }

        public override int GetSize() => _window.GetSize() + 20;

        public override string ToString() => $"ShadowDecorator({_window})";
    }

    public class BorderDecorator : WindowDecorator
    {
        public int BorderWidth { get; set; }
        public string BorderStyle { get; set; }

        public BorderDecorator(Window window) : base(window)
        {
            BorderWidth = 2;
            BorderStyle = "Solid";
        }

        public override void Draw()
        {
            _window.Draw();
            Console.WriteLine($"  + Border: {BorderWidth}px {BorderStyle}");
        }

        public override int GetSize() => _window.GetSize() + (BorderWidth * 4);

        public override string ToString() => $"BorderDecorator({_window})";
    }
}
