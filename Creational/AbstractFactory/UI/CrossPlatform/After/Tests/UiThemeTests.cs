using NUnit.Framework;
using UiThemeFactory.After.Context;

namespace UiThemeFactory.After.Tests
{
    [TestFixture]
    public class UiThemeTests
    {
        [Test]
        public void WindowsFactory_CreateButton()
        {
            var factory = new WindowsUIFactory();
            var button = factory.CreateButton();
            Assert.That(button.GetStyle(), Is.EqualTo("Windows Modern"));
        }

        [Test]
        public void MacFactory_CreateCheckbox()
        {
            var factory = new MacUIFactory();
            var checkbox = factory.CreateCheckbox();
            Assert.That(checkbox.GetStyle(), Is.EqualTo("Mac Native"));
        }

        [Test]
        public void LinuxFactory_CreateTextField()
        {
            var factory = new LinuxUIFactory();
            var textField = factory.CreateTextField();
            Assert.That(textField.GetStyle(), Is.EqualTo("Linux GTK"));
        }

        [Test]
        public void ProviderReturnsCorrectFactory_Windows()
        {
            var factory = UIFactoryProvider.GetFactory("windows");
            Assert.That(factory, Is.InstanceOf<WindowsUIFactory>());
        }

        [Test]
        public void ProviderReturnsCorrectFactory_Mac()
        {
            var factory = UIFactoryProvider.GetFactory("mac");
            Assert.That(factory, Is.InstanceOf<MacUIFactory>());
        }

        [Test]
        public void AllElementsConsistent_SameFactory()
        {
            var factory = new WindowsUIFactory();
            var button = factory.CreateButton();
            var checkbox = factory.CreateCheckbox();
            var textField = factory.CreateTextField();
            
            Assert.That(button.GetStyle(), Is.EqualTo(checkbox.GetStyle()));
            Assert.That(checkbox.GetStyle(), Is.EqualTo(textField.GetStyle()));
        }

        [Test]
        public void UIApplication_RunsSuccessfully()
        {
            var factory = new MacUIFactory();
            var app = new UIApplication(factory);
            app.RunUI();
            Assert.Pass();
        }
    }
}
