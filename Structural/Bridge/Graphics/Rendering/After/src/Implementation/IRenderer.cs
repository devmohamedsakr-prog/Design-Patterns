using System;
using System.Collections.Generic;

namespace Bridge.Graphics.Rendering.Implementation
{
    /// <summary>
    /// Implementation interface: Renderer contract.
    /// </summary>
    public interface IRenderer
    {
        void DrawCircle(int x, int y, int radius);
        void DrawRectangle(int x, int y, int width, int height);
        void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3);
        void DrawLine(int x1, int y1, int x2, int y2);
    }

    /// <summary>
    /// Implementation: GDI+ renderer for Windows.
    /// </summary>
    public class GDIPlusRenderer : IRenderer
    {
        private readonly List<string> _drawCommands = new List<string>();

        public void DrawCircle(int x, int y, int radius)
        {
            var command = $"GDI+: Draw circle at ({x},{y}) with radius {radius}";
            _drawCommands.Add(command);
        }

        public void DrawRectangle(int x, int y, int width, int height)
        {
            var command = $"GDI+: Draw rectangle at ({x},{y}) size {width}x{height}";
            _drawCommands.Add(command);
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            var command = $"GDI+: Draw triangle ({x1},{y1})->({x2},{y2})->({x3},{y3})";
            _drawCommands.Add(command);
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            var command = $"GDI+: Draw line ({x1},{y1})->({x2},{y2})";
            _drawCommands.Add(command);
        }

        public IReadOnlyList<string> GetCommands => _drawCommands.AsReadOnly();

        public override string ToString() => $"GDIPlusRenderer({_drawCommands.Count} commands)";
    }

    /// <summary>
    /// Implementation: OpenGL renderer for cross-platform graphics.
    /// </summary>
    public class OpenGLRenderer : IRenderer
    {
        private readonly List<string> _drawCommands = new List<string>();

        public void DrawCircle(int x, int y, int radius)
        {
            var command = $"OpenGL: glDrawCircle({x}, {y}, {radius})";
            _drawCommands.Add(command);
        }

        public void DrawRectangle(int x, int y, int width, int height)
        {
            var command = $"OpenGL: glDrawRect({x}, {y}, {width}, {height})";
            _drawCommands.Add(command);
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            var command = $"OpenGL: glDrawTri({x1}, {y1}, {x2}, {y2}, {x3}, {y3})";
            _drawCommands.Add(command);
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            var command = $"OpenGL: glDrawLine({x1}, {y1}, {x2}, {y2})";
            _drawCommands.Add(command);
        }

        public IReadOnlyList<string> GetCommands => _drawCommands.AsReadOnly();

        public override string ToString() => $"OpenGLRenderer({_drawCommands.Count} commands)";
    }

    /// <summary>
    /// Implementation: Vulkan renderer for high-performance graphics.
    /// </summary>
    public class VulkanRenderer : IRenderer
    {
        private readonly List<string> _drawCommands = new List<string>();

        public void DrawCircle(int x, int y, int radius)
        {
            var command = $"Vulkan: vkCmdDrawCircle(x={x}, y={y}, r={radius})";
            _drawCommands.Add(command);
        }

        public void DrawRectangle(int x, int y, int width, int height)
        {
            var command = $"Vulkan: vkCmdDrawRect(x={x}, y={y}, w={width}, h={height})";
            _drawCommands.Add(command);
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            var command = $"Vulkan: vkCmdDrawTri(p1=({x1},{y1}), p2=({x2},{y2}), p3=({x3},{y3}))";
            _drawCommands.Add(command);
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            var command = $"Vulkan: vkCmdDrawLine(p1=({x1},{y1}), p2=({x2},{y2}))";
            _drawCommands.Add(command);
        }

        public IReadOnlyList<string> GetCommands => _drawCommands.AsReadOnly();

        public override string ToString() => $"VulkanRenderer({_drawCommands.Count} commands)";
    }

    /// <summary>
    /// Implementation: SVG renderer for web/vector graphics.
    /// </summary>
    public class SVGRenderer : IRenderer
    {
        private readonly List<string> _svgElements = new List<string>();

        public void DrawCircle(int x, int y, int radius)
        {
            var svg = $"<circle cx=\"{x}\" cy=\"{y}\" r=\"{radius}\" />";
            _svgElements.Add(svg);
        }

        public void DrawRectangle(int x, int y, int width, int height)
        {
            var svg = $"<rect x=\"{x}\" y=\"{y}\" width=\"{width}\" height=\"{height}\" />";
            _svgElements.Add(svg);
        }

        public void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            var svg = $"<polygon points=\"{x1},{y1} {x2},{y2} {x3},{y3}\" />";
            _svgElements.Add(svg);
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            var svg = $"<line x1=\"{x1}\" y1=\"{y1}\" x2=\"{x2}\" y2=\"{y2}\" />";
            _svgElements.Add(svg);
        }

        public IReadOnlyList<string> GetSVG => _svgElements.AsReadOnly();

        public override string ToString() => $"SVGRenderer({_svgElements.Count} elements)";
    }
}
