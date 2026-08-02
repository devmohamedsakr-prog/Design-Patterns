using System;
using System.Threading.Tasks;

namespace PaymentGateway.Before.Src
{
    /// <summary>
    /// BEFORE: Tightly coupled payment processing
    /// ❌ PROBLEM: Hard-coded payment processor instantiation
    /// - Cannot easily add new payment methods
    /// - Violates Open-Closed Principle
    /// - Each payment type requires code modification
    /// - No abstraction - direct dependency on concrete classes
    /// </summary>
    public class PaymentProcessor
    {
        public async Task<PaymentResult> ProcessPaymentAsync(string paymentType, decimal amount, 
            string currency, string orderId)
        {
            try
            {
                // ❌ PROBLEM 1: Hard-coded if-else for each payment type
                if (paymentType == "Stripe")
                {
                    return await ProcessStripePayment(amount, currency, orderId);
                }
                else if (paymentType == "PayPal")
                {
                    return await ProcessPayPalPayment(amount, currency, orderId);
                }
                else if (paymentType == "BankTransfer")
                {
                    return await ProcessBankTransferPayment(amount, currency, orderId);
                }
                else
                {
                    return new PaymentResult 
                    { 
                        Success = false, 
                        Message = "Unknown payment type" 
                    };
                }
            }
            catch (Exception ex)
            {
                return new PaymentResult { Success = false, Message = ex.Message };
            }
        }

        // ❌ PROBLEM 2: All payment logic in single class
        // Violates Single Responsibility Principle
        private async Task<PaymentResult> ProcessStripePayment(decimal amount, string currency, string orderId)
        {
            if (amount <= 0)
                return new PaymentResult { Success = false, Message = "Invalid amount" };
            if (string.IsNullOrEmpty(currency))
                return new PaymentResult { Success = false, Message = "Invalid currency" };

            // Simulate Stripe API call
            await Task.Delay(50);

            return new PaymentResult
            {
                Success = true,
                TransactionId = $"stripe_txn_{orderId}_{DateTime.Now.Ticks}",
                Message = "Payment processed successfully via Stripe",
                Amount = amount,
                ProcessorName = "Stripe"
            };
        }

        private async Task<PaymentResult> ProcessPayPalPayment(decimal amount, string currency, string orderId)
        {
            if (amount <= 0)
                return new PaymentResult { Success = false, Message = "Invalid amount" };
            if (string.IsNullOrEmpty(currency))
                return new PaymentResult { Success = false, Message = "Invalid currency" };

            // Simulate PayPal API call
            await Task.Delay(75);

            return new PaymentResult
            {
                Success = true,
                TransactionId = $"paypal_txn_{orderId}_{DateTime.Now.Ticks}",
                Message = "Payment processed successfully via PayPal",
                Amount = amount,
                ProcessorName = "PayPal"
            };
        }

        private async Task<PaymentResult> ProcessBankTransferPayment(decimal amount, string currency, string orderId)
        {
            if (amount <= 0)
                return new PaymentResult { Success = false, Message = "Invalid amount" };
            if (string.IsNullOrEmpty(currency))
                return new PaymentResult { Success = false, Message = "Invalid currency" };

            // Simulate Bank Transfer API call
            await Task.Delay(200);

            return new PaymentResult
            {
                Success = true,
                TransactionId = $"bank_txn_{orderId}_{DateTime.Now.Ticks}",
                Message = "Bank transfer initiated successfully",
                Amount = amount,
                ProcessorName = "BankTransfer"
            };
        }
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
