using System;
using System.Threading.Tasks;

namespace PaymentGateway.After.Abstracts
{
    /// <summary>
    /// Payment Gateway Abstract Base Class
    /// Defines creation interface - subclasses override CreatePaymentProcessor()
    /// Key: Factory Method delegates instantiation to subclasses
    /// </summary>
    public abstract class PaymentGatewayCreator
    {
        /// <summary>
        /// Factory Method: Abstract - subclasses must implement
        /// Each subclass decides which payment processor to create
        /// </summary>
        protected abstract IPaymentProcessor CreatePaymentProcessor();

        /// <summary>
        /// Template method: Uses factory method to process payment
        /// </summary>
        public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, string currency, string orderId)
        {
            try
            {
                // Use factory method to get appropriate processor
                IPaymentProcessor processor = CreatePaymentProcessor();

                // Validate payment
                if (!ValidatePayment(amount, currency, orderId))
                    return new PaymentResult { Success = false, Message = "Validation failed" };

                // Process payment
                PaymentResult result = await processor.ProcessAsync(amount, currency, orderId);

                if (result.Success)
                {
                    LogTransaction(orderId, amount, processor.GetProcessorName(), "SUCCESS");
                }
                else
                {
                    LogTransaction(orderId, amount, processor.GetProcessorName(), "FAILED");
                }

                return result;
            }
            catch (Exception ex)
            {
                LogTransaction(orderId, amount, "UNKNOWN", $"ERROR: {ex.Message}");
                return new PaymentResult { Success = false, Message = ex.Message };
            }
        }

        protected virtual bool ValidatePayment(decimal amount, string currency, string orderId)
        {
            return amount > 0 && !string.IsNullOrEmpty(currency) && !string.IsNullOrEmpty(orderId);
        }

        protected virtual void LogTransaction(string orderId, decimal amount, string processor, string status)
        {
            Console.WriteLine($"[LOG] Order: {orderId}, Amount: ${amount:F2}, Processor: {processor}, Status: {status}");
        }
    }

    /// <summary>
    /// Payment Processor Interface: Product interface
    /// </summary>
    public interface IPaymentProcessor
    {
        Task<PaymentResult> ProcessAsync(decimal amount, string currency, string orderId);
        string GetProcessorName();
    }

    /// <summary>
    /// Payment Result Model
    /// </summary>
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public decimal Amount { get; set; }
        public string ProcessorName { get; set; }
    }
}
