using System;

namespace UiThemeFactory.After.Context
{
    // Abstract product families
    public interface IButton
    {
        void Click();
        string GetStyle();
    }

    public interface ICheckbox
    {
        void Toggle();
        string GetStyle();
    }

    public interface ITextField
    {
        void SetText(string text);
        string GetStyle();
    }

    // Abstract factory
    public interface IUIFactory
    {
        IButton CreateButton();
        ICheckbox CreateCheckbox();
        ITextField CreateTextField();
    }

    // Windows implementations
    public class WindowsButton : IButton
    {
        public void Click() => Console.WriteLine("🪟 Windows Button clicked - flat design");
        public string GetStyle() => "Windows Modern";
    }

    public class WindowsCheckbox : ICheckbox
    {
        public void Toggle() => Console.WriteLine("🪟 Windows Checkbox toggled - metro style");
        public string GetStyle() => "Windows Modern";
    }

    public class WindowsTextField : ITextField
    {
        public void SetText(string text) => Console.WriteLine($"🪟 Windows TextField: {text}");
        public string GetStyle() => "Windows Modern";
    }

    // Mac implementations
    public class MacButton : IButton
    {
        public void Click() => Console.WriteLine("🍎 Mac Button clicked - rounded design");
        public string GetStyle() => "Mac Native";
    }

    public class MacCheckbox : ICheckbox
    {
        public void Toggle() => Console.WriteLine("🍎 Mac Checkbox toggled - aqua style");
        public string GetStyle() => "Mac Native";
    }

    public class MacTextField : ITextField
    {
        public void SetText(string text) => Console.WriteLine($"🍎 Mac TextField: {text}");
        public string GetStyle() => "Mac Native";
    }

    // Linux implementations
    public class LinuxButton : IButton
    {
        public void Click() => Console.WriteLine("🐧 Linux Button clicked - GTK design");
        public string GetStyle() => "Linux GTK";
    }

    public class LinuxCheckbox : ICheckbox
    {
        public void Toggle() => Console.WriteLine("🐧 Linux Checkbox toggled - GTK style");
        public string GetStyle() => "Linux GTK";
    }

    public class LinuxTextField : ITextField
    {
        public void SetText(string text) => Console.WriteLine($"🐧 Linux TextField: {text}");
        public string GetStyle() => "Linux GTK";
    }

    // Concrete factories
    public class WindowsUIFactory : IUIFactory
    {
        public IButton CreateButton() => new WindowsButton();
        public ICheckbox CreateCheckbox() => new WindowsCheckbox();
        public ITextField CreateTextField() => new WindowsTextField();
    }

    public class MacUIFactory : IUIFactory
    {
        public IButton CreateButton() => new MacButton();
        public ICheckbox CreateCheckbox() => new MacCheckbox();
        public ITextField CreateTextField() => new MacTextField();
    }

    public class LinuxUIFactory : IUIFactory
    {
        public IButton CreateButton() => new LinuxButton();
        public ICheckbox CreateCheckbox() => new LinuxCheckbox();
        public ITextField CreateTextField() => new LinuxTextField();
    }

    // Factory selector
    public class UIFactoryProvider
    {
        public static IUIFactory GetFactory(string platform)
        {
            return platform.ToLower() switch
            {
                "windows" => new WindowsUIFactory(),
                "mac" => new MacUIFactory(),
                "linux" => new LinuxUIFactory(),
                _ => throw new ArgumentException($"Unknown platform: {platform}")
            };
        }
    }

    // Application
    public class UIApplication
    {
        private IButton _button;
        private ICheckbox _checkbox;
        private ITextField _textField;

        public UIApplication(IUIFactory factory)
        {
            _button = factory.CreateButton();
            _checkbox = factory.CreateCheckbox();
            _textField = factory.CreateTextField();
        }

        public void RunUI()
        {
            Console.WriteLine($"\n🎨 UI Application started");
            _button.Click();
            _checkbox.Toggle();
            _textField.SetText("Hello Platform!");
            Console.WriteLine($"✓ All UI elements using style: {_button.GetStyle()}");
        }
    }
}
