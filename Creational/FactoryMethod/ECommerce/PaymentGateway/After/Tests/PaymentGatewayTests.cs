using NUnit.Framework;
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using PaymentGateway.After.Abstracts;
using PaymentGateway.After.Creators;
using PaymentGateway.After.Processors;

namespace PaymentGateway.After.Tests
{
    [TestFixture]
    public class PaymentGatewayTests
    {
        // ============================================================
        // STRIPE PAYMENT GATEWAY TESTS
        // ============================================================

        [Test]
        public async Task StripePaymentGateway_ProcessPayment_ShouldReturnSuccessResult()
        {
            // Arrange
            var gateway = new StripePaymentGateway();
            decimal amount = 100m;
            string currency = "USD";
            string orderId = "ORD-001";

            // Act
            var result = await gateway.ProcessPaymentAsync(amount, currency, orderId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ProcessorName, Is.EqualTo("Stripe"));
            Assert.That(result.Amount, Is.EqualTo(amount));
        }

        [Test]
        public async Task StripePaymentGateway_CreatePaymentProcessor_ShouldCreateStripeProcessor()
        {
            // Arrange
            var gateway = new StripePaymentGateway();

            // Act
            var result = await gateway.ProcessPaymentAsync(50m, "USD", "ORD-002");

            // Assert
            Assert.That(result.ProcessorName, Is.EqualTo("Stripe"));
        }

        [Test]
        public async Task StripePaymentGateway_ProcessPayment_ShouldGenerateTransactionId()
        {
            // Arrange
            var gateway = new StripePaymentGateway();

            // Act
            var result = await gateway.ProcessPaymentAsync(200m, "USD", "ORD-003");

            // Assert
            Assert.That(result.TransactionId, Is.Not.Null);
            Assert.That(result.TransactionId, Does.StartWith("stripe_txn_"));
        }

        [Test]
        public async Task StripePaymentGateway_ProcessPayment_WithMultipleOrders_ShouldSucceed()
        {
            // Arrange
            var gateway = new StripePaymentGateway();

            // Act & Assert
            for (int i = 1; i <= 5; i++)
            {
                var result = await gateway.ProcessPaymentAsync(100m * i, "USD", $"ORD-{i:00}");
                Assert.That(result.Success, Is.True);
                Assert.That(result.Amount, Is.EqualTo(100m * i));
            }
        }

        // ============================================================
        // PAYPAL PAYMENT GATEWAY TESTS
        // ============================================================

        [Test]
        public async Task PayPalPaymentGateway_ProcessPayment_ShouldReturnSuccessResult()
        {
            // Arrange
            var gateway = new PayPalPaymentGateway();
            decimal amount = 150m;
            string currency = "USD";
            string orderId = "ORD-010";

            // Act
            var result = await gateway.ProcessPaymentAsync(amount, currency, orderId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ProcessorName, Is.EqualTo("PayPal"));
        }

        [Test]
        public async Task PayPalPaymentGateway_CreatePaymentProcessor_ShouldCreatePayPalProcessor()
        {
            // Arrange
            var gateway = new PayPalPaymentGateway();

            // Act
            var result = await gateway.ProcessPaymentAsync(75m, "USD", "ORD-011");

            // Assert
            Assert.That(result.ProcessorName, Is.EqualTo("PayPal"));
        }

        [Test]
        public async Task PayPalPaymentGateway_ProcessPayment_ShouldGenerateTransactionId()
        {
            // Arrange
            var gateway = new PayPalPaymentGateway();

            // Act
            var result = await gateway.ProcessPaymentAsync(300m, "USD", "ORD-012");

            // Assert
            Assert.That(result.TransactionId, Does.StartWith("paypal_txn_"));
        }

        [Test]
        public async Task PayPalPaymentGateway_ProcessPayment_WithDifferentCurrencies_ShouldSucceed()
        {
            // Arrange
            var gateway = new PayPalPaymentGateway();
            var currencies = new[] { "USD", "EUR", "GBP", "JPY" };

            // Act & Assert
            foreach (var currency in currencies)
            {
                var result = await gateway.ProcessPaymentAsync(100m, currency, $"ORD-{currency}");
                Assert.That(result.Success, Is.True);
            }
        }

        // ============================================================
        // BANK TRANSFER PAYMENT GATEWAY TESTS
        // ============================================================

        [Test]
        public async Task BankTransferPaymentGateway_ProcessPayment_ShouldReturnSuccessResult()
        {
            // Arrange
            var gateway = new BankTransferPaymentGateway();
            decimal amount = 500m;
            string currency = "USD";
            string orderId = "ORD-020";

            // Act
            var result = await gateway.ProcessPaymentAsync(amount, currency, orderId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ProcessorName, Is.EqualTo("BankTransfer"));
        }

