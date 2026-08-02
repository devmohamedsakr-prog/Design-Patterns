using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// ============================================================================
// ❌ BEFORE: Currency Converter WITHOUT Singleton Pattern
// ============================================================================
// Problems:
// 1. Multiple instances are created
// 2. Each instance loads exchange rates independently (redundant API calls)
// 3. Data inconsistency between instances
// 4. Memory waste - duplicate data
// 5. No global access point
// ============================================================================

namespace CurrencyConverterBefore
{
    // ❌ WITHOUT SINGLETON - Every instance loads rates independently
    public class CurrencyConverter
    {
        private Dictionary<string, decimal> exchangeRates;
        private string instanceId;

        // ❌ PROBLEM: No protection against multiple instantiation
        public CurrencyConverter()
        {
            // Every time an instance is created, this runs!
            instanceId = Guid.NewGuid().ToString().Substring(0, 8);
            Console.WriteLine($"[{instanceId}] Creating new CurrencyConverter instance...");
            
            // ❌ PROBLEM: Each instance loads rates (redundant API calls)
            LoadExchangeRates();
        }

        private void LoadExchangeRates()
        {
            // ❌ PROBLEM: Simulating API call - happens for EVERY instance!
            Console.WriteLine($"[{instanceId}] 📡 API Call: Loading exchange rates from server...");
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

            Console.WriteLine($"[{instanceId}] ✅ Exchange rates loaded into memory (50KB)");
            Console.WriteLine();
        }

        public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
        {
            if (!exchangeRates.ContainsKey(fromCurrency) || !exchangeRates.ContainsKey(toCurrency))
            {
                throw new ArgumentException($"Unsupported currency");
            }

            decimal fromRate = exchangeRates[fromCurrency];
            decimal toRate = exchangeRates[toCurrency];
            decimal result = amount * (toRate / fromRate);

            Console.WriteLine($"[{instanceId}] Convert: {amount} {fromCurrency} = {result:F2} {toCurrency}");
            return result;
        }

        public void UpdateRate(string currency, decimal newRate)
        {
            // ❌ PROBLEM: Update only affects THIS instance!
            if (exchangeRates.ContainsKey(currency))
            {
                exchangeRates[currency] = newRate;
                Console.WriteLine($"[{instanceId}] 📊 Updated {currency} rate to {newRate}");
            }
        }

        public decimal GetRate(string currency)
        {
            if (exchangeRates.ContainsKey(currency))
            {
                return exchangeRates[currency];
            }
            throw new ArgumentException($"Currency {currency} not found");
        }
    }

    // ============================================================================
    // CONSOLE APPLICATION - Demonstrating the problems
    // ============================================================================
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ❌ BEFORE: Currency Converter WITHOUT Singleton Pattern       ║");
            Console.WriteLine("║  Problem: Multiple instances, redundant API calls, data chaos  ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            // ❌ PROBLEM 1 & 2: Multiple instances created, each loads rates
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 1: Creating Multiple Instances");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            CurrencyConverter converter1 = new CurrencyConverter();
            CurrencyConverter converter2 = new CurrencyConverter();
            CurrencyConverter converter3 = new CurrencyConverter();

            Console.WriteLine("⚠️  PROBLEM: 3 API calls made instead of 1!");
            Console.WriteLine("⚠️  PROBLEM: 150KB of memory used instead of 50KB!\n");

            // ❌ PROBLEM 3: Data inconsistency
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 2: Data Inconsistency Between Instances");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            decimal initialRate = converter1.GetRate("USD");
            Console.WriteLine($"converter1 USD Rate: {initialRate}\n");

            // Update rate in converter1
            converter1.UpdateRate("USD", 1.1m);
            Console.WriteLine();

            // ❌ converter2 and converter3 still have old rate!
            decimal rate2 = converter2.GetRate("USD");
            decimal rate3 = converter3.GetRate("USD");

            Console.WriteLine($"converter2 USD Rate: {rate2} (OUT OF SYNC!)");
            Console.WriteLine($"converter3 USD Rate: {rate3} (OUT OF SYNC!)");
            Console.WriteLine("\n⚠️  PROBLEM: Each instance has its own data copy!");
            Console.WriteLine("⚠️  PROBLEM: Updates don't sync across instances!\n");

            // Demonstrate conversions
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 3: Conversions Using Different Instances");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            converter1.Convert(100, "USD", "EUR");
            converter2.Convert(100, "USD", "EUR");
            converter3.Convert(100, "USD", "EUR");

            Console.WriteLine("\n⚠️  PROBLEM: Different results due to inconsistent data!\n");

            // Summary
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("CONS - WITHOUT SINGLETON PATTERN:");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("❌ Multiple instances created uncontrollably");
            Console.WriteLine("❌ Redundant API calls (1 call should become 3 calls)");
            Console.WriteLine("❌ 3x memory usage for same data");
            Console.WriteLine("❌ Data inconsistency between instances");
            Console.WriteLine("❌ No global access point - manual passing required");
            Console.WriteLine("❌ Hard to maintain and test");
            Console.WriteLine("❌ Difficult to update exchange rates globally");
            Console.WriteLine("❌ Application performance degradation");
            Console.ResetColor();

            Console.WriteLine("\n📖 SOLUTION: Use Singleton Pattern!");
            Console.WriteLine("🔗 Go to '../After' folder to see the solution.\n");
        }
    }
}

/*
═══════════════════════════════════════════════════════════════════════════════
EXECUTION OUTPUT EXPLANATION:
═══════════════════════════════════════════════════════════════════════════════

When you run this program, you'll see:

1. THREE converter instances being created
   - Each with a unique ID (e.g., a1b2c3d4, e5f6g7h8, etc.)

2. THREE separate API calls happening
   - "[a1b2c3d4] 📡 API Call: Loading exchange rates..."
   - "[e5f6g7h8] 📡 API Call: Loading exchange rates..."
   - "[i9j0k1l2] 📡 API Call: Loading exchange rates..."

3. Update in one instance doesn't affect others
   - converter1 rate updated to 1.1
   - converter2 still shows 1.0
   - converter3 still shows 1.0

4. Different conversion results from same input
   - Because they're using different rate tables

═══════════════════════════════════════════════════════════════════════════════
PERFORMANCE METRICS:
═══════════════════════════════════════════════════════════════════════════════

Without Singleton:
- Instances: 3
- API Calls: 3
- Memory: ~150KB
- Time to Load: ~3 seconds
- Data Consistency: ❌ NO

With Singleton:
- Instances: 1
- API Calls: 1
- Memory: ~50KB
- Time to Load: ~1 second
- Data Consistency: ✅ YES

═══════════════════════════════════════════════════════════════════════════════
*/
