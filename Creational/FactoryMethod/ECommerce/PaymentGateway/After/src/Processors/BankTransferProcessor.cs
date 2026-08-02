using System;
using System.Threading.Tasks;
using PaymentGateway.After.Abstracts;

namespace PaymentGateway.After.Processors
{
    /// <summary>
    /// Bank Transfer Payment Processor
    /// Concrete product created by BankTransferPaymentGateway factory method
    /// </summary>
    public class BankTransferProcessor : IPaymentProcessor
    {
        public string GetProcessorName() => "BankTransfer";

        public async Task<PaymentResult> ProcessAsync(decimal amount, string currency, string orderId)
        {
            // Simulate Bank Transfer API call (slower)
            await Task.Delay(200);

            return new PaymentResult
            {
                Success = true,
                TransactionId = $"bank_txn_{orderId}_{DateTime.Now.Ticks}",
                Message = "Bank transfer initiated successfully",
                Amount = amount,
                ProcessorName = GetProcessorName()
            };
        }
    }
}
