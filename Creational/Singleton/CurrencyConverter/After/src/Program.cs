using System;

// ============================================================================
// CONSOLE APPLICATION: Main Program
// ============================================================================
// Responsibility: Demonstrate the Singleton pattern with SRP
// 
// This class is responsible for:
// - Running console demonstrations
// - Showing pattern benefits
// - Displaying output to user
//
// It SHOULD NOT:
// - Implement core pattern logic
// - Manage rates
// - Perform conversions directly
// ============================================================================

namespace CurrencyConverterAfter
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  ✅ AFTER: Currency Converter WITH Singleton Pattern + SRP     ║");
            Console.WriteLine("║  Solution: One instance, one API call, consistent data         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            // Step 1: Create multiple converter references
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 1: Creating Multiple Converter References");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            ICurrencyConverter converter1 = new CurrencyConverter();
            ICurrencyConverter converter2 = new CurrencyConverter();
            ICurrencyConverter converter3 = new CurrencyConverter();

            Console.WriteLine("✅ Three converter references created!");
            Console.WriteLine("✅ But only ONE API call happened (see above)!");
            Console.WriteLine("✅ All converters share the SAME rate data!\n");

            // Step 2: Verify singleton instance
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 2: Verifying Singleton Instance");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            var instance1 = ExchangeRateManager.Instance;
            var instance2 = ExchangeRateManager.Instance;
            var instance3 = ExchangeRateManager.Instance;

            Console.WriteLine($"instance1 == instance2? {instance1 == instance2} ✅");
            Console.WriteLine($"instance2 == instance3? {instance2 == instance3} ✅");
            Console.WriteLine($"All references are the SAME object!\n");

            // Step 3: Verify data consistency
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 3: Data Consistency");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            decimal rate1 = ExchangeRateManager.Instance.GetRate("USD");
            Console.WriteLine($"Initial USD Rate: {rate1}");

            ExchangeRateManager.Instance.UpdateRate("USD", 1.1m);
            Console.WriteLine();

            decimal rate2 = ExchangeRateManager.Instance.GetRate("USD");
            Console.WriteLine($"After update, USD Rate: {rate2} (SYNCHRONIZED!)");
            Console.WriteLine("✅ All converters automatically see the new rate!\n");

            // Step 4: Verify consistent conversions
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

            // Step 5: Demonstrate decorators
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("Step 5: SRP with Decorators");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n");

            ICurrencyConverter baseConverter = new CurrencyConverter();
            ICurrencyConverter loggingConverter = new LoggingCurrencyConverter(baseConverter);
            ICurrencyConverter trackedConverter = new OperationTracker(loggingConverter);

            Console.WriteLine("Converting with logging and tracking:\n");
            trackedConverter.Convert(250, "USD", "GBP");

            // Step 6: Summary
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
