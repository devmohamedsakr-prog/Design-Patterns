using System;

// ============================================================================
// INTERFACE: ICurrencyConverter
// ============================================================================
// Responsibility: Define the contract for currency conversion operations
// 
// This interface is responsible for:
// - Defining the conversion method signature
// - Specifying behavior that all converters must implement
// - Enabling loose coupling through abstraction
// - Allowing decorator pattern implementation
//
// It SHOULD NOT:
// - Implement conversion logic
// - Manage state
// - Handle singleton concerns
// ============================================================================

namespace CurrencyConverterAfter
{
    /// <summary>
    /// Interface for currency conversion operations.
    /// Implemented by CurrencyConverter and its decorators.
    /// Enables loose coupling and decorator pattern.
    /// </summary>
    public interface ICurrencyConverter
    {
        /// <summary>
        /// Converts an amount from one currency to another.
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="fromCurrency">Source currency code (e.g., "USD")</param>
        /// <param name="toCurrency">Target currency code (e.g., "EUR")</param>
        /// <returns>Converted amount in target currency</returns>
        /// <exception cref="ArgumentException">Thrown if currency not supported</exception>
        decimal Convert(decimal amount, string fromCurrency, string toCurrency);
    }
}
