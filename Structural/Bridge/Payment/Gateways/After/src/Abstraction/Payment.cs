using System;
using System.Collections.Generic;
using Bridge.Payment.Gateways.Implementation;

namespace Bridge.Payment.Gateways.Abstraction
{
    /// <summary>
    /// Abstraction: Payment processing operations.
    /// Demonstrates: Bridge pattern for payment gateway independence.
    /// </summary>
    public abstract class Payment
    {
        protected IPaymentGateway _gateway;

        public Payment(IPaymentGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        public abstract PaymentResult Process();

        public void SetGateway(IPaymentGateway gateway)
        {
            _gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }
    }

    /// <summary>
    /// Concrete abstraction: Credit card payment.
    /// </summary>
    public class CreditCardPayment : Payment
    {
        public string CardNumber { get; set; }
        public string CardholderName { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
        public decimal Amount { get; set; }

        public CreditCardPayment(IPaymentGateway gateway) : base(gateway)
        {
        }

        public override PaymentResult Process()
        {
            return _gateway.ProcessCreditCard(CardNumber, CardholderName, ExpiryDate, CVV, Amount);
        }

        public override string ToString() =>
            $"CreditCard Payment(Amount={Amount}, Card=****{CardNumber.Substring(CardNumber.Length - 4)})";
    }

    /// <summary>
    /// Concrete abstraction: Digital wallet payment.
    /// </summary>
    public class DigitalWalletPayment : Payment
    {
        public string WalletId { get; set; }
        public string WalletType { get; set; } // PayPal, ApplePay, GooglePay
        public decimal Amount { get; set; }

        public DigitalWalletPayment(IPaymentGateway gateway) : base(gateway)
        {
        }

        public override PaymentResult Process()
        {
            return _gateway.ProcessWallet(WalletId, WalletType, Amount);
        }

        public override string ToString() =>
            $"Digital Wallet Payment(Type={WalletType}, Amount={Amount})";
    }

    /// <summary>
    /// Concrete abstraction: Bank transfer payment.
    /// </summary>
    public class BankTransferPayment : Payment
    {
        public string AccountNumber { get; set; }
        public string RoutingNumber { get; set; }
        public string BankName { get; set; }
        public decimal Amount { get; set; }

        public BankTransferPayment(IPaymentGateway gateway) : base(gateway)
        {
        }

        public override PaymentResult Process()
        {
            return _gateway.ProcessBankTransfer(AccountNumber, RoutingNumber, BankName, Amount);
        }

        public override string ToString() =>
            $"Bank Transfer Payment(Bank={BankName}, Amount={Amount})";
    }

    /// <summary>
    /// Concrete abstraction: Cryptocurrency payment.
    /// </summary>
    public class CryptoPayment : Payment
    {
        public string WalletAddress { get; set; }
        public string CryptoType { get; set; } // Bitcoin, Ethereum, etc.
        public decimal Amount { get; set; }

        public CryptoPayment(IPaymentGateway gateway) : base(gateway)
        {
        }

        public override PaymentResult Process()
        {
            return _gateway.ProcessCrypto(WalletAddress, CryptoType, Amount);
        }

        public override string ToString() =>
            $"Crypto Payment(Type={CryptoType}, Amount={Amount})";
    }

    /// <summary>
    /// Payment result.
    /// </summary>
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string ErrorMessage { get; set; }
        public string Status { get; set; } // Pending, Completed, Failed, Refunded

        public override string ToString() =>
            $"PaymentResult(Success={Success}, TransactionId={TransactionId}, Status={Status})";
    }

    /// <summary>
    /// Payment processor managing multiple payment methods and gateways.
    /// </summary>
    public class PaymentProcessor
    {
        private readonly List<Payment> _payments = new List<Payment>();
        private IPaymentGateway _currentGateway;

        public PaymentProcessor(IPaymentGateway gateway)
        {
            _currentGateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        public void AddPayment(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));
            _payments.Add(payment);
        }

        public void SetGateway(IPaymentGateway gateway)
        {
            _currentGateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            foreach (var payment in _payments)
            {
                payment.SetGateway(gateway);
            }
        }

        public List<PaymentResult> ProcessAll()
        {
            var results = new List<PaymentResult>();
            foreach (var payment in _payments)
            {
                results.Add(payment.Process());
            }
            return results;
        }

        public int PaymentCount => _payments.Count;
    }
}
