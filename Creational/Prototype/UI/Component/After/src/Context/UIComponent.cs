using System;
using System.Collections.Generic;

namespace Prototype.UI.Component.Context
{
    /// <summary>
    /// Product: UI component with style and theme cloning.
    /// Demonstrates: Prototype pattern for UI component instantiation.
    /// </summary>
    public class UIComponent
    {
        public string Id { get; set; }
        public string ComponentType { get; set; } // Button, TextBox, Label, Panel
        public string Text { get; set; }
        public Styling Style { get; set; }
        public Layout Layout { get; set; }
        public EventHandlers Events { get; set; }
        public IList<UIComponent> Children { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }

        public UIComponent()
        {
            Children = new List<UIComponent>();
            Style = new Styling();
            Layout = new Layout();
            Events = new EventHandlers();
            IsEnabled = true;
            IsVisible = true;
        }

        /// <summary>
        /// Deep copy clone of this component and its children.
        /// </summary>
        public UIComponent Clone()
        {
            var clone = new UIComponent
            {
                Id = Guid.NewGuid().ToString(),
                ComponentType = this.ComponentType,
                Text = this.Text,
                Style = this.Style?.Clone(),
                Layout = this.Layout?.Clone(),
                Events = this.Events?.Clone(),
                IsEnabled = this.IsEnabled,
                IsVisible = this.IsVisible
            };

            foreach (var child in this.Children)
            {
                clone.Children.Add(child.Clone());
            }

            return clone;
        }

        public override string ToString()
        {
            return $"UIComponent(Type={ComponentType}, Text={Text}, Enabled={IsEnabled}, " +
                   $"Children={Children.Count}, Size={Layout?.Width}x{Layout?.Height})";
        }
    }

    /// <summary>
    /// Styling information.
    /// </summary>
    public class Styling
    {
        public string BackgroundColor { get; set; }
        public string ForegroundColor { get; set; }
        public string BorderColor { get; set; }
        public int BorderWidth { get; set; }
        public int BorderRadius { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public string FontWeight { get; set; } // Normal, Bold, Light
        public int Opacity { get; set; } // 0-100
        public IList<string> CssClasses { get; set; }

        public Styling()
        {
            CssClasses = new List<string>();
        }

        public Styling Clone()
        {
            var clone = new Styling
            {
                BackgroundColor = this.BackgroundColor,
                ForegroundColor = this.ForegroundColor,
                BorderColor = this.BorderColor,
                BorderWidth = this.BorderWidth,
                BorderRadius = this.BorderRadius,
                FontFamily = this.FontFamily,
                FontSize = this.FontSize,
                FontWeight = this.FontWeight,
                Opacity = this.Opacity
            };

            foreach (var cls in this.CssClasses)
            {
                clone.CssClasses.Add(cls);
            }

            return clone;
        }

        public override string ToString() =>
            $"Styling(BG={BackgroundColor}, FG={ForegroundColor}, Font={FontSize}pt)";
    }

    /// <summary>
    /// Layout information.
    /// </summary>
    public class Layout
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Display { get; set; } // Block, Inline, Flex, Grid
        public string AlignItems { get; set; } // Start, Center, End
        public string JustifyContent { get; set; }
        public int Padding { get; set; }
        public int Margin { get; set; }

        public Layout Clone()
        {
            return new Layout
            {
                X = this.X,
                Y = this.Y,
                Width = this.Width,
                Height = this.Height,
                Display = this.Display,
                AlignItems = this.AlignItems,
                JustifyContent = this.JustifyContent,
                Padding = this.Padding,
                Margin = this.Margin
            };
        }

        public override string ToString() =>
            $"Layout(Pos={X},{Y}, Size={Width}x{Height}, Display={Display})";
    }

    /// <summary>
    /// Event handlers and callbacks.
    /// </summary>
    public class EventHandlers
    {
        public string OnClick { get; set; }
        public string OnHover { get; set; }
        public string OnFocus { get; set; }
        public string OnBlur { get; set; }
        public string OnChange { get; set; }
        public IList<string> CustomEvents { get; set; }

        public EventHandlers()
        {
            CustomEvents = new List<string>();
        }

        public EventHandlers Clone()
        {
            var clone = new EventHandlers
            {
                OnClick = this.OnClick,
                OnHover = this.OnHover,
                OnFocus = this.OnFocus,
                OnBlur = this.OnBlur,
                OnChange = this.OnChange
            };

            foreach (var evt in this.CustomEvents)
            {
                clone.CustomEvents.Add(evt);
            }

            return clone;
        }

        public override string ToString() =>
            $"Events(Click={OnClick}, Hover={OnHover}, Focus={OnFocus})";
    }

    /// <summary>
    /// Component library for managing UI prototypes.
    /// </summary>
    public class ComponentLibrary
    {
        private readonly Dictionary<string, UIComponent> _components =
            new Dictionary<string, UIComponent>();

        public void RegisterComponent(string name, UIComponent component)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            _components[name] = component;
        }

        public UIComponent GetComponent(string name)
        {
            if (!_components.ContainsKey(name))
                throw new KeyNotFoundException($"Component '{name}' not found");

            return _components[name];
        }

        public UIComponent CloneComponent(string name)
        {
            if (!_components.ContainsKey(name))
                throw new KeyNotFoundException($"Component '{name}' not found");

            return _components[name].Clone();
        }

        public UIComponent CloneComponent(string name, string newId)
        {
            var component = CloneComponent(name);
            component.Id = newId;
            return component;
        }

        public bool HasComponent(string name) => _components.ContainsKey(name);

        public int ComponentCount => _components.Count;
    }
}
