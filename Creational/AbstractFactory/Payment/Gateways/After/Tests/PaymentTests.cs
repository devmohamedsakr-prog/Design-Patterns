using NUnit.Framework;
using PaymentGatewayFactory.After.Context;
using System.Collections.Generic;

namespace PaymentGatewayFactory.After.Tests
{
    [TestFixture]
    public class PaymentTests
    {
        [Test]
        public void StripeFactory_CreatePaymentProcessor()
        {
            var factory = new StripeFactory();
            var processor = factory.CreatePaymentProcessor();
            Assert.IsNotNull(processor);
        }

        [Test]
        public void PayPalFactory_CreateRefundHandler()
        {
            var factory = new PayPalFactory();
            var refund = factory.CreateRefundHandler();
            Assert.IsNotNull(refund);
        }

        [Test]
        public void SquareFactory_CreateWebhookHandler()
        {
            var factory = new SquareFactory();
            var webhook = factory.CreateWebhookHandler();
            Assert.IsNotNull(webhook);
        }

        [Test]
        public void ProviderReturnsCorrectFactory_Stripe()
        {
            var factory = PaymentGatewayProvider.GetFactory("stripe");
            Assert.That(factory, Is.InstanceOf<StripeFactory>());
        }

        [Test]
        public void ProviderReturnsCorrectFactory_PayPal()
        {
            var factory = PaymentGatewayProvider.GetFactory("paypal");
            Assert.That(factory, Is.InstanceOf<PayPalFactory>());
        }

        [Test]
        public void PaymentProcessor_ProcessPayment()
        {
            var factory = new StripeFactory();
            var processor = factory.CreatePaymentProcessor();
            var result = processor.ProcessPayment(99.99m, "tok_visa");
            Assert.That(result, Is.True);
        }

        [Test]
        public void RefundHandler_RefundPayment()
        {
            var factory = new PayPalFactory();
            var refund = factory.CreateRefundHandler();
            var result = refund.RefundPayment("ch_123456", 50.00m);
            Assert.That(result, Is.True);
        }

        [Test]
        public void WebhookHandler_HandleWebhook()
        {
            var factory = new SquareFactory();
            var webhook = factory.CreateWebhookHandler();
            webhook.HandleWebhook("payment.success", new Dictionary<string, string> { { "amount", "100" } });
            Assert.Pass();
        }

        [Test]
        public void PaymentApplication_RunsSuccessfully()
        {
            var factory = new StripeFactory();
            var app = new PaymentApplication(factory);
            app.ProcessTransaction();
            Assert.Pass();
        }
    }
}
