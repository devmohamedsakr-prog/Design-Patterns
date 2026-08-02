using NUnit.Framework;
using OrderProcessing.After.Context;

namespace OrderProcessing.After.Tests
{
    [TestFixture]
    public class OrderProcessingTests
    {
        private OrderProcessingChain _chain;

        [SetUp]
        public void Setup() => _chain = new OrderProcessingChain();

        [Test]
        public void ValidOrder_SuccessfulProcessing()
        {
            var order = new Order 
            { 
                OrderId = "O001", 
                Items = new[] { "Item1", "Item2" }, 
                Amount = 100 
            };
            _chain.ProcessOrder(order);
            
            Assert.That(order.Status, Is.EqualTo("Ready for Shipping"));
            Assert.That(order.IsValid, Is.True);
        }

        [Test]
        public void InvalidOrder_ValidationFails()
        {
            var order = new Order { OrderId = "", Items = new string[0], Amount = 50 };
            _chain.ProcessOrder(order);
            
            Assert.That(order.Status, Is.EqualTo("Failed"));
        }

        [Test]
        public void InsufficientInventory()
        {
            var order = new Order 
            { 
                OrderId = "O002", 
                Items = new[] { "1", "2", "3", "4", "5", "6" }, 
                Amount = 200 
            };
            _chain.ProcessOrder(order);
            
            Assert.That(order.Status, Is.EqualTo("Failed"));
        }

        [Test]
        public void InvalidPaymentAmount()
        {
            var order = new Order 
            { 
                OrderId = "O003", 
                Items = new[] { "Item1" }, 
                Amount = -50 
            };
            _chain.ProcessOrder(order);
            
            Assert.That(order.Status, Is.EqualTo("Failed"));
        }

        [Test]
        public void FullOrderProcessing()
        {
            var order = new Order 
            { 
                OrderId = "O004", 
                Items = new[] { "Item1", "Item2", "Item3" }, 
                Amount = 500 
            };
            _chain.ProcessOrder(order);
            
            Assert.That(order.IsValid, Is.True);
            Assert.That(order.InventoryChecked, Is.True);
            Assert.That(order.PaymentProcessed, Is.True);
        }
    }
}
