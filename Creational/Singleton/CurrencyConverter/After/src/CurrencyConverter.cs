using System;

// ============================================================================
// CONVERTER: CurrencyConverter
// ============================================================================
// Responsibility: Perform currency conversions
// 
// This class is responsible for:
// - Converting amounts between currencies
// - Accessing rates from the singleton
// - Performing conversion calculations
// - Validating input currencies
//
// It SHOULD NOT handle:
// - Rate management or updates
// - Logging or output
// - Decorator functionality
// - Persistence or caching
// ============================================================================

namespace CurrencyConverterAfter
{
    /// <summary>
    /// Implementation of ICurrencyConverter that performs currency conversions.
    /// Uses ExchangeRateManager singleton for rate data.
    /// Stateless, focused only on conversion logic.
    /// </summary>
    public class CurrencyConverter : ICurrencyConverter
    {
        // Reference to the singleton rate manager
        private readonly ExchangeRateManager rateManager;

        /// <summary>
        /// Initializes a new CurrencyConverter.
        /// Gets reference to the singleton ExchangeRateManager.
        /// </summary>
        public CurrencyConverter()
        {
            // Always gets the SAME singleton instance
            rateManager = ExchangeRateManager.Instance;
        }

        /// <summary>
        /// Converts an amount from one currency to another.
        /// Uses exchange rates from the singleton.
        /// </summary>
        /// <param name="amount">Amount to convert</param>
        /// <param name="fromCurrency">Source currency code</param>
        /// <param name="toCurrency">Target currency code</param>
        /// <returns>Converted amount</returns>
        /// <exception cref="ArgumentException">If currency not supported</exception>
        public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
        {
            // Validate currencies exist
            if (!rateManager.RateExists(fromCurrency))
                throw new ArgumentException($"Unsupported currency: {fromCurrency}");
            if (!rateManager.RateExists(toCurrency))
                throw new ArgumentException($"Unsupported currency: {toCurrency}");

            // Get rates from singleton
            decimal fromRate = rateManager.GetRate(fromCurrency);
            decimal toRate = rateManager.GetRate(toCurrency);

            // Perform conversion
            decimal result = amount * (toRate / fromRate);

            return result;
        }
    }
}
