using Xunit;
using Composite.UI.Components.Component;
using System.Collections.Generic;

namespace Composite.UI.Components.Tests
{
    public class UIElementTests
    {
        [Fact]
        public void Button_Render_Success()
        {
            var button = new Button("btn1", "SubmitBtn", "Submit");
            button.Width = 100;
            button.Height = 40;

            button.Render();

            Assert.True(button.IsEnabled);
            Assert.Equal(100, button.Width);
        }

        [Fact]
        public void Label_Display_Success()
        {
            var label = new Label("lbl1", "Title", "Welcome");
            label.Width = 200;
            label.Height = 30;

            label.Display();

            Assert.Equal("Welcome", label.Text);
        }

        [Fact]
        public void TextBox_CreateAndConfigure()
        {
            var textbox = new TextBox("txt1", "InputField");
            textbox.Width = 300;
            textbox.Height = 40;
            textbox.Placeholder = "Enter text";

            Assert.Equal(300, textbox.Width);
            Assert.Equal("Enter text", textbox.Placeholder);
        }

        [Fact]
        public void Button_GetSize_Correct()
        {
            var button = new Button("btn1", "Test", "Click");
            button.Width = 100;
            button.Height = 50;

            int size = button.GetTotalSize();

            Assert.Equal(5000, size);
        }

        [Fact]
        public void Panel_AddButton_Success()
        {
            var panel = new Panel("panel1", "MainPanel");
            var button = new Button("btn1", "TestBtn", "Submit");

            panel.Add(button);

            Assert.Single(panel.GetChildren());
        }

        [Fact]
        public void Panel_AddMultipleElements()
        {
            var panel = new Panel("panel1", "Container");
            panel.Add(new Button("btn1", "B1", "OK"));
            panel.Add(new Button("btn2", "B2", "Cancel"));
            panel.Add(new Label("lbl1", "L1", "Message"));

            Assert.Equal(3, panel.GetChildren().Count);
        }

        [Fact]
        public void Panel_RemoveElement_Success()
        {
            var panel = new Panel("panel1", "Container");
            var button = new Button("btn1", "Test", "Click");

            panel.Add(button);
            Assert.Single(panel.GetChildren());

            panel.Remove(button);
            Assert.Empty(panel.GetChildren());
        }

        [Fact]
        public void Panel_GetTotalSize_IncludesChildren()
        {
            var panel = new Panel("panel1", "Parent");
            panel.Width = 400;
            panel.Height = 300;

            var button = new Button("btn1", "Test", "Click");
            button.Width = 100;
            button.Height = 50;

            panel.Add(button);

            int totalSize = panel.GetTotalSize();

            Assert.Equal(125000, totalSize); // 400*300 + 100*50
        }

        [Fact]
        public void Panel_Enable_EnablesAllChildren()
        {
            var panel = new Panel("panel1", "Container");
            var button = new Button("btn1", "Test", "Click");

            panel.Add(button);
            button.IsEnabled = false;

            panel.Enable();

            Assert.True(button.IsEnabled);
        }

        [Fact]
        public void Panel_Disable_DisablesAllChildren()
        {
            var panel = new Panel("panel1", "Container");
            var button = new Button("btn1", "Test", "Click");

            panel.Add(button);
            button.IsEnabled = true;

            panel.Disable();

            Assert.False(button.IsEnabled);
        }

        [Fact]
        public void Form_AddFields_Success()
        {
            var form = new Form("form1", "LoginForm", "User Login");
            form.AddField(new Label("lbl1", "L1", "Username"));
            form.AddField(new TextBox("txt1", "UsernameInput"));
            form.AddField(new Label("lbl2", "L2", "Password"));
            form.AddField(new TextBox("txt2", "PasswordInput"));

            Assert.Equal(4, form._fields.Count);
        }

        [Fact]
        public void Form_Render_Success()
        {
            var form = new Form("form1", "ContactForm", "Contact Us");
            form.AddField(new Label("lbl1", "L1", "Name"));
            form.AddField(new TextBox("txt1", "NameInput"));

            form.Render();

            Assert.NotEmpty(form._fields);
        }

        [Fact]
        public void NestedPanel_Hierarchy()
        {
            var mainPanel = new Panel("main", "MainPanel");
            var subPanel = new Panel("sub", "SubPanel");
            var button = new Button("btn1", "Test", "Click");

            subPanel.Add(button);
            mainPanel.Add(subPanel);

            Assert.Single(mainPanel.GetChildren());
            Assert.Single(subPanel.GetChildren());
        }

        [Fact]
        public void Button_Enable_DisableToggle()
        {
            var button = new Button("btn1", "Test", "Click");

            button.Enable();
            Assert.True(button.IsEnabled);

            button.Disable();
            Assert.False(button.IsEnabled);

            button.Enable();
            Assert.True(button.IsEnabled);
        }

        [Fact]
        public void TextBox_IsReadOnly_Disable()
        {
            var textbox = new TextBox("txt1", "Input");
            textbox.Enable();
            Assert.False(textbox.IsReadOnly);

            textbox.Disable();
            Assert.True(textbox.IsReadOnly);
        }

        [Fact]
        public void Panel_ToString_ContainsInfo()
        {
            var panel = new Panel("p1", "TestPanel");
            panel.Add(new Button("b1", "B1", "OK"));

            var str = panel.ToString();
            Assert.Contains("TestPanel", str);
            Assert.Contains("1", str);
        }

        [Fact]
        public void Form_GetTotalSize_IncludesAllFields()
        {
            var form = new Form("form1", "TestForm", "Form");
            var field1 = new Label("lbl1", "L1", "Name");
            field1.Width = 100;
            field1.Height = 30;

            form.AddField(field1);

            int size = form.GetTotalSize();
            Assert.Equal(3000, size);
        }

        [Fact]
        public void Panel_AddNull_ThrowsException()
        {
            var panel = new Panel("p1", "Test");

            var exception = Assert.Throws<ArgumentNullException>(() =>
                panel.Add(null)
            );

            Assert.Contains("element", exception.Message);
        }

        [Fact]
        public void Form_AddNull_ThrowsException()
        {
            var form = new Form("form1", "Test", "Form");

            var exception = Assert.Throws<ArgumentNullException>(() =>
                form.AddField(null)
            );

            Assert.Contains("field", exception.Message);
        }
    }
}
