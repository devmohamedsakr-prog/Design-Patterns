using Xunit;
using Prototype.UI.Component.Context;
using System;

namespace Prototype.UI.Component.Tests
{
    public class UIComponentTests
    {
        private UIComponent CreateSampleButton()
        {
            var button = new UIComponent
            {
                ComponentType = "Button",
                Text = "Click Me",
                IsEnabled = true,
                IsVisible = true
            };

            button.Style.BackgroundColor = "#0078D4";
            button.Style.ForegroundColor = "#FFFFFF";
            button.Style.FontSize = 14;
            button.Style.FontWeight = "Bold";
            button.Style.BorderRadius = 4;

            button.Layout.Width = 100;
            button.Layout.Height = 40;
            button.Layout.Padding = 10;
            button.Layout.Display = "Block";

            button.Events.OnClick = "HandleButtonClick";
            button.Events.OnHover = "HandleHover";

            button.Style.CssClasses.Add("primary-btn");
            button.Style.CssClasses.Add("interactive");

            return button;
        }

        [Fact]
        public void Clone_CreatesIndependentCopy()
        {
            var original = CreateSampleButton();
            var clone = original.Clone();

            Assert.NotSame(original, clone);
            Assert.NotEqual(original.Id, clone.Id);
            Assert.Equal(original.ComponentType, clone.ComponentType);
            Assert.NotSame(original.Style, clone.Style);
        }

        [Fact]
        public void Clone_GeneratesUniqueIds()
        {
            var original = CreateSampleButton();
            var clone1 = original.Clone();
            var clone2 = original.Clone();

            Assert.NotEqual(clone1.Id, clone2.Id);
        }

        [Fact]
        public void Clone_ChangeToCloneDoesNotAffectOriginal()
        {
            var original = CreateSampleButton();
            var clone = original.Clone();

            clone.Text = "New Text";
            clone.Style.BackgroundColor = "#FF0000";
            clone.Layout.Width = 200;

            Assert.Equal("Click Me", original.Text);
            Assert.Equal("#0078D4", original.Style.BackgroundColor);
            Assert.Equal(100, original.Layout.Width);
        }

        [Fact]
        public void Clone_DeepCopiesStyle()
        {
            var original = CreateSampleButton();
            var clone = original.Clone();

            clone.Style.ForegroundColor = "#000000";
            clone.Style.FontSize = 20;

            Assert.Equal("#FFFFFF", original.Style.ForegroundColor);
            Assert.Equal(14, original.Style.FontSize);
        }

        [Fact]
        public void Clone_DeepCopiesLayout()
        {
            var original = CreateSampleButton();
            var clone = original.Clone();

            clone.Layout.X = 100;
            clone.Layout.Y = 50;

            Assert.Equal(0, original.Layout.X);
            Assert.Equal(0, original.Layout.Y);
        }

        [Fact]
        public void Clone_DeepCopiesCssClasses()
        {
            var original = CreateSampleButton();
            var clone = original.Clone();

            clone.Style.CssClasses.Add("disabled");

            Assert.Equal(2, original.Style.CssClasses.Count);
            Assert.Equal(3, clone.Style.CssClasses.Count);
        }

        [Fact]
        public void Clone_WithChildren_Success()
        {
            var parent = new UIComponent
            {
                ComponentType = "Panel",
                Text = "Container"
            };

            var child1 = CreateSampleButton();
            child1.Text = "Button 1";

            var child2 = CreateSampleButton();
            child2.Text = "Button 2";

            parent.Children.Add(child1);
            parent.Children.Add(child2);

            var clonedParent = parent.Clone();

            Assert.Equal(2, clonedParent.Children.Count);
            Assert.Equal("Button 1", clonedParent.Children[0].Text);
            Assert.NotSame(parent.Children[0], clonedParent.Children[0]);
        }

        [Fact]
        public void Clone_ChildrenAreIndependent()
        {
            var parent = new UIComponent { ComponentType = "Panel" };
            parent.Children.Add(CreateSampleButton());

            var clonedParent = parent.Clone();
            clonedParent.Children[0].Text = "Modified";

            Assert.Equal("Click Me", parent.Children[0].Text);
        }

        [Fact]
        public void Clone_DeepCopiesEvents()
        {
            var original = CreateSampleButton();
            var clone = original.Clone();

            clone.Events.OnClick = "NewClickHandler";
            clone.Events.OnHover = null;

            Assert.Equal("HandleButtonClick", original.Events.OnClick);
            Assert.Equal("HandleHover", original.Events.OnHover);
        }

        [Fact]
        public void Clone_PreservesEnabledState()
        {
            var original = CreateSampleButton();
            var clone = original.Clone();

            clone.IsEnabled = false;

            Assert.True(original.IsEnabled);
            Assert.False(clone.IsEnabled);
        }

        [Fact]
        public void Clone_PreservesVisibilityState()
        {
            var original = CreateSampleButton();
            var clone = original.Clone();

            clone.IsVisible = false;

            Assert.True(original.IsVisible);
            Assert.False(clone.IsVisible);
        }

        [Fact]
        public void Library_RegisterAndClone_Success()
        {
            var library = new ComponentLibrary();
            var button = CreateSampleButton();

            library.RegisterComponent("PrimaryButton", button);
            var cloned = library.CloneComponent("PrimaryButton");

            Assert.NotSame(button, cloned);
            Assert.Equal("Click Me", cloned.Text);
        }

