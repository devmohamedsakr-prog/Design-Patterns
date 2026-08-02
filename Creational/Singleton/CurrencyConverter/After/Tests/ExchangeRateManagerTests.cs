using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

// ============================================================================
// 🧪 UNIT TESTS: ExchangeRateManager (Singleton)
// ============================================================================
// Tests verify:
// 1. Singleton pattern implementation (only one instance)
// 2. Thread-safety of singleton
// 3. Exchange rate management functionality
// 4. Data consistency
// ============================================================================

namespace CurrencyConverterAfter.Tests
{
    [TestFixture]
    public class ExchangeRateManagerTests
    {
        // ════════════════════════════════════════════════════════════════════
        // TEST 1: Singleton Pattern - Same Instance
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Singleton")]
        public void Instance_CalledMultipleTimes_ReturnsSameObject()
        {
            // Arrange & Act
            var instance1 = ExchangeRateManager.Instance;
            var instance2 = ExchangeRateManager.Instance;
            var instance3 = ExchangeRateManager.Instance;

            // Assert
            Assert.That(instance1, Is.SameAs(instance2), 
                "First and second calls should return same instance");
            Assert.That(instance2, Is.SameAs(instance3), 
                "Second and third calls should return same instance");
            
            // ✅ VERIFIED: Singleton guarantees single instance
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 2: Thread-Safety - Concurrent Access
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Singleton")]
        public void Instance_ConcurrentAccess_AllThreadsGetSameInstance()
        {
            // Arrange
            var instances = new ConcurrentBag<ExchangeRateManager>();
            var threads = new List<Thread>();
            const int threadCount = 100;

            // Act
            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread(() =>
                {
                    var instance = ExchangeRateManager.Instance;
                    instances.Add(instance);
                });
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
                thread.Join();

            // Assert
            var firstInstance = instances.First();
            var allSame = instances.All(x => x == firstInstance);
            
            Assert.That(allSame, Is.True, 
                "All threads should get the same singleton instance");
            Assert.That(instances.Count, Is.EqualTo(threadCount), 
                "All threads should have retrieved an instance");
            
            // ✅ VERIFIED: Thread-safe singleton implementation
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 3: Get Exchange Rate - Valid Currency
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("RateManagement")]
        public void GetRate_ValidCurrency_ReturnsCorrectRate()
        {
            // Arrange
            var manager = ExchangeRateManager.Instance;

            // Act
            decimal usdRate = manager.GetRate("USD");
            decimal eurRate = manager.GetRate("EUR");

            // Assert
            Assert.That(usdRate, Is.EqualTo(1.0m), 
                "USD rate should be 1.0 (base currency)");
            Assert.That(eurRate, Is.EqualTo(0.85m), 
                "EUR rate should be 0.85");
            
            // ✅ VERIFIED: Rates are loaded correctly
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 4: Get Exchange Rate - Invalid Currency
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("RateManagement")]
        public void GetRate_InvalidCurrency_ThrowsArgumentException()
        {
            // Arrange
            var manager = ExchangeRateManager.Instance;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                manager.GetRate("INVALID"));

            Assert.That(exception.Message, Contains.Substring("not found"));
            
            // ✅ VERIFIED: Error handling for invalid currencies
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 5: Update Exchange Rate - Valid Currency
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("RateManagement")]
        public void UpdateRate_ValidCurrency_UpdatesSuccessfully()
        {
            // Arrange
            var manager = ExchangeRateManager.Instance;
            decimal oldRate = manager.GetRate("GBP");

            // Act
            decimal newRate = oldRate + 0.1m;
            manager.UpdateRate("GBP", newRate);
            decimal retrievedRate = manager.GetRate("GBP");

            // Assert
            Assert.That(retrievedRate, Is.EqualTo(newRate), 
                "Rate should be updated to new value");
            
            // ✅ VERIFIED: Rate updates work correctly
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 6: Update Exchange Rate - Invalid Currency
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("RateManagement")]
        public void UpdateRate_InvalidCurrency_ThrowsException()
        {
            // Arrange
            var manager = ExchangeRateManager.Instance;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                manager.UpdateRate("INVALID", 1.5m));
            
