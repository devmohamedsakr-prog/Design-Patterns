using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ============================================================================
// SINGLETON: ExchangeRateManager
// ============================================================================
// Responsibility: Manage and provide exchange rates
// 
// This class is responsible for:
// - Loading exchange rates from external source
// - Caching rates in memory
// - Providing thread-safe access to rates
// - Managing rate updates
//
// It SHOULD NOT handle:
// - Currency conversion logic
// - Logging or output
// - Decorator functionality
// ============================================================================

namespace CurrencyConverterAfter
{
    public sealed class ExchangeRateManager
    {
        // Thread-safe lazy singleton using .NET built-in Lazy<T>
        private static readonly Lazy<ExchangeRateManager> instance = 
            new Lazy<ExchangeRateManager>(() => new ExchangeRateManager());

        private Dictionary<string, decimal> exchangeRates;
        private readonly object lockObject = new object(); // For thread-safety

        /// <summary>
        /// Gets the singleton instance of ExchangeRateManager.
        /// Thread-safe lazy initialization.
        /// </summary>
        public static ExchangeRateManager Instance => instance.Value;

        /// <summary>
        /// Private constructor - prevents external instantiation.
        /// Called only once by Lazy<T> at first access.
        /// </summary>
        private ExchangeRateManager()
        {
            Console.WriteLine("🔄 [ExchangeRateManager] Initializing Singleton...");
            Console.WriteLine("📡 [ExchangeRateManager] Loading exchange rates from server...");
            
            // Only ONE API call happens here - at first access
            Task.Delay(1000).Wait(); // Simulate network latency
            
            exchangeRates = new Dictionary<string, decimal>
            {
                { "USD", 1.0m },
                { "EUR", 0.85m },
                { "GBP", 0.73m },
                { "JPY", 110.50m },
                { "AUD", 1.35m },
                { "CAD", 1.25m },
                { "CHF", 0.92m },
                { "INR", 74.50m }
            };

            Console.WriteLine("✅ [ExchangeRateManager] Singleton initialized (50KB in memory)\n");
        }

        /// <summary>
        /// Gets the exchange rate for a specific currency.
        /// Thread-safe operation.
        /// </summary>
        /// <param name="currency">Currency code (e.g., "USD", "EUR")</param>
        /// <returns>Exchange rate relative to base currency</returns>
        /// <exception cref="ArgumentException">Thrown if currency not found</exception>
        public decimal GetRate(string currency)
        {
            lock (lockObject)
            {
                if (!exchangeRates.ContainsKey(currency))
                    throw new ArgumentException($"Currency {currency} not found");
                
                return exchangeRates[currency];
            }
        }

        /// <summary>
        /// Updates the exchange rate for a specific currency.
        /// Changes are immediately visible to all consumers.
        /// Thread-safe operation.
        /// </summary>
        /// <param name="currency">Currency code to update</param>
        /// <param name="newRate">New exchange rate value</param>
        /// <exception cref="ArgumentException">Thrown if currency not found</exception>
        public void UpdateRate(string currency, decimal newRate)
        {
            lock (lockObject)
            {
                if (!exchangeRates.ContainsKey(currency))
                    throw new ArgumentException($"Currency {currency} not found");
                
                exchangeRates[currency] = newRate;
                Console.WriteLine($"📊 [ExchangeRateManager] Updated {currency} to {newRate}");
            }
        }

        /// <summary>
        /// Checks if a currency rate is available.
        /// Thread-safe operation.
        /// </summary>
        public bool RateExists(string currency)
        {
            lock (lockObject)
            {
                return exchangeRates.ContainsKey(currency);
            }
        }

        /// <summary>
        /// Gets all available exchange rates.
        /// Returns a copy to prevent external modifications.
        /// Thread-safe operation.
        /// </summary>
        public Dictionary<string, decimal> GetAllRates()
        {
            lock (lockObject)
            {
                return new Dictionary<string, decimal>(exchangeRates);
            }
        }
    }
}
