using NUnit.Framework;
using OrderLifecycle.After.Context;

namespace OrderLifecycle.After.Tests
{
    [TestFixture]
    public class OrderLifecycleTests
    {
        private Order _order;

        [SetUp]
        public void Setup()
        {
            _order = new Order("ORD-001", 99.99m);
        }

        // ============================================================
        // INITIAL STATE TESTS
        // ============================================================

        [Test]
        public void Order_InitialState_ShouldBePending()
        {
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Pending"));
        }

        [Test]
        public void Order_Pending_CanProcessPayment()
        {
            Assert.That(_order.CanPayment(), Is.True);
        }

        [Test]
        public void Order_Pending_CannotShip()
        {
            Assert.That(_order.CanShip(), Is.False);
        }

        [Test]
        public void Order_Pending_CanCancel()
        {
            Assert.That(_order.CanCancel(), Is.True);
        }

        // ============================================================
        // STATE TRANSITION TESTS
        // ============================================================

        [Test]
        public void Order_ProcessPayment_TransitionsToProcessing()
        {
            bool result = _order.ProcessPayment();
            Assert.That(result, Is.True);
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Processing"));
        }

        [Test]
        public void Order_ProcessingToShipped_Succeeds()
        {
            _order.ProcessPayment();
            bool result = _order.ShipOrder();
            Assert.That(result, Is.True);
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Shipped"));
        }

        [Test]
        public void Order_ShippedToDelivered_Succeeds()
        {
            _order.ProcessPayment();
            _order.ShipOrder();
            bool result = _order.DeliverOrder();
            Assert.That(result, Is.True);
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Delivered"));
        }

        [Test]
        public void Order_FullLifecycle_PendingToDelivered()
        {
            Assert.That(_order.ProcessPayment(), Is.True);
            Assert.That(_order.ShipOrder(), Is.True);
            Assert.That(_order.DeliverOrder(), Is.True);
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Delivered"));
        }

        // ============================================================
        // INVALID TRANSITION TESTS
        // ============================================================

        [Test]
        public void Order_CannotShipFromPending()
        {
            bool result = _order.ShipOrder();
            Assert.That(result, Is.False);
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Pending"));
        }

        [Test]
        public void Order_CannotDeliverFromPending()
        {
            bool result = _order.DeliverOrder();
            Assert.That(result, Is.False);
        }

        [Test]
        public void Order_CannotPaymentTwice()
        {
            _order.ProcessPayment();
            bool result = _order.ProcessPayment();
            Assert.That(result, Is.False);
        }

        [Test]
        public void Order_CannotShipFromPendingState()
        {
            Assert.That(_order.CanShip(), Is.False);
            bool result = _order.ShipOrder();
            Assert.That(result, Is.False);
        }

        // ============================================================
        // CANCELLATION TESTS
        // ============================================================

        [Test]
        public void Order_CancelFromPending_Succeeds()
        {
            bool result = _order.CancelOrder();
            Assert.That(result, Is.True);
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Cancelled"));
        }

        [Test]
        public void Order_CancelFromProcessing_Succeeds()
        {
            _order.ProcessPayment();
            bool result = _order.CancelOrder();
            Assert.That(result, Is.True);
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Cancelled"));
        }

        [Test]
        public void Order_CannotCancelFromShipped()
        {
            _order.ProcessPayment();
            _order.ShipOrder();
            bool result = _order.CancelOrder();
            Assert.That(result, Is.False);
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Shipped"));
        }

        [Test]
        public void Order_CannotCancelFromDelivered()
        {
            _order.ProcessPayment();
            _order.ShipOrder();
            _order.DeliverOrder();
            bool result = _order.CancelOrder();
            Assert.That(result, Is.False);
        }

        // ============================================================
        // STATE PERMISSIONS TESTS
        // ============================================================

        [Test]
        public void Order_Processing_CanShip()
        {
            _order.ProcessPayment();
            Assert.That(_order.CanShip(), Is.True);
        }

        [Test]
        public void Order_Processing_CannotDeliver()
        {
            _order.ProcessPayment();
            Assert.That(_order.CanDeliver(), Is.False);
        }

        [Test]
        public void Order_Shipped_CanDeliver()
        {
            _order.ProcessPayment();
            _order.ShipOrder();
            Assert.That(_order.CanDeliver(), Is.True);
        }

