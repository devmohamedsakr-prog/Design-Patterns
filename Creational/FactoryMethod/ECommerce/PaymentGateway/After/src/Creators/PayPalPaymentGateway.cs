using PaymentGateway.After.Abstracts;
using PaymentGateway.After.Processors;

namespace PaymentGateway.After.Creators
{
    /// <summary>
    /// PayPal Payment Gateway Creator
    /// Concrete Creator: Implements factory method to create PayPalProcessor
    /// </summary>
    public class PayPalPaymentGateway : PaymentGatewayCreator
    {
        /// <summary>
        /// Factory Method: Creates PayPal processor
        /// Subclass decides which concrete product to instantiate
        /// </summary>
        protected override IPaymentProcessor CreatePaymentProcessor()
        {
            return new PayPalProcessor();
        }
    }
}
