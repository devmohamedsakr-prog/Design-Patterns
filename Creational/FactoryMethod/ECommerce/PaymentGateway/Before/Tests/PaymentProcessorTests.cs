using NUnit.Framework;
using System.Threading.Tasks;
using PaymentGateway.Before.Src;

namespace PaymentGateway.Before.Tests
{
    [TestFixture]
    public class PaymentProcessorTests
    {
        private PaymentProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _processor = new PaymentProcessor();
        }

        // ============================================================
        // STRIPE TESTS
        // ============================================================

        [Test]
        public async Task ProcessStripePayment_ShouldReturnSuccess()
        {
            // Arrange
            decimal amount = 100m;
            string currency = "USD";
            string orderId = "ORD-001";

            // Act
            var result = await _processor.ProcessPaymentAsync("Stripe", amount, currency, orderId);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ProcessorName, Is.EqualTo("Stripe"));
        }

        [Test]
        public async Task ProcessStripePayment_ShouldGenerateTransactionId()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("Stripe", 100m, "USD", "ORD-002");

            // Assert
            Assert.That(result.TransactionId, Does.StartWith("stripe_txn_"));
        }

        [Test]
        public async Task ProcessStripePayment_ZeroAmount_ShouldFail()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("Stripe", 0m, "USD", "ORD-003");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task ProcessStripePayment_NullCurrency_ShouldFail()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("Stripe", 100m, null, "ORD-004");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task ProcessStripePayment_MultiplePayments_ShouldSucceed()
        {
            // Act & Assert
            for (int i = 1; i <= 5; i++)
            {
                var result = await _processor.ProcessPaymentAsync("Stripe", 100m, "USD", $"ORD-{i:00}");
                Assert.That(result.Success, Is.True);
            }
        }

        // ============================================================
        // PAYPAL TESTS
        // ============================================================

        [Test]
        public async Task ProcessPayPalPayment_ShouldReturnSuccess()
        {
            // Arrange
            decimal amount = 150m;

            // Act
            var result = await _processor.ProcessPaymentAsync("PayPal", amount, "USD", "ORD-010");

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ProcessorName, Is.EqualTo("PayPal"));
        }

        [Test]
        public async Task ProcessPayPalPayment_ShouldGenerateTransactionId()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("PayPal", 100m, "USD", "ORD-011");

            // Assert
            Assert.That(result.TransactionId, Does.StartWith("paypal_txn_"));
        }

        [Test]
        public async Task ProcessPayPalPayment_ZeroAmount_ShouldFail()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("PayPal", 0m, "USD", "ORD-012");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task ProcessPayPalPayment_NullCurrency_ShouldFail()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("PayPal", 100m, null, "ORD-013");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task ProcessPayPalPayment_DifferentCurrencies_ShouldSucceed()
        {
            // Act & Assert
            foreach (var currency in new[] { "USD", "EUR", "GBP" })
            {
                var result = await _processor.ProcessPaymentAsync("PayPal", 100m, currency, $"ORD-{currency}");
                Assert.That(result.Success, Is.True);
            }
        }

        // ============================================================
        // BANK TRANSFER TESTS
        // ============================================================

        [Test]
        public async Task ProcessBankTransferPayment_ShouldReturnSuccess()
        {
            // Arrange
            decimal amount = 500m;

            // Act
            var result = await _processor.ProcessPaymentAsync("BankTransfer", amount, "USD", "ORD-020");

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.ProcessorName, Is.EqualTo("BankTransfer"));
        }

        [Test]
        public async Task ProcessBankTransferPayment_ShouldGenerateTransactionId()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("BankTransfer", 100m, "USD", "ORD-021");

            // Assert
            Assert.That(result.TransactionId, Does.StartWith("bank_txn_"));
        }

        [Test]
        public async Task ProcessBankTransferPayment_ZeroAmount_ShouldFail()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("BankTransfer", 0m, "USD", "ORD-022");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task ProcessBankTransferPayment_LargeAmount_ShouldSucceed()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("BankTransfer", 50000m, "USD", "ORD-023");

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Amount, Is.EqualTo(50000m));
        }

        // ============================================================
        // ERROR HANDLING TESTS
        // ============================================================

        [Test]
        public async Task ProcessPayment_UnknownPaymentType_ShouldFail()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("UnknownType", 100m, "USD", "ORD-030");

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("Unknown"));
        }

        [Test]
        public async Task ProcessPayment_InvalidPaymentType_ShouldFail()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("CreditCard", 100m, "USD", "ORD-031");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task ProcessPayment_NegativeAmount_ShouldFail()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("Stripe", -100m, "USD", "ORD-032");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        // ============================================================
        // PROBLEM DEMONSTRATION TESTS
        // ============================================================

        [Test]
        public async Task Problem_HardCodedIfElse_DifficultyAddingNewPaymentType()
        {
            // This test demonstrates the problem:
            // To add a new payment type (e.g., Apple Pay), we must:
            // 1. Modify ProcessPaymentAsync() method (violates Open-Closed Principle)
            // 2. Add new private method for Apple Pay logic
            // 3. Update all tests
            // ❌ Not scalable!

            // Act - Try to use Apple Pay (not supported in Before version)
            var result = await _processor.ProcessPaymentAsync("ApplePay", 100m, "USD", "ORD-040");

            // Assert - Fails because hard-coded
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task Problem_MonolithicClass_ViolatesSingleResponsibility()
        {
            // BEFORE has all payment logic in ONE class:
            // - Stripe logic: ProcessStripePayment()
            // - PayPal logic: ProcessPayPalPayment()
            // - BankTransfer logic: ProcessBankTransferPayment()
            // ❌ Violates SRP: Class has too many reasons to change

            // This is just checking that it exists (problem is implicit)
            Assert.That(_processor, Is.Not.Null);
        }

        [Test]
        public async Task Problem_TightCoupling_CannotSwapImplementations()
        {
            // BEFORE has tight coupling via hard-coded string matching
            // Cannot easily mock or swap payment processors
            // ❌ No abstraction/interface to program against

            // Each payment method is accessed only by string
            var result1 = await _processor.ProcessPaymentAsync("Stripe", 100m, "USD", "ORD-050");
            var result2 = await _processor.ProcessPaymentAsync("PayPal", 100m, "USD", "ORD-051");

            // Both work but no way to swap implementations
            Assert.That(result1.ProcessorName, Is.EqualTo("Stripe"));
            Assert.That(result2.ProcessorName, Is.EqualTo("PayPal"));
        }

        [Test]
        public async Task AllPaymentTypes_ShouldWork()
        {
            // Act & Assert
            var types = new[] { "Stripe", "PayPal", "BankTransfer" };
            foreach (var type in types)
            {
                var result = await _processor.ProcessPaymentAsync(type, 100m, "USD", "ORD-060");
                Assert.That(result.Success, Is.True);
                Assert.That(result.ProcessorName, Is.EqualTo(type));
            }
        }

        [Test]
        public async Task ProcessPayment_HighAmounts_ShouldSucceed()
        {
            // Act & Assert
            decimal[] amounts = { 1000m, 10000m, 100000m };
            foreach (var amount in amounts)
            {
                var result = await _processor.ProcessPaymentAsync("Stripe", amount, "USD", $"ORD-{amount:00}");
                Assert.That(result.Success, Is.True);
            }
        }

        [Test]
        public async Task ProcessPayment_TransactionIdUniqueness_ShouldBeUnique()
        {
            // Act
            var txnIds = new System.Collections.Generic.HashSet<string>();
            for (int i = 0; i < 10; i++)
            {
                var result = await _processor.ProcessPaymentAsync("Stripe", 100m, "USD", $"ORD-{i:000}");
                txnIds.Add(result.TransactionId);
            }

            // Assert
            Assert.That(txnIds.Count, Is.EqualTo(10), "All transaction IDs should be unique");
        }

        [Test]
        public async Task ProcessPayment_EmptyCurrency_ShouldFail()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("Stripe", 100m, "", "ORD-070");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task ProcessPayment_CaseInsensitivePaymentType_ShouldFail()
        {
            // BEFORE version is case-sensitive (another limitation)
            // Act
            var result = await _processor.ProcessPaymentAsync("stripe", 100m, "USD", "ORD-071");

            // Assert - Fails because "stripe" != "Stripe"
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task ProcessPayment_NullPaymentType_ShouldFail()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync(null, 100m, "USD", "ORD-072");

            // Assert
            Assert.That(result.Success, Is.False);
        }

        [Test]
        public async Task ProcessPayment_SequentialProcessing_ShouldWork()
        {
            // Act
            var result1 = await _processor.ProcessPaymentAsync("Stripe", 100m, "USD", "ORD-080");
            var result2 = await _processor.ProcessPaymentAsync("PayPal", 100m, "USD", "ORD-081");
            var result3 = await _processor.ProcessPaymentAsync("BankTransfer", 100m, "USD", "ORD-082");

            // Assert
            Assert.That(result1.Success, Is.True);
            Assert.That(result2.Success, Is.True);
            Assert.That(result3.Success, Is.True);
        }

        [Test]
        public async Task ProcessPayment_MessageContent_ShouldBeDescriptive()
        {
            // Act
            var result = await _processor.ProcessPaymentAsync("Stripe", 100m, "USD", "ORD-090");

            // Assert
            Assert.That(result.Message, Is.Not.Null);
            Assert.That(result.Message, Is.Not.Empty);
        }
    }
}
