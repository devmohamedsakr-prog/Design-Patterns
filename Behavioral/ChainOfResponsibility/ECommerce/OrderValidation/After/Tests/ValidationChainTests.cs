using NUnit.Framework;
using System;
using OrderValidation.After.Models;
using OrderValidation.After.Handlers;
using OrderValidation.After.Builders;

namespace OrderValidation.After.Tests
{
    [TestFixture]
    public class ValidationChainTests
    {
        private Order _validOrder;

        [SetUp]
        public void Setup()
        {
            _validOrder = new Order("ORD001", 500, 10, "Credit Card", "123 Main Street, New York, NY 10001");
        }

        // Individual handler tests
        [Test]
        public void InventoryHandler_ValidQuantity()
        {
            var handler = new InventoryHandler();
            var result = handler.Handle(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void InventoryHandler_InvalidQuantity_Zero()
        {
            var handler = new InventoryHandler();
            var order = new Order("ORD002", 100, 0, "Credit Card", "456 Oak Ave");
            var result = handler.Handle(order);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void InventoryHandler_InvalidQuantity_TooHigh()
        {
            var handler = new InventoryHandler(maxQuantity: 100);
            var order = new Order("ORD003", 100, 200, "Credit Card", "789 Pine Rd");
            var result = handler.Handle(order);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void PaymentHandler_ValidPayment()
        {
            var handler = new PaymentHandler();
            var result = handler.Handle(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void PaymentHandler_InvalidPaymentMethod()
        {
            var handler = new PaymentHandler();
            var order = new Order("ORD004", 500, 10, "Bitcoin", "123 Main St");
            var result = handler.Handle(order);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void PaymentHandler_InvalidAmount_Negative()
        {
            var handler = new PaymentHandler();
            var order = new Order("ORD005", -100, 10, "Credit Card", "123 Main St");
            var result = handler.Handle(order);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void FraudHandler_ValidAmount()
        {
            var handler = new FraudHandler();
            var result = handler.Handle(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void FraudHandler_HighAmount()
        {
            var handler = new FraudHandler(highAmountThreshold: 5000);
            var order = new Order("ORD006", 10000, 10, "Credit Card", "123 Main St");
            var result = handler.Handle(order);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void FraudHandler_HighQuantity()
        {
            var handler = new FraudHandler(highQuantityThreshold: 100);
            var order = new Order("ORD007", 500, 200, "Credit Card", "123 Main St");
            var result = handler.Handle(order);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ShippingHandler_ValidAddress()
        {
            var handler = new ShippingHandler();
            var result = handler.Handle(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void ShippingHandler_InvalidAddress_Empty()
        {
            var handler = new ShippingHandler();
            var order = new Order("ORD008", 500, 10, "Credit Card", "");
            var result = handler.Handle(order);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void ShippingHandler_InvalidAddress_TooShort()
        {
            var handler = new ShippingHandler(minAddressLength: 20);
            var order = new Order("ORD009", 500, 10, "Credit Card", "123 Main");
            var result = handler.Handle(order);
            Assert.That(result.IsValid, Is.False);
        }

        // Chain tests
        [Test]
        public void SimpleChain_AllValid()
        {
            var chain = new InventoryHandler()
                .SetNext(new PaymentHandler());

            var result = chain.Handle(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void CompleteChain_AllValid()
        {
            var chain = new InventoryHandler()
                .SetNext(new PaymentHandler())
                .SetNext(new FraudHandler())
                .SetNext(new ShippingHandler());

            var result = chain.Handle(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void CompleteChain_FailsAtInventory()
        {
            var chain = new InventoryHandler(maxQuantity: 5)
                .SetNext(new PaymentHandler())
                .SetNext(new FraudHandler())
                .SetNext(new ShippingHandler());

            var result = chain.Handle(_validOrder);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.HandlerName, Contains.Substring("Inventory"));
        }

        [Test]
        public void CompleteChain_FailsAtPayment()
        {
            var chain = new InventoryHandler()
                .SetNext(new PaymentHandler())
                .SetNext(new FraudHandler())
                .SetNext(new ShippingHandler());

            var order = new Order("ORD010", 500, 10, "InvalidMethod", "123 Main St");
            var result = chain.Handle(order);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.HandlerName, Contains.Substring("Payment"));
        }

        [Test]
        public void CompleteChain_FailsAtFraud()
        {
            var chain = new InventoryHandler()
                .SetNext(new PaymentHandler())
                .SetNext(new FraudHandler(highAmountThreshold: 1000))
                .SetNext(new ShippingHandler());

            var order = new Order("ORD011", 5000, 10, "Credit Card", "123 Main St");
            var result = chain.Handle(order);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.HandlerName, Contains.Substring("Fraud"));
        }

        [Test]
        public void CompleteChain_FailsAtShipping()
        {
            var chain = new InventoryHandler()
                .SetNext(new PaymentHandler())
                .SetNext(new FraudHandler())
                .SetNext(new ShippingHandler(minAddressLength: 50));

            var result = chain.Handle(_validOrder);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.HandlerName, Contains.Substring("Shipping"));
        }

        // Builder tests
        [Test]
        public void Builder_CreateChain()
        {
            var builder = new ValidationChainBuilder()
                .AddInventoryCheck()
                .AddPaymentCheck()
                .AddFraudCheck()
                .AddShippingCheck();

            var chain = builder.Build();
            var result = chain.Handle(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Builder_ValidateOrder()
        {
            var builder = new ValidationChainBuilder()
                .AddInventoryCheck()
                .AddPaymentCheck()
                .AddFraudCheck()
                .AddShippingCheck();

            var result = builder.Validate(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Builder_EmptyChain_Throws()
        {
            var builder = new ValidationChainBuilder();
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Test]
        public void Builder_SelectiveHandlers()
        {
            var builder = new ValidationChainBuilder()
                .AddInventoryCheck()
                .AddShippingCheck();

            var chain = builder.Build();
            var result = chain.Handle(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        // Real-world scenarios
        [Test]
        public void Scenario_ValidOrder()
        {
            var builder = new ValidationChainBuilder()
                .AddInventoryCheck()
                .AddPaymentCheck()
                .AddFraudCheck()
                .AddShippingCheck();

            var result = builder.Validate(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Scenario_LoyaltyCustomer_HigherLimits()
        {
            var builder = new ValidationChainBuilder()
                .AddInventoryCheck(maxQuantity: 5000)
                .AddPaymentCheck(maxAmount: 100000)
                .AddFraudCheck(highAmountThreshold: 50000)
                .AddShippingCheck();

            var order = new Order("LOY001", 25000, 500, "Credit Card", "VIP Address, New York, NY");
            var result = builder.Validate(order);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Scenario_InternationalOrder_StrictValidation()
        {
            var builder = new ValidationChainBuilder()
                .AddInventoryCheck(maxQuantity: 100)
                .AddPaymentCheck(maxAmount: 5000)
                .AddFraudCheck(highAmountThreshold: 2000, highQuantityThreshold: 50)
                .AddShippingCheck(minAddressLength: 30);

            var order = new Order("INTL001", 3000, 50, "Bank Transfer", "International Address, London, UK");
            var result = builder.Validate(order);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Scenario_MultipleFailures_StopsAtFirst()
        {
            var builder = new ValidationChainBuilder()
                .AddInventoryCheck(maxQuantity: 5)
                .AddPaymentCheck()
                .AddFraudCheck()
                .AddShippingCheck();

            var order = new Order("BAD001", 500, 100, "InvalidMethod", "");
            var result = builder.Validate(order);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.HandlerName, Contains.Substring("Inventory"));
        }

        [Test]
        public void Scenario_BulkOrder_InventoryFocus()
        {
            var builder = new ValidationChainBuilder()
                .AddInventoryCheck(maxQuantity: 1000)
                .AddShippingCheck();

            var order = new Order("BULK001", 100, 500, "Credit Card", "Warehouse Address, Chicago, IL");
            var result = builder.Validate(order);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Chain_DifferentOrder()
        {
            // Fraud check BEFORE payment
            var chain = new FraudHandler()
                .SetNext(new PaymentHandler())
                .SetNext(new InventoryHandler())
                .SetNext(new ShippingHandler());

            var result = chain.Handle(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Chain_SkipHandlers_PartialChain()
        {
            // Only check inventory and shipping, skip fraud and payment
            var chain = new InventoryHandler()
                .SetNext(new ShippingHandler());

            var result = chain.Handle(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }

        [Test]
        public void Chain_SingleHandler()
        {
            var chain = new InventoryHandler();
            var result = chain.Handle(_validOrder);
            Assert.That(result.IsValid, Is.True);
        }
    }
}
