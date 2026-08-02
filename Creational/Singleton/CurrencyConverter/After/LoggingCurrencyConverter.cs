using System;

// ============================================================================
// DECORATOR: LoggingCurrencyConverter
// ============================================================================
// Responsibility: Add logging behavior to currency conversion
// Pattern: Decorator Pattern
// 
// This class is responsible for:
// - Adding logging/output to conversions
// - Delegating conversion to inner converter
// - Wrapping conversion behavior
//
// It SHOULD NOT handle:
// - Actual conversion logic
// - Rate management
// - Operation tracking
// - Business logic modification
// ============================================================================

namespace CurrencyConverterAfter
{
    /// <summary>
    /// Decorator that adds logging to currency conversions.
    /// Wraps any ICurrencyConverter implementation.
    /// Does not modify conversion logic, only adds logging.
    /// Demonstrates Decorator Pattern with SRP.
    /// </summary>
    public class LoggingCurrencyConverter : ICurrencyConverter
    {
        // The converter being wrapped
        private readonly ICurrencyConverter innerConverter;
        private readonly string instanceId;

        /// <summary>
        /// Initializes LoggingCurrencyConverter with an inner converter to wrap.
        /// </summary>
        /// <param name="converter">Inner converter to wrap</param>
        public LoggingCurrencyConverter(ICurrencyConverter converter)
        {
            innerConverter = converter;
            instanceId = Guid.NewGuid().ToString().Substring(0, 8);
        }

        /// <summary>
        /// Converts currency with logging.
        /// Logs before and after conversion.
        /// Delegates actual conversion to inner converter.
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="fromCurrency">Source currency</param>
        /// <param name="toCurrency">Target currency</param>
        /// <returns>Converted amount (from inner converter)</returns>
        public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
        {
            // Log conversion request
            Console.WriteLine($"[{instanceId}] 📝 Converting {amount} {fromCurrency} → {toCurrency}");
            
            // Delegate to inner converter
            decimal result = innerConverter.Convert(amount, fromCurrency, toCurrency);
            
            // Log result
            Console.WriteLine($"[{instanceId}] ✅ Result: {result:F2} {toCurrency}\n");
            
            return result;
        }
    }
}
