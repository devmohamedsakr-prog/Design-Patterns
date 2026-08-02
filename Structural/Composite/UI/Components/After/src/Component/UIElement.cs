using System;
using System.Collections.Generic;
using System.Linq;

namespace Composite.UI.Components.Component
{
    /// <summary>
    /// Component interface: UI elements can be individual or composite.
    /// Demonstrates: Composite pattern for treating buttons same as panel hierarchies.
    /// </summary>
    public abstract class UIElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public UIElement(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public abstract void Render();
        public abstract int GetTotalSize();
        public abstract void Enable();
        public abstract void Disable();
    }

    /// <summary>
    /// Leaf: Button element with no children.
    /// </summary>
    public class Button : UIElement
    {
        public string Label { get; set; }
        public bool IsEnabled { get; set; }
        public string ClickHandler { get; set; }

        public Button(string id, string name, string label) : base(id, name)
        {
            Label = label;
            IsEnabled = true;
        }

        public override void Render()
        {
            Console.WriteLine($"  [Button] {Label} ({Width}x{Height})");
        }

        public override int GetTotalSize() => Width * Height;

        public override void Enable() => IsEnabled = true;

        public override void Disable() => IsEnabled = false;

        public override string ToString() => $"Button({Label}, {Width}x{Height})";
    }

    /// <summary>
    /// Leaf: Label element with no children.
    /// </summary>
    public class Label : UIElement
    {
        public string Text { get; set; }
        public string FontSize { get; set; }

        public Label(string id, string name, string text) : base(id, name)
        {
            Text = text;
            FontSize = "12px";
        }

        public override void Render()
        {
            Console.WriteLine($"  [Label] {Text}");
        }

        public override int GetTotalSize() => Width * Height;

        public override void Enable() { }

        public override void Disable() { }

        public override string ToString() => $"Label({Text})";
    }

    /// <summary>
    /// Leaf: TextBox element.
    /// </summary>
    public class TextBox : UIElement
    {
        public string Placeholder { get; set; }
        public string Value { get; set; }
        public bool IsReadOnly { get; set; }

        public TextBox(string id, string name) : base(id, name)
        {
            IsReadOnly = false;
        }

        public override void Render()
        {
            Console.WriteLine($"  [TextBox] {Placeholder ?? "Input"} ({Width}x{Height})");
        }

        public override int GetTotalSize() => Width * Height;

        public override void Enable() => IsReadOnly = false;

        public override void Disable() => IsReadOnly = true;

        public override string ToString() => $"TextBox({Name}, {Width}x{Height})";
    }

    /// <summary>
    /// Composite: Panel container that can hold other elements.
    /// </summary>
    public class Panel : UIElement
    {
        private readonly List<UIElement> _children = new List<UIElement>();
        public string BackgroundColor { get; set; }
        public int Padding { get; set; }
        public bool IsEnabled { get; set; }

        public Panel(string id, string name) : base(id, name)
        {
            BackgroundColor = "#FFFFFF";
            Padding = 10;
            IsEnabled = true;
        }

        public void Add(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            _children.Add(element);
        }

        public void Remove(UIElement element)
        {
            _children.Remove(element);
        }

        public IReadOnlyList<UIElement> GetChildren() => _children.AsReadOnly();

        public override void Render()
        {
            Console.WriteLine($"[Panel] {Name} ({Width}x{Height})");
            foreach (var child in _children)
            {
                child.Render();
            }
            Console.WriteLine($"[/Panel]");
        }

        public override int GetTotalSize()
        {
            int totalSize = Width * Height;
            foreach (var child in _children)
            {
                totalSize += child.GetTotalSize();
            }
            return totalSize;
        }

        public override void Enable()
        {
            IsEnabled = true;
            foreach (var child in _children)
            {
                child.Enable();
            }
        }

        public override void Disable()
        {
            IsEnabled = false;
            foreach (var child in _children)
            {
                child.Disable();
            }
        }

        public override string ToString() => $"Panel({Name}, {_children.Count} children)";
    }

    /// <summary>
    /// Composite: Form container with specialized layout.
    /// </summary>
    public class Form : UIElement
    {
        private readonly List<UIElement> _fields = new List<UIElement>();
        public string Title { get; set; }
        public bool IsSubmitted { get; set; }

        public Form(string id, string name, string title) : base(id, name)
        {
            Title = title;
            IsSubmitted = false;
        }

        public void AddField(UIElement field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));
            _fields.Add(field);
        }

        public override void Render()
        {
            Console.WriteLine($"<Form> {Title} ({_fields.Count} fields)");
            foreach (var field in _fields)
            {
                field.Render();
            }
            Console.WriteLine($"</Form>");
        }

        public override int GetTotalSize()
        {
            return _fields.Sum(f => f.GetTotalSize());
        }

        public override void Enable()
        {
            foreach (var field in _fields)
            {
                field.Enable();
            }
        }

        public override void Disable()
        {
            foreach (var field in _fields)
            {
                field.Disable();
            }
        }

        public override string ToString() => $"Form({Title}, {_fields.Count} fields)";
    }
}