        [Test]
        public async Task BankTransferPaymentGateway_CreatePaymentProcessor_ShouldCreateBankTransferProcessor()
        {
            // Arrange
            var gateway = new BankTransferPaymentGateway();

            // Act
            var result = await gateway.ProcessPaymentAsync(250m, "USD", "ORD-021");

            // Assert
            Assert.That(result.ProcessorName, Is.EqualTo("BankTransfer"));
        }

        [Test]
        public async Task BankTransferPaymentGateway_ProcessPayment_ShouldGenerateTransactionId()
        {
            // Arrange
            var gateway = new BankTransferPaymentGateway();

            // Act
            var result = await gateway.ProcessPaymentAsync(1000m, "USD", "ORD-022");

            // Assert
            Assert.That(result.TransactionId, Does.StartWith("bank_txn_"));
        }

        [Test]
        public async Task BankTransferPaymentGateway_ProcessPayment_LargeAmount_ShouldSucceed()
        {
            // Arrange
            var gateway = new BankTransferPaymentGateway();

            // Act
            var result = await gateway.ProcessPaymentAsync(50000m, "USD", "ORD-023");

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Amount, Is.EqualTo(50000m));
        }

        // ============================================================
        // FACTORY METHOD PATTERN TESTS
        // ============================================================

        [Test]
        public async Task FactoryMethod_DifferentCreators_ShouldCreateDifferentProcessors()
        {
            // Arrange
            var stripeGateway = new StripePaymentGateway();
            var paypalGateway = new PayPalPaymentGateway();
            var bankGateway = new BankTransferPaymentGateway();

            // Act
            var stripeResult = await stripeGateway.ProcessPaymentAsync(100m, "USD", "ORD-031");
            var paypalResult = await paypalGateway.ProcessPaymentAsync(100m, "USD", "ORD-032");
            var bankResult = await bankGateway.ProcessPaymentAsync(100m, "USD", "ORD-033");

            // Assert
            Assert.That(stripeResult.ProcessorName, Is.Not.EqualTo(paypalResult.ProcessorName));
            Assert.That(paypalResult.ProcessorName, Is.Not.EqualTo(bankResult.ProcessorName));
            Assert.That(stripeResult.ProcessorName, Is.Not.EqualTo(bankResult.ProcessorName));
        }

        [Test]
        public async Task FactoryMethod_SameCreator_ShouldCreateSameProcessorType()
        {
            // Arrange
            var gateway1 = new StripePaymentGateway();
            var gateway2 = new StripePaymentGateway();

            // Act
            var result1 = await gateway1.ProcessPaymentAsync(100m, "USD", "ORD-041");
            var result2 = await gateway2.ProcessPaymentAsync(100m, "USD", "ORD-042");

            // Assert
            Assert.That(result1.ProcessorName, Is.EqualTo(result2.ProcessorName));
            Assert.That(result1.ProcessorName, Is.EqualTo("Stripe"));
        }

        [Test]
        public async Task FactoryMethod_AllGateways_ShouldReturnTransactionIds()
        {
            // Arrange
            var gateways = new PaymentGatewayCreator[]
            {
                new StripePaymentGateway(),
                new PayPalPaymentGateway(),
                new BankTransferPaymentGateway()
            };

            // Act & Assert
            foreach (var gateway in gateways)
            {
                var result = await gateway.ProcessPaymentAsync(100m, "USD", "ORD-050");
                Assert.That(result.TransactionId, Is.Not.Null);
                Assert.That(result.TransactionId, Is.Not.Empty);
            }
        }

        // ============================================================
        // VALIDATION TESTS
        // ============================================================

