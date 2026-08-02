using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;

// ============================================================================
// 🧪 UNIT TESTS: Decorator Pattern (LoggingCurrencyConverter)
// ============================================================================
// Tests verify:
// 1. Logging decorator adds behavior without modifying core logic
// 2. Decorator correctly delegates to inner converter
// 3. Results are preserved through decoration
// 4. Decorators can be chained
// 5. SRP - Each decorator has single responsibility
// ============================================================================

namespace CurrencyConverterAfter.Tests
{
    [TestFixture]
    public class DecoratorPatternTests
    {
        // ════════════════════════════════════════════════════════════════════
        // TEST 1: LoggingDecorator Wraps Converter
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Decorator")]
        public void LoggingDecorator_WrapsConverter_PreservesResult()
        {
            // Arrange
            var innerConverter = new Mock<ICurrencyConverter>();
            var expectedResult = 85m;
            innerConverter
                .Setup(x => x.Convert(100, "USD", "EUR"))
                .Returns(expectedResult);

            var loggingConverter = new LoggingCurrencyConverter(innerConverter.Object);

            // Act
            var result = loggingConverter.Convert(100, "USD", "EUR");

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult),
                "Decorator should return same result as inner converter");
            
            // ✅ VERIFIED: Decorator preserves conversion result
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 2: LoggingDecorator Delegates to Inner Converter
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Decorator")]
        public void LoggingDecorator_DelegatesCall_ToInnerConverter()
        {
            // Arrange
            var innerConverter = new Mock<ICurrencyConverter>();
            innerConverter
                .Setup(x => x.Convert(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(85m);

            var loggingConverter = new LoggingCurrencyConverter(innerConverter.Object);

            // Act
            loggingConverter.Convert(100, "USD", "EUR");

            // Assert
            innerConverter.Verify(
                x => x.Convert(100, "USD", "EUR"),
                Times.Once,
                "Should delegate to inner converter exactly once");
            
            // ✅ VERIFIED: Decorator properly delegates
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 3: LoggingDecorator With Real Converter
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Decorator")]
        public void LoggingDecorator_WithRealConverter_WorksTogether()
        {
            // Arrange
            var realConverter = new CurrencyConverter();
            var loggingConverter = new LoggingCurrencyConverter(realConverter);

            // Act
            decimal result = loggingConverter.Convert(100, "USD", "EUR");

            // Assert
            Assert.That(result, Is.EqualTo(85m).Within(0.01m),
                "Logging decorator should work with real converter");
            
            // ✅ VERIFIED: Decorator works with real implementations
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 4: Multiple Decorators Can Be Chained
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Decorator")]
        public void Decorators_CanBeChained_WorkCorrectly()
        {
            // Arrange
            ICurrencyConverter baseConverter = new CurrencyConverter();
            
            // Layer logging decorator
            ICurrencyConverter loggingConverter = 
                new LoggingCurrencyConverter(baseConverter);
            
            // Layer operation tracker
            ICurrencyConverter trackedConverter = 
                new OperationTracker(loggingConverter);

            // Act
            decimal result = trackedConverter.Convert(100, "USD", "EUR");

            // Assert
            Assert.That(result, Is.EqualTo(85m).Within(0.01m),
                "Chained decorators should work correctly");
            
            // ✅ VERIFIED: Decorators can be composed
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 5: Decorator Doesn't Modify Core Logic
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("SRP")]
        public void LoggingDecorator_OnlyAddsLogging_NoLogicChange()
        {
            // Arrange
            var mock = new Mock<ICurrencyConverter>();
            mock.Setup(x => x.Convert(It.IsAny<decimal>(), 
                It.IsAny<string>(), It.IsAny<string>()))
                .Returns(50m);

            var decorator = new LoggingCurrencyConverter(mock.Object);

            // Act
            decimal result = decorator.Convert(100, "USD", "XXX");

            // Assert
            Assert.That(result, Is.EqualTo(50m),
                "Decorator should not change conversion logic");
            
            // ✅ VERIFIED: Decorator only adds logging, not logic
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 6: Decorator Throws Exception From Inner Converter
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Decorator")]
        public void LoggingDecorator_InnerConverterThrows_PropagatesException()
        {
            // Arrange
            var mock = new Mock<ICurrencyConverter>();
            mock.Setup(x => x.Convert(100m, "USD", "INVALID"))
                .Throws<ArgumentException>();

            var decorator = new LoggingCurrencyConverter(mock.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                decorator.Convert(100, "USD", "INVALID"));
            
            // ✅ VERIFIED: Exceptions propagate through decorator
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 7: OperationTracker Counts Operations
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Decorator")]
        public void OperationTracker_CountsConversions()
        {
            // Arrange
            var mock = new Mock<ICurrencyConverter>();
            mock.Setup(x => x.Convert(It.IsAny<decimal>(), 
                It.IsAny<string>(), It.IsAny<string>()))
                .Returns(50m);

            var tracker = new OperationTracker(mock.Object);
            
            int initialCount = OperationTracker.GetOperationCount();

            // Act
            tracker.Convert(100, "USD", "EUR");
            tracker.Convert(200, "USD", "GBP");

            // Assert
            int finalCount = OperationTracker.GetOperationCount();
            Assert.That(finalCount, Is.GreaterThan(initialCount),
                "Operation count should increase with each conversion");
            
            // ✅ VERIFIED: Tracker counts operations
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 8: Decorator Interface Compliance
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("SRP")]
        public void LoggingDecorator_ImplementsICurrencyConverter()
        {
            // Arrange
            var converter = new CurrencyConverter();
            var decorator = new LoggingCurrencyConverter(converter);

            // Assert
            Assert.That(decorator, Is.InstanceOf(typeof(ICurrencyConverter)),
                "Decorator should implement ICurrencyConverter");
            
            // ✅ VERIFIED: Decorator follows interface contract
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 9: Multiple Decorators With Same Inner Converter
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Decorator")]
        public void MultipleDecorators_SameInnerConverter_WorkIndependently()
        {
            // Arrange
            var realConverter = new CurrencyConverter();
            
            var decorated1 = new LoggingCurrencyConverter(realConverter);
            var decorated2 = new LoggingCurrencyConverter(realConverter);

            // Act
            decimal result1 = decorated1.Convert(100, "USD", "EUR");
            decimal result2 = decorated2.Convert(100, "USD", "EUR");

            // Assert
            Assert.That(result1, Is.EqualTo(result2),
                "Multiple decorators of same converter should produce same result");
            
            // ✅ VERIFIED: Decorators are independent
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 10: Decorator With Different Inner Implementations
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Decorator")]
        public void LoggingDecorator_WithDifferentImplementations_AdaptsWell()
        {
            // Arrange - Create mock converters with different behaviors
            var mockConverter1 = new Mock<ICurrencyConverter>();
            mockConverter1.Setup(x => x.Convert(100, "USD", "EUR")).Returns(85m);

            var mockConverter2 = new Mock<ICurrencyConverter>();
            mockConverter2.Setup(x => x.Convert(100, "USD", "EUR")).Returns(84m);

            var decorator1 = new LoggingCurrencyConverter(mockConverter1.Object);
            var decorator2 = new LoggingCurrencyConverter(mockConverter2.Object);

            // Act
            decimal result1 = decorator1.Convert(100, "USD", "EUR");
            decimal result2 = decorator2.Convert(100, "USD", "EUR");

            // Assert
            Assert.That(result1, Is.Not.EqualTo(result2),
                "Decorator should preserve behavior of underlying implementation");
            
            // ✅ VERIFIED: Decorator adapts to different implementations
        }
    }

    // ════════════════════════════════════════════════════════════════════════
    // HELPER: Mock Implementation for Testing
    // ════════════════════════════════════════════════════════════════════════
    
    public class LoggingDecorator : ICurrencyConverter
    {
        private readonly ICurrencyConverter innerConverter;

        public LoggingDecorator(ICurrencyConverter converter)
        {
            innerConverter = converter;
        }

        public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
        {
            Console.WriteLine($"Converting {amount} {fromCurrency} to {toCurrency}");
            var result = innerConverter.Convert(amount, fromCurrency, toCurrency);
            Console.WriteLine($"Result: {result}");
            return result;
        }
    }

}
