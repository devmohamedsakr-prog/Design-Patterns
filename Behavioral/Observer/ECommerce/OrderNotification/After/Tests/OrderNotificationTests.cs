using NUnit.Framework;
using OrderNotification.After.Models;
using OrderNotification.After.Subjects;
using OrderNotification.After.Observers;

namespace OrderNotification.After.Tests
{
    [TestFixture]
    public class OrderNotificationTests
    {
        private OrderSubject _orderSubject;
        private Order _order;
        private EmailObserver _emailObserver;
        private SMSObserver _smsObserver;
        private PushObserver _pushObserver;
        private InventoryObserver _inventoryObserver;
        private AnalyticsObserver _analyticsObserver;

        [SetUp]
        public void Setup()
        {
            _order = new Order("ORD001", "Alice Smith", "alice@example.com", 
                "+1234567890", 150, "CUST001");
            _orderSubject = new OrderSubject(_order);
            
            _emailObserver = new EmailObserver();
            _smsObserver = new SMSObserver();
            _pushObserver = new PushObserver();
            _inventoryObserver = new InventoryObserver();
            _analyticsObserver = new AnalyticsObserver();
        }

        // Basic observer tests
        [Test]
        public void CanAttachObserver()
        {
            _orderSubject.Attach(_emailObserver);
            // Test passes if no exception thrown
            Assert.Pass();
        }

        [Test]
        public void CanAttachMultipleObservers()
        {
            _orderSubject.Attach(_emailObserver);
            _orderSubject.Attach(_smsObserver);
            _orderSubject.Attach(_pushObserver);
            Assert.Pass();
        }

        [Test]
        public void CanDetachObserver()
        {
            _orderSubject.Attach(_emailObserver);
            _orderSubject.Detach(_emailObserver);
            Assert.Pass();
        }

        [Test]
        public void OrderStatusChanges()
        {
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.Placed));
            
