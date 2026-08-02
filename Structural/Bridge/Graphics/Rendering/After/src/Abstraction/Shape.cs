using System;
using System.Collections.Generic;
using Bridge.Graphics.Rendering.Implementation;

namespace Bridge.Graphics.Rendering.Abstraction
{
    /// <summary>
    /// Abstraction: Shape hierarchy independent of rendering implementation.
    /// Demonstrates: Bridge pattern separating shape logic from rendering technology.
    /// </summary>
    public abstract class Shape
    {
        protected IRenderer _renderer;

        public Shape(IRenderer renderer)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }

        public abstract void Draw();

        public void SetRenderer(IRenderer renderer)
        {
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
        }
    }

    /// <summary>
    /// Concrete abstraction: Circle shape.
    /// </summary>
    public class Circle : Shape
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; set; }

        public Circle(IRenderer renderer, int x, int y, int radius) : base(renderer)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public override void Draw()
        {
            _renderer.DrawCircle(X, Y, Radius);
        }

        public override string ToString() => $"Circle(({X},{Y}), R={Radius})";
    }

    /// <summary>
    /// Concrete abstraction: Rectangle shape.
    /// </summary>
    public class Rectangle : Shape
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Rectangle(IRenderer renderer, int x, int y, int width, int height) : base(renderer)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override void Draw()
        {
            _renderer.DrawRectangle(X, Y, Width, Height);
        }

        public override string ToString() => $"Rectangle(({X},{Y}), {Width}x{Height})";
    }

    /// <summary>
    /// Concrete abstraction: Triangle shape.
    /// </summary>
    public class Triangle : Shape
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int X3 { get; set; }
        public int Y3 { get; set; }

        public Triangle(IRenderer renderer, int x1, int y1, int x2, int y2, int x3, int y3) 
            : base(renderer)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            X3 = x3;
            Y3 = y3;
        }

        public override void Draw()
        {
            _renderer.DrawTriangle(X1, Y1, X2, Y2, X3, Y3);
        }

        public override string ToString() => $"Triangle(({X1},{Y1}),({X2},{Y2}),({X3},{Y3}))";
    }

    /// <summary>
    /// Concrete abstraction: Line shape.
    /// </summary>
    public class Line : Shape
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }

        public Line(IRenderer renderer, int x1, int y1, int x2, int y2) : base(renderer)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public override void Draw()
        {
            _renderer.DrawLine(X1, Y1, X2, Y2);
        }

        public override string ToString() => $"Line(({X1},{Y1})->({X2},{Y2}))";
    }

    /// <summary>
    /// Canvas for managing multiple shapes.
    /// </summary>
    public class Canvas
    {
        private readonly List<Shape> _shapes = new List<Shape>();

        public void AddShape(Shape shape)
        {
            if (shape == null)
                throw new ArgumentNullException(nameof(shape));
            _shapes.Add(shape);
        }

        public void RemoveShape(Shape shape)
        {
            _shapes.Remove(shape);
        }

        public void DrawAll()
        {
            foreach (var shape in _shapes)
            {
                shape.Draw();
            }
        }

        public void ChangeRenderer(IRenderer newRenderer)
        {
            foreach (var shape in _shapes)
            {
                shape.SetRenderer(newRenderer);
            }
        }

        public int ShapeCount => _shapes.Count;
    }
}
