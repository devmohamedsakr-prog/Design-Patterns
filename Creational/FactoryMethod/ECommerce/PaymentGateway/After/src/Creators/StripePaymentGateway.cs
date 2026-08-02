using PaymentGateway.After.Abstracts;
using PaymentGateway.After.Processors;

namespace PaymentGateway.After.Creators
{
    /// <summary>
    /// Stripe Payment Gateway Creator
    /// Concrete Creator: Implements factory method to create StripeProcessor
    /// </summary>
    public class StripePaymentGateway : PaymentGatewayCreator
    {
        /// <summary>
        /// Factory Method: Creates Stripe processor
        /// Subclass decides which concrete product to instantiate
        /// </summary>
        protected override IPaymentProcessor CreatePaymentProcessor()
        {
            return new StripeProcessor();
        }
    }
}
