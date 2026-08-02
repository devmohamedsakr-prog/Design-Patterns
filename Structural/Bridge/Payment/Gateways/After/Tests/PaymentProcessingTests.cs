using Xunit;
using Bridge.Payment.Gateways.Abstraction;
using Bridge.Payment.Gateways.Implementation;

namespace Bridge.Payment.Gateways.Tests
{
    public class PaymentProcessingTests
    {
        [Fact]
        public void CreditCardPayment_ProcessWithStripe_Success()
        {
            var gateway = new StripeGateway("sk_test_123");
            gateway.Authorize();
            
            var payment = new CreditCardPayment(gateway)
            {
                CardNumber = "4111111111111111",
                CardholderName = "John Doe",
                ExpiryDate = "12/25",
                CVV = "123",
                Amount = 99.99m
            };

            var result = payment.Process();

            Assert.True(result.Success);
            Assert.NotEmpty(result.TransactionId);
            Assert.Equal("Completed", result.Status);
        }

        [Fact]
        public void DigitalWalletPayment_ProcessWithPayPal_Success()
        {
            var gateway = new PayPalGateway("client_123");
            gateway.Authorize();
            
            var payment = new DigitalWalletPayment(gateway)
            {
                WalletId = "user_123",
                WalletType = "PayPal",
                Amount = 49.99m
            };

            var result = payment.Process();

            Assert.True(result.Success);
            Assert.Equal("PayPal", payment.WalletType);
        }

        [Fact]
        public void BankTransferPayment_ProcessWithPayPal_Success()
        {
            var gateway = new PayPalGateway("client_123");
            gateway.Authorize();
            
            var payment = new BankTransferPayment(gateway)
            {
                AccountNumber = "123456789",
                RoutingNumber = "987654321",
                BankName = "Bank of America",
                Amount = 500m
            };

            var result = payment.Process();

            Assert.True(result.Success);
            Assert.Equal("Pending", result.Status);
        }

        [Fact]
        public void CryptoPayment_ProcessWithCryptoGateway_Success()
        {
            var gateway = new CryptoGateway("http://localhost:8545");
            gateway.Authorize();
            
            var payment = new CryptoPayment(gateway)
            {
                WalletAddress = "0x123abc",
                CryptoType = "Ethereum",
                Amount = 1.5m
            };

            var result = payment.Process();

            Assert.True(result.Success);
        }

        [Fact]
        public void Payment_SwitchGateway_Success()
        {
            var stripe = new StripeGateway("sk_test_123");
            stripe.Authorize();
            
            var payment = new CreditCardPayment(stripe)
            {
                CardNumber = "4111111111111111",
                CardholderName = "John Doe",
                ExpiryDate = "12/25",
                CVV = "123",
                Amount = 100m
            };

            var result1 = payment.Process();
            Assert.True(result1.Success);

            var square = new SquareGateway("sq_token_123");
            square.Authorize();
            payment.SetGateway(square);

            var result2 = payment.Process();
            Assert.True(result2.Success);
        }

        [Fact]
        public void PaymentProcessor_ProcessMultiplePayments_Success()
        {
            var gateway = new StripeGateway("sk_test_123");
            gateway.Authorize();
            
            var processor = new PaymentProcessor(gateway);

            processor.AddPayment(new CreditCardPayment(gateway)
            {
                CardNumber = "4111111111111111",
                CardholderName = "Alice",
                ExpiryDate = "12/25",
                CVV = "123",
                Amount = 50m
            });

            processor.AddPayment(new CreditCardPayment(gateway)
            {
                CardNumber = "5555555555554444",
                CardholderName = "Bob",
                ExpiryDate = "06/26",
                CVV = "456",
                Amount = 75m
            });

            var results = processor.ProcessAll();

            Assert.Equal(2, results.Count);
            Assert.True(results[0].Success);
            Assert.True(results[1].Success);
        }

        [Fact]
        public void PaymentProcessor_SetGateway_UpdatesAllPayments()
        {
            var stripe = new StripeGateway("sk_test_123");
            stripe.Authorize();
            
            var processor = new PaymentProcessor(stripe);
            processor.AddPayment(new CreditCardPayment(stripe) { Amount = 100m });

            var square = new SquareGateway("sq_token_123");
            square.Authorize();
            processor.SetGateway(square);

            var results = processor.ProcessAll();
            Assert.True(results[0].Success);
        }

        [Fact]
        public void StripeGateway_DoesNotSupportBankTransfer()
        {
            var gateway = new StripeGateway("sk_test_123");
            gateway.Authorize();
            
            var result = gateway.ProcessBankTransfer("123456789", "987654321", "Bank", 100m);

            Assert.False(result.Success);
            Assert.Contains("not supported", result.ErrorMessage);
        }

        [Fact]
        public void PayPalGateway_SupportsBankTransfer()
        {
            var gateway = new PayPalGateway("client_123");
            gateway.Authorize();
            
            var result = gateway.ProcessBankTransfer("123456789", "987654321", "Bank", 100m);

            Assert.True(result.Success);
        }

        [Fact]
        public void CryptoGateway_OnlySupportsCrypto()
        {
            var gateway = new CryptoGateway("http://localhost:8545");
            gateway.Authorize();
            
            var cardResult = gateway.ProcessCreditCard("4111", "John", "12/25", "123", 100m);
            Assert.False(cardResult.Success);

            var cryptoResult = gateway.ProcessCrypto("0x123", "Bitcoin", 1m);
            Assert.True(cryptoResult.Success);
        }

        [Fact]
        public void PaymentResult_ToString_ContainsInfo()
        {
            var result = new PaymentResult
            {
                Success = true,
                TransactionId = "txn_123",
                Amount = 99.99m,
                Status = "Completed"
            };

            var str = result.ToString();
            Assert.Contains("True", str);
            Assert.Contains("txn_123", str);
            Assert.Contains("Completed", str);
        }

        [Fact]
        public void Payment_WithNullGateway_ThrowsException()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new CreditCardPayment(null)
            );

            Assert.Contains("gateway", exception.Message);
        }

        [Fact]
        public void SetGateway_WithNullGateway_ThrowsException()
        {
            var gateway = new StripeGateway("sk_test_123");
            var payment = new CreditCardPayment(gateway);

            var exception = Assert.Throws<ArgumentNullException>(() =>
                payment.SetGateway(null)
            );

            Assert.Contains("gateway", exception.Message);
        }

        [Fact]
        public void PaymentProcessor_AddNullPayment_ThrowsException()
        {
            var gateway = new StripeGateway("sk_test_123");
            var processor = new PaymentProcessor(gateway);

            var exception = Assert.Throws<ArgumentNullException>(() =>
                processor.AddPayment(null)
            );

            Assert.Contains("payment", exception.Message);
        }

        [Fact]
        public void PaymentProcessor_SetNullGateway_ThrowsException()
        {
            var gateway = new StripeGateway("sk_test_123");
            var processor = new PaymentProcessor(gateway);

            var exception = Assert.Throws<ArgumentNullException>(() =>
                processor.SetGateway(null)
            );

            Assert.Contains("gateway", exception.Message);
        }

        [Fact]
        public void AllGateways_Authorize_Success()
        {
            var gateways = new IPaymentGateway[]
            {
                new StripeGateway("key"),
                new PayPalGateway("client"),
                new SquareGateway("token"),
                new CryptoGateway("url")
            };

            foreach (var gateway in gateways)
            {
                var authorized = gateway.Authorize();
                Assert.True(authorized);
            }
        }
    }
}