        [Test]
        public async Task PaymentGateway_ZeroAmount_ShouldFail()
        {
            // Arrange
            var gateway = new StripePaymentGateway();

            // Act
            var result = await gateway.ProcessPaymentAsync(0m, "USD", "ORD-060");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task PaymentGateway_NullCurrency_ShouldFail()
        {
            // Arrange
            var gateway = new PayPalPaymentGateway();

            // Act
            var result = await gateway.ProcessPaymentAsync(100m, null, "ORD-061");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task PaymentGateway_NullOrderId_ShouldFail()
        {
            // Arrange
            var gateway = new BankTransferPaymentGateway();

            // Act
            var result = await gateway.ProcessPaymentAsync(100m, "USD", null);

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task PaymentGateway_NegativeAmount_ShouldFail()
        {
            // Arrange
            var gateway = new StripePaymentGateway();

            // Act
            var result = await gateway.ProcessPaymentAsync(-100m, "USD", "ORD-062");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        // ============================================================
        // MULTIPLE PAYMENT SCENARIOS
        // ============================================================

        [Test]
        public async Task PaymentGateway_MultipleGatewaysSequential_ShouldAllSucceed()
        {
            // Arrange
            var stripe = new StripePaymentGateway();
            var paypal = new PayPalPaymentGateway();

            // Act
            var result1 = await stripe.ProcessPaymentAsync(100m, "USD", "ORD-070");
            var result2 = await paypal.ProcessPaymentAsync(100m, "USD", "ORD-071");
            var result3 = await stripe.ProcessPaymentAsync(150m, "USD", "ORD-072");

            // Assert
            Assert.That(result1.Success, Is.True);
            Assert.That(result2.Success, Is.True);
            Assert.That(result3.Success, Is.True);
        }

        [Test]
        public async Task PaymentGateway_ProcessorName_ShouldMatchGatewayType()
        {
            // Arrange
            var gateways = new (PaymentGatewayCreator gateway, string expectedName)[]
            {
                (new StripePaymentGateway(), "Stripe"),
                (new PayPalPaymentGateway(), "PayPal"),
                (new BankTransferPaymentGateway(), "BankTransfer")
            };

            // Act & Assert
            foreach (var (gateway, expectedName) in gateways)
            {
                var result = await gateway.ProcessPaymentAsync(100m, "USD", "ORD-080");
                Assert.That(result.ProcessorName, Is.EqualTo(expectedName));
            }
        }

        [Test]
        public async Task PaymentGateway_HighAmountPayments_ShouldSucceed()
        {
            // Arrange
            var gateway = new StripePaymentGateway();
            decimal[] amounts = { 1000m, 10000m, 100000m, 1000000m };

            // Act & Assert
            foreach (var amount in amounts)
            {
                var result = await gateway.ProcessPaymentAsync(amount, "USD", $"ORD-{amount:00}");
                Assert.That(result.Success, Is.True);
                Assert.That(result.Amount, Is.EqualTo(amount));
            }
        }

        [Test]
        public async Task PaymentGateway_TransactionIdUniqueness_ShouldGenerateUniqueTxnIds()
        {
            // Arrange
            var gateway = new StripePaymentGateway();
            var txnIds = new System.Collections.Generic.HashSet<string>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                var result = await gateway.ProcessPaymentAsync(100m, "USD", $"ORD-{i:000}");
                txnIds.Add(result.TransactionId);
            }

            // Assert
            Assert.That(txnIds.Count, Is.EqualTo(10), "All transaction IDs should be unique");
        }

        // ============================================================
        // CONCRETE PROCESSOR TESTS
        // ============================================================

        [Test]
        public async Task StripeProcessor_ShouldImplementIPaymentProcessor()
        {
            // Arrange
            var processor = new StripeProcessor();

            // Act & Assert
            Assert.That(processor, Is.InstanceOf<IPaymentProcessor>());
            Assert.That(processor.GetProcessorName(), Is.EqualTo("Stripe"));
        }

        [Test]
        public async Task PayPalProcessor_ShouldImplementIPaymentProcessor()
        {
            // Arrange
            var processor = new PayPalProcessor();

            // Act & Assert
            Assert.That(processor, Is.InstanceOf<IPaymentProcessor>());
            Assert.That(processor.GetProcessorName(), Is.EqualTo("PayPal"));
        }

        [Test]
        public async Task BankTransferProcessor_ShouldImplementIPaymentProcessor()
        {
            // Arrange
            var processor = new BankTransferProcessor();

            // Act & Assert
            Assert.That(processor, Is.InstanceOf<IPaymentProcessor>());
            Assert.That(processor.GetProcessorName(), Is.EqualTo("BankTransfer"));
        }

        [Test]
        public async Task Processor_DirectCall_ShouldReturnValidResult()
        {
            // Arrange
            IPaymentProcessor processor = new StripeProcessor();

            // Act
            var result = await processor.ProcessAsync(100m, "USD", "ORD-090");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Amount, Is.EqualTo(100m));
        }

        [Test]
        public async Task AllProcessors_DirectCall_ShouldSucceed()
        {
            // Arrange
            IPaymentProcessor[] processors = new IPaymentProcessor[]
            {
                new StripeProcessor(),
                new PayPalProcessor(),
                new BankTransferProcessor()
            };

            // Act & Assert
            foreach (var processor in processors)
            {
                var result = await processor.ProcessAsync(100m, "USD", "ORD-100");
                Assert.That(result.Success, Is.True);
                Assert.That(result.ProcessorName, Is.Not.Null);
            }
        }
    }
}