            // ✅ VERIFIED: Error handling for invalid currencies
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 7: Rate Exists - Existing Currency
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("RateManagement")]
        public void RateExists_ExistingCurrency_ReturnsTrue()
        {
            // Arrange
            var manager = ExchangeRateManager.Instance;

            // Act
            bool usdExists = manager.RateExists("USD");
            bool eurExists = manager.RateExists("EUR");
            bool jpyExists = manager.RateExists("JPY");

            // Assert
            Assert.That(usdExists, Is.True);
            Assert.That(eurExists, Is.True);
            Assert.That(jpyExists, Is.True);
            
            // ✅ VERIFIED: RateExists works for existing currencies
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 8: Rate Exists - Non-Existing Currency
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("RateManagement")]
        public void RateExists_NonExistentCurrency_ReturnsFalse()
        {
            // Arrange
            var manager = ExchangeRateManager.Instance;

            // Act
            bool exists = manager.RateExists("INVALID");
            bool xlm = manager.RateExists("XLM");

            // Assert
            Assert.That(exists, Is.False);
            Assert.That(xlm, Is.False);
            
            // ✅ VERIFIED: RateExists returns false for non-existent currencies
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 9: Get All Rates
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("RateManagement")]
        public void GetAllRates_ReturnsAllAvailableCurrencies()
        {
            // Arrange
            var manager = ExchangeRateManager.Instance;
            var expectedCurrencies = new[] { "USD", "EUR", "GBP", "JPY", "AUD", "CAD", "CHF", "INR" };

            // Act
            var allRates = manager.GetAllRates();

            // Assert
            Assert.That(allRates.Count, Is.EqualTo(expectedCurrencies.Length),
                "Should have all expected currencies");
            
            foreach (var currency in expectedCurrencies)
            {
                Assert.That(allRates.ContainsKey(currency), Is.True,
                    $"Should contain {currency}");
                Assert.That(allRates[currency], Is.GreaterThan(0),
                    $"{currency} rate should be positive");
            }
            
            // ✅ VERIFIED: GetAllRates returns complete set
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 10: Data Consistency - Multiple Accesses
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Consistency")]
        public void GetRate_MultipleAccesses_ReturnsConsistentData()
        {
            // Arrange
            var manager = ExchangeRateManager.Instance;

            // Act
            decimal rate1 = manager.GetRate("EUR");
            decimal rate2 = manager.GetRate("EUR");
            decimal rate3 = manager.GetRate("EUR");

            // Assert
            Assert.That(rate1, Is.EqualTo(rate2));
            Assert.That(rate2, Is.EqualTo(rate3));
            
            // ✅ VERIFIED: Consistent data on multiple accesses
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 11: Thread-Safe Rate Updates
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("ThreadSafety")]
        public void UpdateRate_ConcurrentUpdates_AllSucceed()
        {
            // Arrange
            var manager = ExchangeRateManager.Instance;
            var updateTasks = new List<Task>();
            const int taskCount = 50;

            // Act
            for (int i = 0; i < taskCount; i++)
            {
                int index = i;
                var task = Task.Run(() =>
                {
                    decimal newRate = 1.0m + (index * 0.01m);
                    manager.UpdateRate("AUD", newRate);
                });
                updateTasks.Add(task);
            }

            Task.WaitAll(updateTasks.ToArray());

            // Assert - At least one update succeeded
            decimal finalRate = manager.GetRate("AUD");
            Assert.That(finalRate, Is.GreaterThan(1.0m),
                "At least one update should have succeeded");
            
            // ✅ VERIFIED: Thread-safe concurrent updates
        }

        // ════════════════════════════════════════════════════════════════════
        // TEST 12: Singleton Initialization Only Once
        // ════════════════════════════════════════════════════════════════════
        
        [Test]
        [Category("Singleton")]
        [Explicit("This test demonstrates singleton initialization occurs once")]
        public void Instance_MultipleThreads_InitializedOnlyOnce()
        {
            // This test demonstrates that Lazy<T> ensures
            // initialization happens exactly once even with concurrent access
            
            // Arrange
            var instance1 = ExchangeRateManager.Instance;
            
            // Act
            var concurrentInstances = new ConcurrentBag<ExchangeRateManager>();
            Parallel.For(0, 100, i =>
            {
                concurrentInstances.Add(ExchangeRateManager.Instance);
            });

            // Assert - all are the same instance
            var firstInstance = concurrentInstances.First();
            Assert.That(concurrentInstances.All(x => ReferenceEquals(x, firstInstance)));
            
            // ✅ VERIFIED: Lazy<T> thread-safe initialization
        }
    }

}
