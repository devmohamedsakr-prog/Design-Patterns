using System;
using System.Collections.Generic;
using System.Linq;

namespace Composite.Graphics.SceneGraph.Component
{
    /// <summary>
    /// Component interface: Graphics objects in scene hierarchy.
    /// Demonstrates: Composite pattern for treating single shape same as group with transformations.
    /// </summary>
    public abstract class GraphicsElement
    {
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public bool IsVisible { get; set; }

        protected GraphicsElement(string name)
        {
            Name = name;
            X = 0;
            Y = 0;
            Rotation = 0;
            ScaleX = 1;
            ScaleY = 1;
            IsVisible = true;
        }

        public abstract void Draw();
        public abstract void Rotate(float angle);
        public abstract void Scale(float sx, float sy);
        public abstract void Translate(float dx, float dy);
        public abstract int GetBoundingBoxSize();
    }

    /// <summary>
    /// Leaf: Circle shape.
    /// </summary>
    public class Circle : GraphicsElement
    {
        public float Radius { get; set; }
        public string Color { get; set; }

        public Circle(string name, float radius) : base(name)
        {
            Radius = radius;
            Color = "#000000";
        }

        public override void Draw()
        {
            if (IsVisible)
                Console.WriteLine($"  Drawing Circle: {Name} at ({X},{Y}) r={Radius}");
        }

        public override void Rotate(float angle) => Rotation = (Rotation + angle) % 360;

        public override void Scale(float sx, float sy)
        {
            ScaleX *= sx;
            ScaleY *= sy;
        }

        public override void Translate(float dx, float dy)
        {
            X += dx;
            Y += dy;
        }

        public override int GetBoundingBoxSize() => (int)(Radius * 2);

        public override string ToString() => $"Circle({Name}, R={Radius})";
    }

    /// <summary>
    /// Leaf: Rectangle shape.
    /// </summary>
    public class Rectangle : GraphicsElement
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public string Color { get; set; }

        public Rectangle(string name, float width, float height) : base(name)
        {
            Width = width;
            Height = height;
            Color = "#000000";
        }

        public override void Draw()
        {
            if (IsVisible)
                Console.WriteLine($"  Drawing Rect: {Name} at ({X},{Y}) {Width}x{Height}");
        }

        public override void Rotate(float angle) => Rotation = (Rotation + angle) % 360;

        public override void Scale(float sx, float sy)
        {
            Width *= sx;
            Height *= sy;
        }

        public override void Translate(float dx, float dy)
        {
            X += dx;
            Y += dy;
        }

        public override int GetBoundingBoxSize() => (int)(Width * Height);

        public override string ToString() => $"Rectangle({Name}, {Width}x{Height})";
    }

    /// <summary>
    /// Leaf: Line shape.
    /// </summary>
    public class Line : GraphicsElement
    {
        public float X2 { get; set; }
        public float Y2 { get; set; }
        public float Thickness { get; set; }
        public string Color { get; set; }

        public Line(string name, float x2, float y2) : base(name)
        {
            X2 = x2;
            Y2 = y2;
            Thickness = 1;
            Color = "#000000";
        }

        public override void Draw()
        {
            if (IsVisible)
                Console.WriteLine($"  Drawing Line: {Name} from ({X},{Y}) to ({X2},{Y2})");
        }

        public override void Rotate(float angle) => Rotation = (Rotation + angle) % 360;

        public override void Scale(float sx, float sy)
        {
            X2 *= sx;
            Y2 *= sy;
        }

        public override void Translate(float dx, float dy)
        {
            X += dx;
            Y += dy;
            X2 += dx;
            Y2 += dy;
        }

        public override int GetBoundingBoxSize() => (int)(X2 * Y2);

        public override string ToString() => $"Line({Name})";
    }

    /// <summary>
    /// Composite: Group of graphics elements.
    /// </summary>
    public class Group : GraphicsElement
    {
        private readonly List<GraphicsElement> _children = new List<GraphicsElement>();

        public Group(string name) : base(name)
        {
        }

        public void Add(GraphicsElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            _children.Add(element);
        }

        public void Remove(GraphicsElement element)
        {
            _children.Remove(element);
        }

        public IReadOnlyList<GraphicsElement> GetChildren() => _children.AsReadOnly();

        public override void Draw()
        {
            Console.WriteLine($"Drawing Group: {Name}");
            if (IsVisible)
            {
                foreach (var child in _children)
                {
                    child.Draw();
                }
            }
        }

        public override void Rotate(float angle)
        {
            Rotation = (Rotation + angle) % 360;
            foreach (var child in _children)
            {
                child.Rotate(angle);
            }
        }

        public override void Scale(float sx, float sy)
        {
            ScaleX *= sx;
            ScaleY *= sy;
            foreach (var child in _children)
            {
                child.Scale(sx, sy);
            }
        }

        public override void Translate(float dx, float dy)
        {
            X += dx;
            Y += dy;
            foreach (var child in _children)
            {
                child.Translate(dx, dy);
            }
        }

        public override int GetBoundingBoxSize()
        {
            return _children.Sum(c => c.GetBoundingBoxSize());
        }

        public override string ToString() => $"Group({Name}, {_children.Count} elements)";
    }

    /// <summary>
    /// Composite: Layer for organizing graphics.
    /// </summary>
    public class Layer : GraphicsElement
    {
        private readonly List<GraphicsElement> _elements = new List<GraphicsElement>();
        public int ZIndex { get; set; }

        public Layer(string name, int zIndex = 0) : base(name)
        {
            ZIndex = zIndex;
        }

        public void AddElement(GraphicsElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            _elements.Add(element);
        }

        public void RemoveElement(GraphicsElement element)
        {
            _elements.Remove(element);
        }

        public override void Draw()
        {
            Console.WriteLine($"Layer: {Name} (Z={ZIndex})");
            foreach (var element in _elements)
            {
                element.Draw();
            }
        }

        public override void Rotate(float angle)
        {
            foreach (var element in _elements)
            {
                element.Rotate(angle);
            }
        }

        public override void Scale(float sx, float sy)
        {
            foreach (var element in _elements)
            {
                element.Scale(sx, sy);
            }
        }

        public override void Translate(float dx, float dy)
        {
            foreach (var element in _elements)
            {
                element.Translate(dx, dy);
            }
        }

        public override int GetBoundingBoxSize()
        {
            return _elements.Sum(e => e.GetBoundingBoxSize());
        }

        public override string ToString() => $"Layer({Name}, {_elements.Count} elements)";
    }
}