        [Test]
        public void Order_Delivered_NoOperations()
        {
            _order.ProcessPayment();
            _order.ShipOrder();
            _order.DeliverOrder();

            Assert.That(_order.CanPayment(), Is.False);
            Assert.That(_order.CanShip(), Is.False);
            Assert.That(_order.CanDeliver(), Is.False);
            Assert.That(_order.CanCancel(), Is.False);
        }

        // ============================================================
        // STATE HISTORY TESTS
        // ============================================================

        [Test]
        public void Order_StateHistory_RecordsTransitions()
        {
            _order.ProcessPayment();
            _order.ShipOrder();
            
            var history = _order.GetStateHistory();
            Assert.That(history.Count, Is.GreaterThan(2));
            Assert.That(history[0], Does.Contain("Pending"));
            Assert.That(history[1], Does.Contain("Processing"));
        }

        [Test]
        public void Order_StateHistory_IncludesTimestamps()
        {
            _order.ProcessPayment();
            var history = _order.GetStateHistory();
            
            Assert.That(history[1], Does.Contain(System.DateTime.UtcNow.ToString("yyyy-MM-dd")));
        }

        [Test]
        public void Order_StateHistory_TracksCancellation()
        {
            _order.CancelOrder();
            var history = _order.GetStateHistory();
            
            Assert.That(history[1], Does.Contain("Cancelled"));
        }

        // ============================================================
        // ORDER PROPERTIES TESTS
        // ============================================================

        [Test]
        public void Order_PropertiesSet_Correctly()
        {
            Assert.That(_order.OrderId, Is.EqualTo("ORD-001"));
            Assert.That(_order.Amount, Is.EqualTo(99.99m));
        }

        [Test]
        public void Order_CreatedAtSet_OnConstruction()
        {
            Assert.That(_order.CreatedAt, Is.LessThanOrEqualTo(System.DateTime.UtcNow));
        }

        // ============================================================
        // MULTIPLE ORDERS TESTS
        // ============================================================

        [Test]
        public void Multiple_Orders_IndependentStates()
        {
            var order2 = new Order("ORD-002", 199.99m);
            
            _order.ProcessPayment();
            
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Processing"));
            Assert.That(order2.GetCurrentStateName(), Is.EqualTo("Pending"));
        }

        [Test]
        public void Multiple_Orders_DifferentPaths()
        {
            var order2 = new Order("ORD-002", 50.00m);
            
            _order.ProcessPayment();
            _order.ShipOrder();
            
            order2.CancelOrder();
            
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Shipped"));
            Assert.That(order2.GetCurrentStateName(), Is.EqualTo("Cancelled"));
        }

        // ============================================================
        // EDGE CASE TESTS
        // ============================================================

        [Test]
        public void Order_ZeroAmount_StillProcesses()
        {
            var freeOrder = new Order("ORD-FREE", 0m);
            bool result = freeOrder.ProcessPayment();
            Assert.That(result, Is.True);
        }

        [Test]
        public void Order_LargeAmount_Processes()
        {
            var largeOrder = new Order("ORD-LARGE", 999999.99m);
            bool result = largeOrder.ProcessPayment();
            Assert.That(result, Is.True);
        }

        [Test]
        public void Order_SpecialCharsInId_Accepted()
        {
            var specialOrder = new Order("ORD-2024-01-001-ABC", 100m);
            Assert.That(specialOrder.OrderId, Does.Contain("ORD"));
        }

        // ============================================================
        // COMPREHENSIVE WORKFLOW TESTS
        // ============================================================

        [Test]
        public void Order_CompleteWorkflow_AllStates()
        {
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Pending"));
            Assert.That(_order.ProcessPayment(), Is.True);
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Processing"));
            Assert.That(_order.ShipOrder(), Is.True);
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Shipped"));
            Assert.That(_order.DeliverOrder(), Is.True);
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Delivered"));
        }

        [Test]
        public void Order_CancelledWorkflow()
        {
            _order.ProcessPayment();
            _order.CancelOrder();
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Cancelled"));
        }

        [Test]
        public void Order_ImmediateCancel_FromPending()
        {
            _order.CancelOrder();
            Assert.That(_order.GetCurrentStateName(), Is.EqualTo("Cancelled"));
            var history = _order.GetStateHistory();
            Assert.That(history.Count, Is.EqualTo(2));
        }

        [Test]
        public void Order_StateNameConsistency()
        {
            Assert.That(_order.GetCurrentStateName().Length, Is.GreaterThan(0));
            _order.ProcessPayment();
            Assert.That(_order.GetCurrentStateName().Length, Is.GreaterThan(0));
        }
    }
}
