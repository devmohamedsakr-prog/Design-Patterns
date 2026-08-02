using System;
using System.Threading.Tasks;
using PaymentGateway.After.Abstracts;

namespace PaymentGateway.After.Processors
{
    /// <summary>
    /// Stripe Payment Processor
    /// Concrete product created by StripePaymentGateway factory method
    /// </summary>
    public class StripeProcessor : IPaymentProcessor
    {
        public string GetProcessorName() => "Stripe";

        public async Task<PaymentResult> ProcessAsync(decimal amount, string currency, string orderId)
        {
            // Simulate Stripe API call
            await Task.Delay(50);

            return new PaymentResult
            {
                Success = true,
                TransactionId = $"stripe_txn_{orderId}_{DateTime.Now.Ticks}",
                Message = "Payment processed successfully via Stripe",
                Amount = amount,
                ProcessorName = GetProcessorName()
            };
        }
    }
}
