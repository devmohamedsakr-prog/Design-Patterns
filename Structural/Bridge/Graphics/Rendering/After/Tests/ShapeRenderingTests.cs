using Xunit;
using Bridge.Graphics.Rendering.Abstraction;
using Bridge.Graphics.Rendering.Implementation;

namespace Bridge.Graphics.Rendering.Tests
{
    public class ShapeRenderingTests
    {
        [Fact]
        public void Circle_DrawWithGDIPlus_Success()
        {
            var renderer = new GDIPlusRenderer();
            var circle = new Circle(renderer, 100, 50, 25);

            circle.Draw();

            Assert.Single(renderer.GetCommands);
            Assert.Contains("GDI+", renderer.GetCommands[0]);
        }

        [Fact]
        public void Circle_DrawWithOpenGL_Success()
        {
            var renderer = new OpenGLRenderer();
            var circle = new Circle(renderer, 100, 50, 25);

            circle.Draw();

            Assert.Single(renderer.GetCommands);
            Assert.Contains("OpenGL", renderer.GetCommands[0]);
        }

        [Fact]
        public void Rectangle_DrawWithVulkan_Success()
        {
            var renderer = new VulkanRenderer();
            var rect = new Rectangle(renderer, 10, 20, 100, 50);

            rect.Draw();

            Assert.Single(renderer.GetCommands);
            Assert.Contains("Vulkan", renderer.GetCommands[0]);
        }

        [Fact]
        public void Triangle_DrawWithSVG_Success()
        {
            var renderer = new SVGRenderer();
            var triangle = new Triangle(renderer, 0, 0, 100, 100, 50, 200);

            triangle.Draw();

            Assert.Single(renderer.GetSVG);
            Assert.Contains("polygon", renderer.GetSVG[0]);
        }

        [Fact]
        public void Line_SwitchRenderer_Success()
        {
            var gdiRenderer = new GDIPlusRenderer();
            var line = new Line(gdiRenderer, 0, 0, 100, 100);

            line.Draw();
            Assert.Single(gdiRenderer.GetCommands);

            var openGLRenderer = new OpenGLRenderer();
            line.SetRenderer(openGLRenderer);
            line.Draw();

            Assert.Single(openGLRenderer.GetCommands);
        }

        [Fact]
        public void Canvas_DrawMultipleShapes_Success()
        {
            var renderer = new GDIPlusRenderer();
            var canvas = new Canvas();

            canvas.AddShape(new Circle(renderer, 50, 50, 10));
            canvas.AddShape(new Rectangle(renderer, 10, 10, 50, 50));
            canvas.AddShape(new Triangle(renderer, 0, 0, 100, 100, 50, 200));

            canvas.DrawAll();

            Assert.Equal(3, renderer.GetCommands.Count);
        }

        [Fact]
        public void Canvas_ChangeRenderer_AllShapesUseNewRenderer()
        {
            var gdiRenderer = new GDIPlusRenderer();
            var canvas = new Canvas();

            canvas.AddShape(new Circle(gdiRenderer, 50, 50, 10));
            canvas.AddShape(new Rectangle(gdiRenderer, 10, 10, 50, 50));

            var openGLRenderer = new OpenGLRenderer();
            canvas.ChangeRenderer(openGLRenderer);
            canvas.DrawAll();

            Assert.Equal(2, openGLRenderer.GetCommands.Count);
        }

        [Fact]
        public void Shape_WithNullRenderer_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Circle(null, 50, 50, 10)
            );

            Assert.Contains("renderer", exception.Message);
        }

        [Fact]
        public void Renderer_GDIPlus_ToString()
        {
            var renderer = new GDIPlusRenderer();
            renderer.DrawCircle(10, 10, 5);

            var str = renderer.ToString();
            Assert.Contains("GDIPlus", str);
            Assert.Contains("1", str); // 1 command
        }

        [Fact]
        public void Renderer_OpenGL_MultipleDraws()
        {
            var renderer = new OpenGLRenderer();
            renderer.DrawCircle(0, 0, 10);
            renderer.DrawRectangle(20, 20, 50, 50);
            renderer.DrawLine(0, 0, 100, 100);

            Assert.Equal(3, renderer.GetCommands.Count);
        }

        [Fact]
        public void Renderer_SVG_GeneratesValidXML()
        {
            var renderer = new SVGRenderer();
            renderer.DrawCircle(50, 50, 25);
            renderer.DrawRectangle(0, 0, 100, 100);

            var svg = renderer.GetSVG;
            Assert.Equal(2, svg.Count);
            Assert.Contains("circle", svg[0]);
            Assert.Contains("rect", svg[1]);
        }

        [Fact]
        public void Canvas_RemoveShape_Success()
        {
            var renderer = new GDIPlusRenderer();
            var canvas = new Canvas();
            var circle = new Circle(renderer, 50, 50, 10);

            canvas.AddShape(circle);
            Assert.Equal(1, canvas.ShapeCount);

            canvas.RemoveShape(circle);
            Assert.Equal(0, canvas.ShapeCount);
        }

        [Fact]
        public void Canvas_AddNullShape_ThrowsException()
        {
            var canvas = new Canvas();

            var exception = Assert.Throws<ArgumentNullException>(() =>
                canvas.AddShape(null)
            );

            Assert.Contains("shape", exception.Message);
        }

        [Fact]
        public void Circle_ToString_ContainsInfo()
        {
            var renderer = new GDIPlusRenderer();
            var circle = new Circle(renderer, 100, 50, 25);

            var str = circle.ToString();
            Assert.Contains("100", str);
            Assert.Contains("50", str);
            Assert.Contains("25", str);
        }

        [Fact]
        public void Rectangle_ToString_ContainsInfo()
        {
            var renderer = new GDIPlusRenderer();
            var rect = new Rectangle(renderer, 10, 20, 100, 50);

            var str = rect.ToString();
            Assert.Contains("10", str);
            Assert.Contains("20", str);
            Assert.Contains("100", str);
        }

        [Fact]
        public void Renderer_Vulkan_LargeDrawCount()
        {
            var renderer = new VulkanRenderer();

            for (int i = 0; i < 100; i++)
            {
                renderer.DrawCircle(i * 10, i * 10, i);
            }

            Assert.Equal(100, renderer.GetCommands.Count);
        }

        [Fact]
        public void SetRenderer_NullRenderer_ThrowsException()
        {
            var renderer = new GDIPlusRenderer();
            var circle = new Circle(renderer, 50, 50, 10);

            var exception = Assert.Throws<ArgumentNullException>(() =>
                circle.SetRenderer(null)
            );

            Assert.Contains("renderer", exception.Message);
        }

        [Fact]
        public void Canvas_ChangeRenderer_NullRenderer_ThrowsException()
        {
            var renderer = new GDIPlusRenderer();
            var canvas = new Canvas();
            canvas.AddShape(new Circle(renderer, 50, 50, 10));

            var exception = Assert.Throws<ArgumentNullException>(() =>
                canvas.ChangeRenderer(null)
            );

            Assert.Contains("renderer", exception.Message);
        }
    }
}
