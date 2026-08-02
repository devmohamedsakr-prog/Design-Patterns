using System;
using System.Threading.Tasks;
using PaymentGateway.After.Abstracts;

namespace PaymentGateway.After.Processors
{
    /// <summary>
    /// PayPal Payment Processor
    /// Concrete product created by PayPalPaymentGateway factory method
    /// </summary>
    public class PayPalProcessor : IPaymentProcessor
    {
        public string GetProcessorName() => "PayPal";

        public async Task<PaymentResult> ProcessAsync(decimal amount, string currency, string orderId)
        {
            // Simulate PayPal API call
            await Task.Delay(75);

            return new PaymentResult
            {
                Success = true,
                TransactionId = $"paypal_txn_{orderId}_{DateTime.Now.Ticks}",
                Message = "Payment processed successfully via PayPal",
                Amount = amount,
                ProcessorName = GetProcessorName()
            };
        }
    }
}
