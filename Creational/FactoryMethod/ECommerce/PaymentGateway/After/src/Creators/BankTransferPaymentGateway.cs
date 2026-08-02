using PaymentGateway.After.Abstracts;
using PaymentGateway.After.Processors;

namespace PaymentGateway.After.Creators
{
    /// <summary>
    /// Bank Transfer Payment Gateway Creator
    /// Concrete Creator: Implements factory method to create BankTransferProcessor
    /// </summary>
    public class BankTransferPaymentGateway : PaymentGatewayCreator
    {
        /// <summary>
        /// Factory Method: Creates Bank Transfer processor
        /// Subclass decides which concrete product to instantiate
        /// </summary>
        protected override IPaymentProcessor CreatePaymentProcessor()
        {
            return new BankTransferProcessor();
        }
    }
}