        [Fact]
        public void Library_RegisterNullComponent_ThrowsException()
        {
            var library = new ComponentLibrary();

            var exception = Assert.Throws<ArgumentNullException>(() =>
                library.RegisterComponent("Button", null)
            );

            Assert.Contains("component", exception.Message);
        }

        [Fact]
        public void Library_RegisterNullName_ThrowsException()
        {
            var library = new ComponentLibrary();
            var button = CreateSampleButton();

            var exception = Assert.Throws<ArgumentException>(() =>
                library.RegisterComponent(null, button)
            );

            Assert.Contains("Name cannot be null or empty", exception.Message);
        }

        [Fact]
        public void Library_GetComponentNotFound_ThrowsException()
        {
            var library = new ComponentLibrary();

            var exception = Assert.Throws<KeyNotFoundException>(() =>
                library.GetComponent("NonExistent")
            );

            Assert.Contains("not found", exception.Message);
        }

        [Fact]
        public void Library_MultipleComponents_Success()
        {
            var library = new ComponentLibrary();

            var button = CreateSampleButton();
            button.ComponentType = "Button";

            var textbox = new UIComponent
            {
                ComponentType = "TextBox",
                Text = "Enter text"
            };

            library.RegisterComponent("Button", button);
            library.RegisterComponent("TextBox", textbox);

            Assert.Equal(2, library.ComponentCount);
            Assert.True(library.HasComponent("Button"));
            Assert.True(library.HasComponent("TextBox"));
        }

        [Fact]
        public void Styling_Clone_Independent()
        {
            var original = new Styling
            {
                BackgroundColor = "#FFFFFF",
                FontSize = 12,
                FontWeight = "Normal"
            };
            original.CssClasses.Add("default");

            var clone = original.Clone();
            clone.BackgroundColor = "#000000";
            clone.FontSize = 14;
            clone.CssClasses.Add("custom");

            Assert.Equal("#FFFFFF", original.BackgroundColor);
            Assert.Equal(12, original.FontSize);
            Assert.Single(original.CssClasses);
        }

        [Fact]
        public void Layout_Clone_Independent()
        {
            var original = new Layout
            {
                Width = 100,
                Height = 50,
                Display = "Block",
                Padding = 10
            };

            var clone = original.Clone();
            clone.Width = 200;
            clone.Display = "Flex";

            Assert.Equal(100, original.Width);
            Assert.Equal("Block", original.Display);
        }

        [Fact]
        public void EventHandlers_Clone_Independent()
        {
            var original = new EventHandlers
            {
                OnClick = "Handler1",
                OnHover = "Handler2"
            };
            original.CustomEvents.Add("OnScroll");

            var clone = original.Clone();
            clone.OnClick = "Handler3";
            clone.CustomEvents.Add("OnResize");

            Assert.Equal("Handler1", original.OnClick);
            Assert.Single(original.CustomEvents);
        }

        [Fact]
        public void Clone_ComplexHierarchy()
        {
            var root = new UIComponent { ComponentType = "Form", Text = "MainForm" };

            var panel = new UIComponent { ComponentType = "Panel", Text = "ControlPanel" };
            var button1 = CreateSampleButton();
            button1.Text = "Save";
            var button2 = CreateSampleButton();
            button2.Text = "Cancel";

            panel.Children.Add(button1);
            panel.Children.Add(button2);
            root.Children.Add(panel);

            var cloned = root.Clone();

            Assert.Equal("MainForm", cloned.Text);
            Assert.Single(cloned.Children);
            Assert.Equal("ControlPanel", cloned.Children[0].Text);
            Assert.Equal(2, cloned.Children[0].Children.Count);
            Assert.Equal("Save", cloned.Children[0].Children[0].Text);
        }

        [Fact]
        public void UIComponent_ToString_ContainsInfo()
        {
            var component = CreateSampleButton();
            var str = component.ToString();

            Assert.Contains("Button", str);
            Assert.Contains("100", str);
            Assert.Contains("40", str);
        }

        [Fact]
        public void Library_CloneWithCustomId_Success()
        {
            var library = new ComponentLibrary();
            var button = CreateSampleButton();

            library.RegisterComponent("Button", button);
            var cloned = library.CloneComponent("Button", "custom-id-123");

            Assert.Equal("custom-id-123", cloned.Id);
        }

        [Fact]
        public void Clone_ChainedClones_AllIndependent()
        {
            var original = CreateSampleButton();
            var clone1 = original.Clone();
            var clone2 = clone1.Clone();

            clone2.Style.BackgroundColor = "#FF0000";
            clone2.Layout.Width = 300;

            Assert.Equal("#0078D4", original.Style.BackgroundColor);
            Assert.Equal("#0078D4", clone1.Style.BackgroundColor);
            Assert.Equal(100, original.Layout.Width);
            Assert.Equal(100, clone1.Layout.Width);
        }

        [Fact]
        public void Library_HasComponent_ChecksCorrectly()
        {
            var library = new ComponentLibrary();
            library.RegisterComponent("Button", CreateSampleButton());

            Assert.True(library.HasComponent("Button"));
            Assert.False(library.HasComponent("TextBox"));
        }
    }
}
