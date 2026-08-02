using Xunit;
using Composite.Graphics.SceneGraph.Component;

namespace Composite.Graphics.SceneGraph.Tests
{
    public class GraphicsTests
    {
        [Fact]
        public void Circle_Create_Success()
        {
            var circle = new Circle("circle1", 25);

            Assert.Equal("circle1", circle.Name);
            Assert.Equal(25, circle.Radius);
            Assert.True(circle.IsVisible);
        }

        [Fact]
        public void Circle_GetBoundingBox()
        {
            var circle = new Circle("circle1", 50);

            int size = circle.GetBoundingBoxSize();

            Assert.Equal(100, size);
        }

        [Fact]
        public void Rectangle_Create_Success()
        {
            var rect = new Rectangle("rect1", 100, 200);

            Assert.Equal(100, rect.Width);
            Assert.Equal(200, rect.Height);
        }

        [Fact]
        public void Rectangle_GetBoundingBox()
        {
            var rect = new Rectangle("rect1", 100, 50);

            int size = rect.GetBoundingBoxSize();

            Assert.Equal(5000, size);
        }

        [Fact]
        public void Line_Create_Success()
        {
            var line = new Line("line1", 100, 100);

            Assert.Equal(100, line.X2);
            Assert.Equal(100, line.Y2);
        }

        [Fact]
        public void Circle_Rotate_Success()
        {
            var circle = new Circle("c1", 10);

            circle.Rotate(45);
            Assert.Equal(45, circle.Rotation);

            circle.Rotate(45);
            Assert.Equal(90, circle.Rotation);
        }

        [Fact]
        public void Circle_Scale_Success()
        {
            var circle = new Circle("c1", 10);

            circle.Scale(2, 2);

            Assert.Equal(2, circle.ScaleX);
            Assert.Equal(2, circle.ScaleY);
        }

        [Fact]
        public void Circle_Translate_Success()
        {
            var circle = new Circle("c1", 10);

            circle.Translate(10, 20);

            Assert.Equal(10, circle.X);
            Assert.Equal(20, circle.Y);
        }

        [Fact]
        public void Group_AddElement_Success()
        {
            var group = new Group("group1");
            var circle = new Circle("c1", 20);

            group.Add(circle);

            Assert.Single(group.GetChildren());
        }

        [Fact]
        public void Group_AddMultipleElements()
        {
            var group = new Group("group1");
            group.Add(new Circle("c1", 10));
            group.Add(new Rectangle("r1", 50, 50));
            group.Add(new Line("l1", 100, 100));

            Assert.Equal(3, group.GetChildren().Count);
        }

        [Fact]
        public void Group_Remove_Success()
        {
            var group = new Group("group1");
            var circle = new Circle("c1", 10);

            group.Add(circle);
            Assert.Single(group.GetChildren());

            group.Remove(circle);
            Assert.Empty(group.GetChildren());
        }

        [Fact]
        public void Group_Rotate_AppliesRecursively()
        {
            var group = new Group("group1");
            var circle = new Circle("c1", 10);

            group.Add(circle);
            group.Rotate(45);

            Assert.Equal(45, group.Rotation);
            Assert.Equal(45, circle.Rotation);
        }

        [Fact]
        public void Group_Scale_AppliesRecursively()
        {
            var group = new Group("group1");
            var rect = new Rectangle("r1", 100, 50);

            group.Add(rect);
            group.Scale(2, 2);

            Assert.Equal(2, group.ScaleX);
            Assert.Equal(200, rect.Width);
            Assert.Equal(100, rect.Height);
        }

        [Fact]
        public void Group_Translate_AppliesRecursively()
        {
            var group = new Group("group1");
            var circle = new Circle("c1", 10);

            group.Add(circle);
            group.Translate(50, 100);

            Assert.Equal(50, group.X);
            Assert.Equal(100, group.Y);
            Assert.Equal(50, circle.X);
            Assert.Equal(100, circle.Y);
        }

        [Fact]
        public void Group_GetBoundingBox_SumOfChildren()
        {
            var group = new Group("group1");
            group.Add(new Circle("c1", 20));
            group.Add(new Rectangle("r1", 50, 50));

            int size = group.GetBoundingBoxSize();

            Assert.Equal(40 + 2500, size);
        }

        [Fact]
        public void Layer_AddElement_Success()
        {
            var layer = new Layer("layer1", 1);
            var circle = new Circle("c1", 10);

            layer.AddElement(circle);

            Assert.NotEmpty(layer.GetChildren());
        }

        [Fact]
        public void Layer_ZIndex_Priority()
        {
            var layer1 = new Layer("bg", 0);
            var layer2 = new Layer("fg", 10);

            Assert.Equal(0, layer1.ZIndex);
            Assert.Equal(10, layer2.ZIndex);
        }

        [Fact]
        public void NestedGroup_Hierarchy()
        {
            var mainGroup = new Group("main");
            var subGroup = new Group("sub");
            var circle = new Circle("c1", 10);

            subGroup.Add(circle);
            mainGroup.Add(subGroup);

            Assert.Single(mainGroup.GetChildren());
            Assert.Single(subGroup.GetChildren());
        }

        [Fact]
        public void NestedGroup_Transform_Recursive()
        {
            var mainGroup = new Group("main");
            var subGroup = new Group("sub");
            var circle = new Circle("c1", 10);

            subGroup.Add(circle);
            mainGroup.Add(subGroup);

            mainGroup.Translate(10, 20);

            Assert.Equal(10, circle.X);
            Assert.Equal(20, circle.Y);
        }

        [Fact]
        public void Circle_Draw_Success()
        {
            var circle = new Circle("c1", 20);
            circle.Draw();

            Assert.NotNull(circle);
        }

        [Fact]
        public void Group_Draw_DrawsChildren()
        {
            var group = new Group("group1");
            group.Add(new Circle("c1", 10));
            group.Add(new Rectangle("r1", 50, 50));

            group.Draw();

            Assert.Equal(2, group.GetChildren().Count);
        }

        [Fact]
        public void Circle_ToString_ContainsInfo()
        {
            var circle = new Circle("circle1", 50);

            var str = circle.ToString();
            Assert.Contains("circle1", str);
            Assert.Contains("50", str);
        }

        [Fact]
        public void Group_ToString_ContainsChildCount()
        {
            var group = new Group("group1");
            group.Add(new Circle("c1", 10));
            group.Add(new Circle("c2", 20));

            var str = group.ToString();
            Assert.Contains("group1", str);
            Assert.Contains("2", str);
        }

        [Fact]
        public void Group_AddNull_ThrowsException()
        {
            var group = new Group("group1");

            var exception = Assert.Throws<ArgumentNullException>(() =>
                group.Add(null)
            );

            Assert.Contains("element", exception.Message);
        }

        [Fact]
        public void Layer_AddNull_ThrowsException()
        {
            var layer = new Layer("layer1");

            var exception = Assert.Throws<ArgumentNullException>(() =>
                layer.AddElement(null)
            );

            Assert.Contains("element", exception.Message);
        }
    }
}
