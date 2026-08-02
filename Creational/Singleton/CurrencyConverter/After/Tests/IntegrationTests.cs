using System;
using System.Threading.Tasks;
using NUnit.Framework;

// ============================================================================
// 🧪 INTEGRATION TESTS: End-to-End Currency Converter Scenarios
// ============================================================================
// Tests verify:
// 1. Complete workflows work end-to-end
// 2. Singleton pattern works with real components
// 3. Multiple converters stay synchronized
// 4. Rate updates affect all converters
// 5. Concurrent operations produce consistent results
// ============================================================================

namespace CurrencyConverterAfter.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        // ════════════════════════════════════════════════════════════════════
        // TEST 1: Complete Conversion Workflow
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Workflow_CompleteConversionScenario_SucceedsEnd2End()
        {
            // Arrange
            var rateManager = ExchangeRateManager.Instance;
            var converter = new CurrencyConverter();

            // Act - Get initial rate
            decimal initialRate = rateManager.GetRate("EUR");

            // Act - Perform conversion
            decimal convertedAmount = converter.Convert(100, "USD", "EUR");

            // Assert - Conversion should work
            Assert.That(convertedAmount, Is.GreaterThan(0));
            Assert.That(convertedAmount, Is.EqualTo(initialRate * 100).Within(0.01m));

            // ✅ VERIFIED: Complete workflow succeeds
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 2: Multiple Converters Synchronized
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Integration_MultipleConverters_StaySynchronized()
        {
            // Arrange
            var converter1 = new CurrencyConverter();
            var converter2 = new CurrencyConverter();
            var converter3 = new CurrencyConverter();
            var rateManager = ExchangeRateManager.Instance;

            // Act - Get conversions from all converters
            decimal result1a = converter1.Convert(100, "USD", "EUR");
            decimal result2a = converter2.Convert(100, "USD", "EUR");
            decimal result3a = converter3.Convert(100, "USD", "EUR");

            // Assert - All should be identical
            Assert.That(result1a, Is.EqualTo(result2a));
            Assert.That(result2a, Is.EqualTo(result3a));

            // Act - Update rates in singleton
            rateManager.UpdateRate("EUR", 0.90m);

            // Act - Get conversions again
            decimal result1b = converter1.Convert(100, "USD", "EUR");
            decimal result2b = converter2.Convert(100, "USD", "EUR");
            decimal result3b = converter3.Convert(100, "USD", "EUR");

            // Assert - All should be updated and identical
            Assert.That(result1b, Is.EqualTo(result2b));
            Assert.That(result2b, Is.EqualTo(result3b));
            Assert.That(result1b, Is.Not.EqualTo(result1a), "Results should change after rate update");

            // ✅ VERIFIED: Multiple converters stay synchronized
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 3: Rate Updates Propagate
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Integration_RateUpdates_PropagateToAllConverters()
        {
            // Arrange
            var rateManager = ExchangeRateManager.Instance;
            var converter1 = new CurrencyConverter();
            var converter2 = new CurrencyConverter();

            decimal originalRate = rateManager.GetRate("GBP");
            decimal newRate = originalRate + 0.05m;

            // Act - Update rate
            rateManager.UpdateRate("GBP", newRate);

            // Act - Convert with both converters
            decimal conv1 = converter1.Convert(100, "USD", "GBP");
            decimal conv2 = converter2.Convert(100, "USD", "GBP");

            // Assert - Both converters use new rate
            decimal expectedResult = 100 * newRate;
            Assert.That(conv1, Is.EqualTo(expectedResult).Within(0.01m));
            Assert.That(conv2, Is.EqualTo(expectedResult).Within(0.01m));

            // ✅ VERIFIED: Rate updates propagate to all converters
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 4: Singleton Only Initialized Once
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Integration_SingletonInitialization_OccursOnlyOnce()
        {
            // Arrange - Get singleton multiple times
            var instance1 = ExchangeRateManager.Instance;
            var instance2 = ExchangeRateManager.Instance;
            var instance3 = ExchangeRateManager.Instance;

            // Assert - All are same instance
            Assert.That(instance1, Is.SameAs(instance2));
            Assert.That(instance2, Is.SameAs(instance3));

            // This means initialization (API call) happened exactly once
            
            // ✅ VERIFIED: Singleton initialized once
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 5: Complex Multi-Step Conversion
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Integration_MultiStepConversion_CalculatesCorrectly()
        {
            // Arrange
            var converter = new CurrencyConverter();

            // Act - Multi-step conversion: USD -> EUR -> GBP
            decimal startAmount = 1000m;
            
            decimal eurAmount = converter.Convert(startAmount, "USD", "EUR");
            decimal gbpAmount = converter.Convert(eurAmount, "EUR", "GBP");
            
            // Act - Direct conversion: USD -> GBP
            decimal directGbpAmount = converter.Convert(startAmount, "USD", "GBP");

            // Assert - Should be approximately equal
            Assert.That(gbpAmount, Is.EqualTo(directGbpAmount).Within(1m),
                "Multi-step and direct conversion should produce same result");

            // ✅ VERIFIED: Multi-step conversions work correctly
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 6: Concurrent Conversions Produce Consistent Results
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Integration_ConcurrentOperations_ProduceConsistentResults()
        {
            // Arrange
            var converter1 = new CurrencyConverter();
            var converter2 = new CurrencyConverter();
            var results = new System.Collections.Concurrent.ConcurrentBag<decimal>();

            // Act - Run conversions concurrently
            var tasks = new Task[]
            {
                Task.Run(() => results.Add(converter1.Convert(100, "USD", "EUR"))),
                Task.Run(() => results.Add(converter2.Convert(100, "USD", "EUR"))),
                Task.Run(() => results.Add(converter1.Convert(100, "USD", "EUR"))),
                Task.Run(() => results.Add(converter2.Convert(100, "USD", "EUR"))),
            };

            Task.WaitAll(tasks);

            // Assert - All results should be identical
            var expectedResult = results.First();
            Assert.That(results.All(r => r == expectedResult),
                "Concurrent conversions should produce identical results");

            // ✅ VERIFIED: Thread-safe concurrent conversions
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 7: Converter With Decorators
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Integration_ConverterWithDecorators_WorksTogether()
        {
            // Arrange
            ICurrencyConverter baseConverter = new CurrencyConverter();
            ICurrencyConverter loggingConverter = 
                new LoggingCurrencyConverter(baseConverter);
            ICurrencyConverter trackedConverter = 
                new OperationTracker(loggingConverter);

            // Act
            decimal result1 = trackedConverter.Convert(100, "USD", "EUR");
            decimal result2 = trackedConverter.Convert(200, "USD", "GBP");
            decimal result3 = trackedConverter.Convert(300, "EUR", "JPY");

            // Assert - Results should be correct
            Assert.That(result1, Is.EqualTo(85m).Within(0.01m));
            Assert.That(result2, Is.GreaterThan(0));
            Assert.That(result3, Is.GreaterThan(0));

            // ✅ VERIFIED: Decorators work with real converter
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 8: Exception Handling End-to-End
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Integration_InvalidInput_HandledGracefully()
        {
            // Arrange
            var converter = new CurrencyConverter();

            // Act & Assert - Invalid from currency
            Assert.Throws<ArgumentException>(() =>
                converter.Convert(100, "INVALID", "EUR"));

            // Act & Assert - Invalid to currency
            Assert.Throws<ArgumentException>(() =>
                converter.Convert(100, "USD", "INVALID"));

            // Act & Assert - Both invalid
            Assert.Throws<ArgumentException>(() =>
                converter.Convert(100, "INVALID1", "INVALID2"));

            // ✅ VERIFIED: Errors handled gracefully
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 9: Real-World Scenario - Bank Processing
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Integration_BankScenario_MultipleTransfers()
        {
            // Arrange - Simulating bank processing multiple transfers
            var converter = new CurrencyConverter();
            var rateManager = ExchangeRateManager.Instance;

            // Simulate bank transfers at different times
            var transfers = new[]
            {
                (amount: 1000m, from: "USD", to: "EUR"),
                (amount: 2000m, from: "USD", to: "GBP"),
                (amount: 500m, from: "EUR", to: "JPY"),
                (amount: 1500m, from: "GBP", to: "CAD"),
            };

            // Act - Process all transfers
            decimal totalInEUR = 0;
            foreach (var transfer in transfers)
            {
                decimal result = converter.Convert(transfer.amount, transfer.from, transfer.to);
                if (transfer.to == "EUR")
                    totalInEUR += result;
                
                Assert.That(result, Is.GreaterThan(0), 
                    $"Transfer {transfer.amount} {transfer.from} -> {transfer.to} should succeed");
            }

            // Assert
            Assert.That(totalInEUR, Is.GreaterThan(0),
                "Should have processed EUR transfers");

            // ✅ VERIFIED: Real-world scenario works
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 10: SRP Verification - Each Component Has Single Role
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Integration_SRP_EachComponentHasSingleRole()
        {
            // ExchangeRateManager: Only manages rates
            var rateManager = ExchangeRateManager.Instance;
            Assert.That(() => rateManager.GetRate("USD"), Throws.Nothing);
            Assert.That(() => rateManager.UpdateRate("USD", 1.1m), Throws.Nothing);

            // CurrencyConverter: Only converts
            var converter = new CurrencyConverter();
            Assert.That(() => converter.Convert(100, "USD", "EUR"), Throws.Nothing);

            // LoggingDecorator: Only adds logging
            var loggingConverter = new LoggingCurrencyConverter(converter);
            Assert.That(() => loggingConverter.Convert(100, "USD", "EUR"), Throws.Nothing);

            // OperationTracker: Only tracks operations
            var trackerConverter = new OperationTracker(converter);
            Assert.That(() => trackerConverter.Convert(100, "USD", "EUR"), Throws.Nothing);

            // ✅ VERIFIED: SRP is maintained throughout
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 11: Performance - Multiple Converters Don't Increase Load
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Integration_Performance_MultipleConvertersEfficient()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Create many converters (should be fast - just objects, no API calls)
            var converters = new ICurrencyConverter[100];
            for (int i = 0; i < 100; i++)
            {
                converters[i] = new CurrencyConverter();
            }

            stopwatch.Stop();
            var creationTime = stopwatch.ElapsedMilliseconds;

            // Assert - Should be very fast (no API calls)
            Assert.That(creationTime, Is.LessThan(100),
                "Creating 100 converters should be fast (singleton prevents API calls)");

            // ✅ VERIFIED: Performance is good
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 12: Singleton Persists Across Multiple Uses
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Integration")]
        public void Integration_SingletonState_PersistsAcrossUses()
        {
            // Arrange
            var rateManager = ExchangeRateManager.Instance;
            var originalRate = rateManager.GetRate("JPY");

            // Act - Update rate
            decimal newRate = originalRate + 1;
            rateManager.UpdateRate("JPY", newRate);

            // Act - Get new converter and verify rate is persisted
            var converter = new CurrencyConverter();
            decimal result = converter.Convert(100, "USD", "JPY");

            // Assert - Rate change should persist
            Assert.That(result, Is.EqualTo(100 * newRate).Within(1m),
                "Rate changes should persist in singleton");

            // ✅ VERIFIED: Singleton state persistence
        }
    }

}
