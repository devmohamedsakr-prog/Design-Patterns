using Xunit;
using Facade.ECommerce.Checkout.Component;
using System.Collections.Generic;

namespace Facade.ECommerce.Checkout.Tests
{
    public class ECommerceFacadeTests
    {
        [Fact]
        public void CheckoutCart_ShouldCreateOrder()
        {
            var facade = new ECommerceFacade();
            var items = new List<CartItem> { new CartItem { ProductId = "PROD001", Quantity = 1, Price = 100 } };
            
            var order = facade.CheckoutCart(items, "customer@test.com", "1234567890123456", "123 Main St", "California");
            Assert.NotNull(order);
        }

        [Fact]
        public void CheckoutCart_ShouldCalculateTax()
        {
            var facade = new ECommerceFacade();
            var items = new List<CartItem> { new CartItem { ProductId = "PROD001", Quantity = 1, Price = 100 } };
            
            var order = facade.CheckoutCart(items, "customer@test.com", "1234567890123456", "123 Main St", "California");
            Assert.True(order.Tax > 0);
        }

        [Fact]
        public void CheckoutCart_ShouldCalculateShipping()
        {
            var facade = new ECommerceFacade();
            var items = new List<CartItem> { new CartItem { ProductId = "PROD001", Quantity = 1, Price = 100 } };
            
            var order = facade.CheckoutCart(items, "customer@test.com", "1234567890123456", "123 Main St", "Texas");
            Assert.True(order.Shipping > 0);
        }

        [Fact]
        public void CheckoutCart_ShouldGenerateOrderId()
        {
            var facade = new ECommerceFacade();
            var items = new List<CartItem> { new CartItem { ProductId = "PROD001", Quantity = 1, Price = 100 } };
            
            var order = facade.CheckoutCart(items, "customer@test.com", "1234567890123456", "123 Main St", "California");
            Assert.NotEmpty(order.OrderId);
        }

        [Fact]
        public void CheckoutCart_ShouldGenerateTrackingNumber()
        {
            var facade = new ECommerceFacade();
            var items = new List<CartItem> { new CartItem { ProductId = "PROD001", Quantity = 1, Price = 100 } };
            
            var order = facade.CheckoutCart(items, "customer@test.com", "1234567890123456", "123 Main St", "California");
            Assert.NotEmpty(order.TrackingNumber);
            Assert.StartsWith("TRACK", order.TrackingNumber);
        }

        [Fact]
        public void CheckoutCart_ShouldRejectInvalidCard()
        {
            var facade = new ECommerceFacade();
            var items = new List<CartItem> { new CartItem { ProductId = "PROD001", Quantity = 1, Price = 100 } };
            
            var order = facade.CheckoutCart(items, "customer@test.com", "invalid", "123 Main St", "California");
            Assert.Null(order);
        }

        [Fact]
        public void CheckoutCart_ShouldSendNotifications()
        {
            var facade = new ECommerceFacade();
            var items = new List<CartItem> { new CartItem { ProductId = "PROD001", Quantity = 1, Price = 100 } };
            
            facade.CheckoutCart(items, "customer@test.com", "1234567890123456", "123 Main St", "California");
            var notifications = facade.GetNotifications();
            
            Assert.NotEmpty(notifications);
        }

        [Fact]
        public void CheckoutCart_ShouldCalculateTotalPrice()
        {
            var facade = new ECommerceFacade();
            var items = new List<CartItem> { new CartItem { ProductId = "PROD001", Quantity = 2, Price = 100 } };
            
            var order = facade.CheckoutCart(items, "customer@test.com", "1234567890123456", "123 Main St", "California");
            Assert.Equal(order.Subtotal + order.Tax + order.Shipping, order.Total);
        }

        [Fact]
        public void CheckoutCart_ShouldHideComplexity()
        {
            var facade = new ECommerceFacade();
            var items = new List<CartItem> 
            { 
                new CartItem { ProductId = "PROD001", Quantity = 1, Price = 50 },
                new CartItem { ProductId = "PROD001", Quantity = 2, Price = 75 }
            };
            
            // Client calls single method instead of managing 5 subsystems
            var order = facade.CheckoutCart(items, "customer@test.com", "1234567890123456", "123 Main St", "New York");
            Assert.NotNull(order);
        }

        [Fact]
        public void CheckoutCart_ShouldSetOrderStatus()
        {
            var facade = new ECommerceFacade();
            var items = new List<CartItem> { new CartItem { ProductId = "PROD001", Quantity = 1, Price = 100 } };
            
            var order = facade.CheckoutCart(items, "customer@test.com", "1234567890123456", "123 Main St", "California");
            Assert.Equal("Confirmed", order.Status);
        }

        [Fact]
        public void CheckoutCart_ShouldDifferentiateTaxByRegion()
        {
            var facade = new ECommerceFacade();
            var items = new List<CartItem> { new CartItem { ProductId = "PROD001", Quantity = 1, Price = 100 } };
            
            var orderCA = facade.CheckoutCart(items, "customer@test.com", "1234567890123456", "123 Main St", "California");
            var orderTX = facade.CheckoutCart(items, "customer@test.com", "1234567890123456", "123 Main St", "Texas");
            
            Assert.NotEqual(orderCA.Tax, orderTX.Tax);
        }
    }
}