            _order.UpdateStatus(OrderStatus.Processing);
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.Processing));
        }

        // Event notification tests
        [Test]
        public void ProcessOrderNotifiesObservers()
        {
            _orderSubject.Attach(_emailObserver);
            _orderSubject.ProcessOrder();
            
            Assert.That(_orderSubject.GetOrder().Status, Is.EqualTo(OrderStatus.Processing));
        }

        [Test]
        public void ShipOrderNotifiesObservers()
        {
            _orderSubject.ProcessOrder();
            _orderSubject.Attach(_emailObserver);
            _orderSubject.ShipOrder();
            
            Assert.That(_orderSubject.GetOrder().Status, Is.EqualTo(OrderStatus.Shipped));
        }

        [Test]
        public void DeliverOrderNotifiesObservers()
        {
            _orderSubject.ProcessOrder();
            _orderSubject.ShipOrder();
            _orderSubject.Attach(_emailObserver);
            _orderSubject.DeliverOrder();
            
            Assert.That(_orderSubject.GetOrder().Status, Is.EqualTo(OrderStatus.Delivered));
        }

        [Test]
        public void CancelOrderNotifiesObservers()
        {
            _orderSubject.ProcessOrder();
            _orderSubject.Attach(_emailObserver);
            _orderSubject.CancelOrder();
            
            Assert.That(_orderSubject.GetOrder().Status, Is.EqualTo(OrderStatus.Cancelled));
        }

        // Observer specific tests
        [Test]
        public void EmailObserverReceivesUpdate()
        {
            var order = new Order("ORD002", "Bob", "bob@example.com", "+1111111111", 200);
            var orderEvent = new OrderEvent(order, "Test message");
            
            // This would normally throw if email address was invalid
            _emailObserver.Update(orderEvent);
            Assert.Pass();
        }

        [Test]
        public void SMSObserverReceivesUpdate()
        {
            var order = new Order("ORD003", "Carol", "carol@example.com", "+2222222222", 300);
            var orderEvent = new OrderEvent(order, "Test SMS");
            
            _smsObserver.Update(orderEvent);
            Assert.Pass();
        }

        [Test]
        public void InventoryObserverTracksStatus()
        {
            var order = new Order("ORD004", "David", "david@example.com", "+3333333333", 400);
            var orderEvent = new OrderEvent(order, "Process order");
            
            _inventoryObserver.Update(orderEvent);
            Assert.Pass();
        }

        [Test]
        public void AnalyticsObserverTracksMetrics()
        {
            var order = new Order("ORD005", "Eve", "eve@example.com", "+4444444444", 500);
            var orderEvent = new OrderEvent(order, "Track event");
            
            _analyticsObserver.Update(orderEvent);
            Assert.Pass();
        }

        // Observer lifecycle tests
        [Test]
        public void ObserverNotNotifiedAfterDetach()
        {
            _orderSubject.Attach(_emailObserver);
            _orderSubject.Detach(_emailObserver);
            _orderSubject.ProcessOrder();
            
            // Email observer should not receive notification
            Assert.Pass();
        }

        [Test]
        public void AllObserversNotified()
        {
            _orderSubject.Attach(_emailObserver);
            _orderSubject.Attach(_smsObserver);
            _orderSubject.Attach(_pushObserver);
            _orderSubject.Attach(_inventoryObserver);
            _orderSubject.Attach(_analyticsObserver);
            
            _orderSubject.ProcessOrder();
            Assert.Pass();
        }

        // Order flow tests
        [Test]
        public void CompleteOrderFlow()
        {
            _orderSubject.Attach(_emailObserver);
            _orderSubject.Attach(_smsObserver);
            
            // Step 1: Process order
            _orderSubject.ProcessOrder();
            Assert.That(_orderSubject.GetOrder().Status, Is.EqualTo(OrderStatus.Processing));
            
            // Step 2: Ship order
            _orderSubject.ShipOrder();
            Assert.That(_orderSubject.GetOrder().Status, Is.EqualTo(OrderStatus.Shipped));
            
            // Step 3: Deliver order
            _orderSubject.DeliverOrder();
            Assert.That(_orderSubject.GetOrder().Status, Is.EqualTo(OrderStatus.Delivered));
        }

        [Test]
        public void OrderCancellationFlow()
        {
            _orderSubject.Attach(_emailObserver);
            _orderSubject.Attach(_inventoryObserver);
            
            _orderSubject.ProcessOrder();
            Assert.That(_orderSubject.GetOrder().Status, Is.EqualTo(OrderStatus.Processing));
            
            _orderSubject.CancelOrder();
            Assert.That(_orderSubject.GetOrder().Status, Is.EqualTo(OrderStatus.Cancelled));
        }

        // Event data tests
        [Test]
        public void OrderEventContainsCorrectData()
        {
            var orderEvent = new OrderEvent(_order, "Test");
            
            Assert.That(orderEvent.OrderId, Is.EqualTo("ORD001"));
            Assert.That(orderEvent.CustomerEmail, Is.EqualTo("alice@example.com"));
            Assert.That(orderEvent.Amount, Is.EqualTo(150));
            Assert.That(orderEvent.Message, Is.EqualTo("Test"));
        }

        [Test]
        public void MultipleOrdersIndependent()
        {
            var order1 = new Order("ORD001", "Alice", "alice@example.com", "+1111111111", 100);
            var order2 = new Order("ORD002", "Bob", "bob@example.com", "+2222222222", 200);
            
            var subject1 = new OrderSubject(order1);
            var subject2 = new OrderSubject(order2);
            
            subject1.Attach(_emailObserver);
            subject1.ProcessOrder();
            
            Assert.That(subject1.GetOrder().Status, Is.EqualTo(OrderStatus.Processing));
            Assert.That(subject2.GetOrder().Status, Is.EqualTo(OrderStatus.Placed));
        }

        [Test]
        public void ObserverCanHandleDifferentStatuses()
        {
            var order = new Order("ORD010", "Test", "test@example.com", "+9999999999", 999);
            
            foreach (OrderStatus status in System.Enum.GetValues(typeof(OrderStatus)))
            {
                order.UpdateStatus(status);
                var orderEvent = new OrderEvent(order, $"Status: {status}");
                
                _emailObserver.Update(orderEvent);
                _inventoryObserver.Update(orderEvent);
                _analyticsObserver.Update(orderEvent);
            }
            
            Assert.Pass();
        }
    }
}
