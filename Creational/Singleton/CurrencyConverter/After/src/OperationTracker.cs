using System;

// ============================================================================
// DECORATOR: OperationTracker
// ============================================================================
// Responsibility: Track and count conversion operations
// Pattern: Decorator Pattern
// 
// This class is responsible for:
// - Counting conversion operations
// - Delegating conversion to inner converter
// - Providing operation statistics
//
// It SHOULD NOT handle:
// - Actual conversion logic
// - Logging output
// - Rate management
// - Business logic modification
// ============================================================================

namespace CurrencyConverterAfter
{
    /// <summary>
    /// Decorator that tracks/counts conversion operations.
    /// Wraps any ICurrencyConverter implementation.
    /// Counts the number of conversions performed.
    /// Demonstrates Decorator Pattern with SRP.
    /// </summary>
    public class OperationTracker : ICurrencyConverter
    {
        // The converter being wrapped
        private readonly ICurrencyConverter innerConverter;
        
        // Static counter to track total operations
        private static int operationCount = 0;

        /// <summary>
        /// Initializes OperationTracker with an inner converter to wrap.
        /// </summary>
        /// <param name="converter">Inner converter to wrap</param>
        public OperationTracker(ICurrencyConverter converter)
        {
            innerConverter = converter;
        }

        /// <summary>
        /// Converts currency and tracks the operation.
        /// Increments operation counter.
        /// Delegates actual conversion to inner converter.
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="fromCurrency">Source currency</param>
        /// <param name="toCurrency">Target currency</param>
        /// <returns>Converted amount (from inner converter)</returns>
        public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
        {
            // Increment operation count
            operationCount++;
            
            // Delegate to inner converter
            decimal result = innerConverter.Convert(amount, fromCurrency, toCurrency);
            
            // Display operation count
            Console.WriteLine($"🔢 [Operation #{operationCount}]\n");
            
            return result;
        }

        /// <summary>
        /// Gets the total number of operations tracked.
        /// </summary>
        /// <returns>Total operation count</returns>
        public static int GetOperationCount() => operationCount;
    }
}
