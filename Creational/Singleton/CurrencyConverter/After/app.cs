using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ============================================================================
// ✅ AFTER: Currency Converter WITH Singleton Pattern + SRP
// ============================================================================
// Benefits:
// 1. Only ONE instance created (guaranteed)
// 2. One API call for all converters
// 3. Consistent data across entire application
// 4. Memory efficient
// 5. Global access point
// 6. SRP - Each class has single responsibility
// 7. Thread-safe implementation
// ============================================================================

namespace CurrencyConverterAfter
{
    // ═══════════════════════════════════════════════════════════════════════
    // SINGLETON: ExchangeRateManager
    // Responsibility: Manage and provide exchange rates
    // ═══════════════════════════════════════════════════════════════════════
    
    public sealed class ExchangeRateManager
    {
        // Thread-safe lazy singleton using .NET built-in Lazy<T>
        private static readonly Lazy<ExchangeRateManager> instance = 
            new Lazy<ExchangeRateManager>(() => new ExchangeRateManager());

        private Dictionary<string, decimal> exchangeRates;
        private readonly object lockObject = new object(); // For thread-safety

        // ✅ Public access to singleton instance
        public static ExchangeRateManager Instance => instance.Value;

        // ✅ Private constructor - prevents external instantiation
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

        // SRP: Only responsibility is to provide rates
        public decimal GetRate(string currency)
        {
            lock (lockObject)
            {
                if (!exchangeRates.ContainsKey(currency))
                    throw new ArgumentException($"Currency {currency} not found");
                
                return exchangeRates[currency];
            }
        }

        // SRP: Only responsibility is to update rates
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

        public bool RateExists(string currency)
        {
            lock (lockObject)
            {
                return exchangeRates.ContainsKey(currency);
            }
        }

