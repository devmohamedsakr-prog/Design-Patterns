using NUnit.Framework;
using XMLProcessing.After.Context;

namespace XMLProcessing.After.Tests
{
    [TestFixture]
    public class XMLProcessingTests
    {
        [Test]
        public void XMLValidator_ValidElement()
        {
            var elem = new XMLElement { TagName = "root" };
            var validator = new XMLValidator();
            elem.Accept(validator);
            Assert.That(validator.Errors.Count, Is.EqualTo(0));
        }

        [Test]
        public void XMLValidator_InvalidElement()
        {
            var elem = new XMLElement { TagName = "" };
            var validator = new XMLValidator();
            elem.Accept(validator);
            Assert.That(validator.Errors.Count, Is.GreaterThan(0));
        }

        [Test]
        public void XMLTransformer()
        {
            var elem = new XMLElement { TagName = "root" };
            elem.Children.Add(new XMLText { Content = "Hello" });
            var transformer = new XMLTransformer();
            elem.Accept(transformer);
            Assert.That(transformer.TransformedXml, Does.Contain("ROOT"));
        }
    }
}
