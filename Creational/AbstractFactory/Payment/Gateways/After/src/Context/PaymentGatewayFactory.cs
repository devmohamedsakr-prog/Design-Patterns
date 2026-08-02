using System;
using System.Collections.Generic;

namespace PaymentGatewayFactory.After.Context
{
    // Abstract products
    public interface IPaymentProcessor
    {
        bool ProcessPayment(decimal amount, string cardToken);
    }

    public interface IRefundHandler
    {
        bool RefundPayment(string transactionId, decimal amount);
    }

    public interface IWebhookHandler
    {
        void HandleWebhook(string eventType, Dictionary<string, string> data);
    }

    // Abstract factory
    public interface IPaymentGatewayFactory
    {
        IPaymentProcessor CreatePaymentProcessor();
        IRefundHandler CreateRefundHandler();
        IWebhookHandler CreateWebhookHandler();
    }

    // Stripe implementations
    public class StripePaymentProcessor : IPaymentProcessor
    {
        public bool ProcessPayment(decimal amount, string cardToken)
        {
            Console.WriteLine($"💳 Stripe: Processing ${amount} with token {cardToken}");
            Console.WriteLine("📤 Stripe: Sending to API endpoint: api.stripe.com/v1/charges");
            return true;
        }
    }

    public class StripeRefundHandler : IRefundHandler
    {
        public bool RefundPayment(string transactionId, decimal amount)
        {
            Console.WriteLine($"↩️ Stripe: Refunding ${amount} for transaction {transactionId}");
            return true;
        }
    }

    public class StripeWebhookHandler : IWebhookHandler
    {
        public void HandleWebhook(string eventType, Dictionary<string, string> data)
        {
            Console.WriteLine($"🔔 Stripe: Webhook received - {eventType}");
            foreach (var item in data)
                Console.WriteLine($"   {item.Key}: {item.Value}");
        }
    }

    // PayPal implementations
    public class PayPalPaymentProcessor : IPaymentProcessor
    {
        public bool ProcessPayment(decimal amount, string cardToken)
        {
            Console.WriteLine($"💳 PayPal: Processing ${amount} with email {cardToken}");
            Console.WriteLine("📤 PayPal: Sending to API endpoint: api.paypal.com/v1/payments");
            return true;
        }
    }

    public class PayPalRefundHandler : IRefundHandler
    {
        public bool RefundPayment(string transactionId, decimal amount)
        {
            Console.WriteLine($"↩️ PayPal: Refunding ${amount} for transaction {transactionId}");
            return true;
        }
    }

    public class PayPalWebhookHandler : IWebhookHandler
    {
        public void HandleWebhook(string eventType, Dictionary<string, string> data)
        {
            Console.WriteLine($"🔔 PayPal: IPN notification received - {eventType}");
            foreach (var item in data)
                Console.WriteLine($"   {item.Key}: {item.Value}");
        }
    }

    // Square implementations
    public class SquarePaymentProcessor : IPaymentProcessor
    {
        public bool ProcessPayment(decimal amount, string cardToken)
        {
            Console.WriteLine($"💳 Square: Processing ${amount} with nonce {cardToken}");
            Console.WriteLine("📤 Square: Sending to API endpoint: api.squareupsandbox.com/v2/payments");
            return true;
        }
    }

    public class SquareRefundHandler : IRefundHandler
    {
        public bool RefundPayment(string transactionId, decimal amount)
        {
            Console.WriteLine($"↩️ Square: Refunding ${amount} for transaction {transactionId}");
            return true;
        }
    }

    public class SquareWebhookHandler : IWebhookHandler
    {
        public void HandleWebhook(string eventType, Dictionary<string, string> data)
        {
            Console.WriteLine($"🔔 Square: Webhook event received - {eventType}");
            foreach (var item in data)
                Console.WriteLine($"   {item.Key}: {item.Value}");
        }
    }

    // Concrete factories
    public class StripeFactory : IPaymentGatewayFactory
    {
        public IPaymentProcessor CreatePaymentProcessor() => new StripePaymentProcessor();
        public IRefundHandler CreateRefundHandler() => new StripeRefundHandler();
        public IWebhookHandler CreateWebhookHandler() => new StripeWebhookHandler();
    }

    public class PayPalFactory : IPaymentGatewayFactory
    {
        public IPaymentProcessor CreatePaymentProcessor() => new PayPalPaymentProcessor();
        public IRefundHandler CreateRefundHandler() => new PayPalRefundHandler();
        public IWebhookHandler CreateWebhookHandler() => new PayPalWebhookHandler();
    }

    public class SquareFactory : IPaymentGatewayFactory
    {
        public IPaymentProcessor CreatePaymentProcessor() => new SquarePaymentProcessor();
        public IRefundHandler CreateRefundHandler() => new SquareRefundHandler();
        public IWebhookHandler CreateWebhookHandler() => new SquareWebhookHandler();
    }

    // Factory provider
    public class PaymentGatewayProvider
    {
        public static IPaymentGatewayFactory GetFactory(string provider)
        {
            return provider.ToLower() switch
            {
                "stripe" => new StripeFactory(),
                "paypal" => new PayPalFactory(),
                "square" => new SquareFactory(),
                _ => throw new ArgumentException($"Unknown payment provider: {provider}")
            };
        }
    }

    // Payment processor application
    public class PaymentApplication
    {
        private IPaymentProcessor _processor;
        private IRefundHandler _refund;
        private IWebhookHandler _webhook;

        public PaymentApplication(IPaymentGatewayFactory factory)
        {
            _processor = factory.CreatePaymentProcessor();
            _refund = factory.CreateRefundHandler();
            _webhook = factory.CreateWebhookHandler();
        }

        public void ProcessTransaction()
        {
            Console.WriteLine($"\n💰 Payment Application");
            _processor.ProcessPayment(99.99m, "tok_visa");
            _refund.RefundPayment("ch_123456", 50.00m);
            _webhook.HandleWebhook("charge.completed", new() { { "amount", "9999" } });
        }
    }
}