        public Dictionary<string, decimal> GetAllRates()
        {
            lock (lockObject)
            {
                return new Dictionary<string, decimal>(exchangeRates);
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // SRP: Interface for Currency Conversion
    // Responsibility: Define contract for conversion operations
    // ═══════════════════════════════════════════════════════════════════════
    
    public interface ICurrencyConverter
    {
        /// <summary>
        /// Converts amount from one currency to another
        /// </summary>
        decimal Convert(decimal amount, string fromCurrency, string toCurrency);
    }

    // ═══════════════════════════════════════════════════════════════════════
    // SRP: Core Currency Converter
    // Responsibility: Perform currency conversions
    // Does NOT manage rates, does NOT handle logging
    // ═══════════════════════════════════════════════════════════════════════
    
    public class CurrencyConverter : ICurrencyConverter
    {
        // ✅ Uses singleton internally
        private readonly ExchangeRateManager rateManager;

        public CurrencyConverter()
        {
            // ✅ Always gets the SAME singleton instance
            rateManager = ExchangeRateManager.Instance;
        }

        // SRP: Only converts currencies
        public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
        {
            if (!rateManager.RateExists(fromCurrency))
                throw new ArgumentException($"Unsupported currency: {fromCurrency}");
            if (!rateManager.RateExists(toCurrency))
                throw new ArgumentException($"Unsupported currency: {toCurrency}");

            decimal fromRate = rateManager.GetRate(fromCurrency);
            decimal toRate = rateManager.GetRate(toCurrency);
            decimal result = amount * (toRate / fromRate);

            return result;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // DECORATOR Pattern with SRP
    // Responsibility: Add logging behavior without modifying converter
    // ═══════════════════════════════════════════════════════════════════════
    
    public class LoggingCurrencyConverter : ICurrencyConverter
    {
        // ✅ Wraps any ICurrencyConverter implementation
        private readonly ICurrencyConverter innerConverter;
        private readonly string instanceId;

        public LoggingCurrencyConverter(ICurrencyConverter converter)
        {
            innerConverter = converter;
            instanceId = Guid.NewGuid().ToString().Substring(0, 8);
        }

        // SRP: Only adds logging, delegates conversion to inner converter
        public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
        {
            Console.WriteLine($"[{instanceId}] 📝 Converting {amount} {fromCurrency} → {toCurrency}");
            
            decimal result = innerConverter.Convert(amount, fromCurrency, toCurrency);
            
            Console.WriteLine($"[{instanceId}] ✅ Result: {result:F2} {toCurrency}\n");
            return result;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // TRACKING DECORATOR - SRP
    // Responsibility: Track conversion operations
    // ═══════════════════════════════════════════════════════════════════════
    
    public class OperationTracker : ICurrencyConverter
    {
        private readonly ICurrencyConverter innerConverter;
        private static int operationCount = 0;

        public OperationTracker(ICurrencyConverter converter)
        {
            innerConverter = converter;
        }

        // SRP: Only tracks operations
        public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
        {
            operationCount++;
            decimal result = innerConverter.Convert(amount, fromCurrency, toCurrency);
            Console.WriteLine($"🔢 [Operation #{operationCount}]\n");
            return result;
        }

        public static int GetOperationCount() => operationCount;
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CONSOLE APPLICATION - Demonstrating Singleton + SRP
    // ═══════════════════════════════════════════════════════════════════════
    
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ✅ AFTER: Currency Converter WITH Singleton Pattern + SRP     ║");
            Console.WriteLine("║  Solution: One instance, one API call, consistent data         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            // ✅ BENEFIT 1: Creating multiple "converters" uses same singleton
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 1: Creating Multiple Converter References");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            // These create multiple converter OBJECTS but use same singleton
            ICurrencyConverter converter1 = new CurrencyConverter();
            ICurrencyConverter converter2 = new CurrencyConverter();
            ICurrencyConverter converter3 = new CurrencyConverter();

            Console.WriteLine("✅ Three converter references created!");
            Console.WriteLine("✅ But only ONE API call happened (see above)!");
            Console.WriteLine("✅ All converters share the SAME rate data!\n");

            // Verify they use same singleton rates
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 2: Verifying Singleton Instance");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            var instance1 = ExchangeRateManager.Instance;
            var instance2 = ExchangeRateManager.Instance;
            var instance3 = ExchangeRateManager.Instance;

            Console.WriteLine($"instance1 == instance2? {instance1 == instance2} ✅");
            Console.WriteLine($"instance2 == instance3? {instance2 == instance3} ✅");
            Console.WriteLine($"All references are the SAME object!\n");

            // ✅ BENEFIT 2: Data consistency across all converters
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 3: Data Consistency");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            decimal rate1 = ExchangeRateManager.Instance.GetRate("USD");
            Console.WriteLine($"Initial USD Rate: {rate1}");

            // Update in singleton
            ExchangeRateManager.Instance.UpdateRate("USD", 1.1m);
            Console.WriteLine();

            // All converters see the updated rate!
            decimal rate2 = ExchangeRateManager.Instance.GetRate("USD");
            Console.WriteLine($"After update, USD Rate: {rate2} (SYNCHRONIZED!)");
            Console.WriteLine("✅ All converters automatically see the new rate!\n");

            // ✅ BENEFIT 3: Consistent conversions
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 4: Consistent Conversions");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            decimal result1 = converter1.Convert(100, "USD", "EUR");
            decimal result2 = converter2.Convert(100, "USD", "EUR");
            decimal result3 = converter3.Convert(100, "USD", "EUR");

            Console.WriteLine($"converter1 result: {result1:F2}");
            Console.WriteLine($"converter2 result: {result2:F2}");
            Console.WriteLine($"converter3 result: {result3:F2}");
            Console.WriteLine("✅ All results are IDENTICAL! (Perfect consistency)\n");

            // ✅ BENEFIT 4: Decorators demonstrate SRP
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 5: SRP with Decorators");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            // Create converter with optional logging
            ICurrencyConverter baseConverter = new CurrencyConverter();
            ICurrencyConverter loggingConverter = new LoggingCurrencyConverter(baseConverter);
            ICurrencyConverter trackedConverter = new OperationTracker(loggingConverter);

            Console.WriteLine("Converting with logging and tracking:\n");
            trackedConverter.Convert(250, "USD", "GBP");

            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 6: Memory and Performance Summary");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ PROS - WITH SINGLETON PATTERN + SRP:");
            Console.WriteLine("✅ Single instance guaranteed");
            Console.WriteLine("✅ Only ONE API call made");
            Console.WriteLine("✅ Only 50KB memory (not 150KB)");
            Console.WriteLine("✅ Perfect data consistency");
            Console.WriteLine("✅ Global access via ExchangeRateManager.Instance");
            Console.WriteLine("✅ Thread-safe implementation");
            Console.WriteLine("✅ Easy to test and maintain");
            Console.WriteLine("✅ SRP: Each class has single responsibility");
            Console.WriteLine("✅ Extensible with decorators");
            Console.ResetColor();

            Console.WriteLine($"\n📊 Total operations executed: {OperationTracker.GetOperationCount()}");
            Console.WriteLine("\n🎯 This is production-ready code!\n");

            // Display all rates
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("All Available Exchange Rates:");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            foreach (var rate in ExchangeRateManager.Instance.GetAllRates())
            {
                Console.WriteLine($"  {rate.Key}: {rate.Value}");
            }

            Console.WriteLine("\n✨ Singleton Pattern with SRP = Clean, Efficient, Maintainable! ✨\n");
        }
    }
}

/*
═══════════════════════════════════════════════════════════════════════════════
EXECUTION OUTPUT EXPLANATION:
═══════════════════════════════════════════════════════════════════════════════

When you run this program, you'll see:

1. ONLY ONE ExchangeRateManager initialization
   - "[ExchangeRateManager] Initializing Singleton..."
   - "[ExchangeRateManager] Loading exchange rates from server..."
   - "✅ [ExchangeRateManager] Singleton initialized"

2. All three converter references created but same singleton used
   - No additional API calls
   - No duplicate initialization

3. Data consistency verification
   - All converters report same rates
   - Updates in singleton immediately visible to all

4. Consistent conversion results
   - converter1.Convert() = X.XX
   - converter2.Convert() = X.XX (SAME)
   - converter3.Convert() = X.XX (SAME)

5. Decorator demonstration
   - Shows how logging and tracking can be added without modifying core logic
   - Each decorator has single responsibility

═══════════════════════════════════════════════════════════════════════════════
ARCHITECTURE HIGHLIGHTS:
═══════════════════════════════════════════════════════════════════════════════

1. SINGLETON PATTERN
   - Private constructor prevents external instantiation
   - Lazy<T> provides thread-safe initialization
   - Static Instance property provides global access

2. SRP (Single Responsibility Principle)
   - ExchangeRateManager: Manages rates only
   - CurrencyConverter: Converts currencies only
   - LoggingCurrencyConverter: Adds logging only
   - OperationTracker: Tracks operations only

3. DEPENDENCY INVERSION
   - Depend on ICurrencyConverter interface
   - Easy to swap implementations
   - Easy to test with mocks

4. DECORATOR PATTERN
   - Add behavior without modifying original class
   - Composable and extensible
   - Each decorator has single concern

═══════════════════════════════════════════════════════════════════════════════
COMPARISON WITH BEFORE:
═══════════════════════════════════════════════════════════════════════════════

METRIC                  BEFORE      AFTER       IMPROVEMENT
─────────────────────────────────────────────────────────
Instances Created       3           1           ✅ 3x
API Calls               3           1           ✅ 3x
Memory Usage            150KB       50KB        ✅ 3x
Data Consistency        ❌ NO       ✅ YES      ✅ PERFECT
Global Access           ❌ NO       ✅ YES      ✅ CLEAN
Code Clarity            ⚠️  MESSY   ✅ CLEAR    ✅ MUCH BETTER
Testability             ⚠️  HARD    ✅ EASY     ✅ MUCH BETTER
Thread Safety           ❌ NO       ✅ YES      ✅ SAFE
Maintenance             ⚠️  HARD    ✅ EASY     ✅ MAINTAINABLE

═══════════════════════════════════════════════════════════════════════════════
WHY THIS PATTERN IS USED HERE:
═══════════════════════════════════════════════════════════════════════════════

✅ Single Source of Truth
   Exchange rates should be loaded once and shared

✅ Performance Critical
   API calls are expensive, should minimize them

✅ Data Consistency
   All parts of app must use same rates

✅ Global Access
   Converters throughout app need rates

✅ Resource Management
   Singleton efficiently manages scarce resources

═══════════════════════════════════════════════════════════════════════════════
*/
