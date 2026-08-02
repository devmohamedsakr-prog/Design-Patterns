using NUnit.Framework;
using Order.After.Context;

namespace Order.After.Tests
{
    [TestFixture]
    public class OrderMementoTests
    {
        private Order _order;
        private OrderCaretaker _caretaker;

        [SetUp]
        public void Setup()
        {
            _order = new Order("ORD-001", "CUST-001");
            _caretaker = new OrderCaretaker();
        }

        [Test]
        public void CreateOrderSnapshot_Success()
        {
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Laptop", UnitPrice = 999m, Quantity = 1 });
            _caretaker.SaveSnapshot(_order, "Order-Created");
            
            Assert.That(_caretaker.GetSnapshotCount(), Is.EqualTo(1));
        }

        [Test]
        public void SaveAndRestoreOrderState()
        {
            var item = new OrderItem { ProductId = "P1", ProductName = "Phone", UnitPrice = 500m, Quantity = 1 };
            _order.AddItem(item);
            _order.ConfirmOrder();
            decimal initialTotal = _order.GetTotal();

            _caretaker.SaveSnapshot(_order, "Checkpoint-1");
            
            _order.VerifyPayment();
            _order.ReserveInventory();
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.InventoryReserved));

            _caretaker.RestoreSnapshot(_order, "Checkpoint-1");
            Assert.That(_order.GetTotal(), Is.EqualTo(initialTotal));
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.Confirmed));
        }

        [Test]
        public void MultipleOrderSnapshots()
        {
            // Snapshot 1: Order created
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Phone", UnitPrice = 500m, Quantity = 1 });
            _order.ConfirmOrder();
            _caretaker.SaveSnapshot(_order, "Snap1-Confirmed");

            // Snapshot 2: Payment verified
            _order.VerifyPayment();
            _caretaker.SaveSnapshot(_order, "Snap2-PaymentVerified");

            // Snapshot 3: Inventory reserved
            _order.ReserveInventory();
            _caretaker.SaveSnapshot(_order, "Snap3-InventoryReserved");

            Assert.That(_caretaker.GetSnapshotCount(), Is.EqualTo(3));
        }

        [Test]
        public void RestoreToSpecificOrderCheckpoint()
        {
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Item", UnitPrice = 100m, Quantity = 1 });
            _order.ConfirmOrder();
            _caretaker.SaveSnapshot(_order, "Checkpoint-1");

            _order.VerifyPayment();
            _order.ReserveInventory();
            _order.PickItems();

            _caretaker.RestoreSnapshot(_order, "Checkpoint-1");
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.Confirmed));
            Assert.That(_order.Items.Count, Is.EqualTo(1));
        }

        [Test]
        public void OrderShippingMethodSnapshot()
        {
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Package", UnitPrice = 50m, Quantity = 1 });
            _order.SetShippingMethod("Standard");
            decimal standardTotal = _order.GetTotal();
            _caretaker.SaveSnapshot(_order, "StandardShipping");

            _order.SetShippingMethod("Express");
            Assert.That(_order.GetTotal(), Is.GreaterThan(standardTotal));

            _caretaker.RestoreSnapshot(_order, "StandardShipping");
            Assert.That(_order.ShippingMethod, Is.EqualTo("Standard"));
            Assert.That(_order.GetTotal(), Is.EqualTo(standardTotal));
        }

        [Test]
        public void GetAvailableOrderSnapshots()
        {
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Item", UnitPrice = 10m, Quantity = 1 });
            _caretaker.SaveSnapshot(_order, "Snap1");
            _caretaker.SaveSnapshot(_order, "Snap2");
            _caretaker.SaveSnapshot(_order, "Snap3");

            var snapshots = _caretaker.GetAvailableSnapshots();
            Assert.That(snapshots.Count, Is.EqualTo(3));
            Assert.That(snapshots, Does.Contain("Snap1"));
        }

        [Test]
        public void DeleteOrderSnapshot()
        {
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Item", UnitPrice = 10m, Quantity = 1 });
            _caretaker.SaveSnapshot(_order, "ToDelete");

            _caretaker.DeleteSnapshot("ToDelete");
            Assert.That(_caretaker.GetSnapshotCount(), Is.EqualTo(0));
        }

        [Test]
        public void RestoreNonExistentOrderSnapshot_Fails()
        {
            _caretaker.RestoreSnapshot(_order, "NonExistent");
            // Should not throw, just log
            Assert.That(_order.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public void OrderProcessingHistory()
        {
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Item1", UnitPrice = 10m, Quantity = 1 });
            _order.ConfirmOrder();
            _caretaker.SaveSnapshot(_order, "Snap1-Confirmed");

            _order.VerifyPayment();
            _caretaker.SaveSnapshot(_order, "Snap2-PaymentVerified");

            var history = _caretaker.GetHistory();
            Assert.That(history.Count, Is.EqualTo(2));
            Assert.That(history[0].Status, Is.EqualTo(OrderStatus.Confirmed));
            Assert.That(history[1].Status, Is.EqualTo(OrderStatus.PaymentVerified));
        }

        [Test]
        public void ComplexOrderWorkflow()
        {
            // Build order
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Laptop", UnitPrice = 999m, Quantity = 1 });
            _order.AddItem(new OrderItem { ProductId = "P2", ProductName = "Mouse", UnitPrice = 25m, Quantity = 2 });
            _order.ConfirmOrder();
            decimal checkpointTotal = _order.GetTotal();
            _caretaker.SaveSnapshot(_order, "BeforePayment");

            // Process order
            _order.VerifyPayment();
            _order.ReserveInventory();
            _order.PickItems();

            // Rollback to payment verification stage
            _caretaker.RestoreSnapshot(_order, "BeforePayment");
            Assert.That(_order.GetTotal(), Is.EqualTo(checkpointTotal));
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.Confirmed));
            Assert.That(_order.Items.Count, Is.EqualTo(2));
        }

        [Test]
        public void OrderSnapshotIsolation()
        {
            var item1 = new OrderItem { ProductId = "P1", ProductName = "Item1", UnitPrice = 100m, Quantity = 1 };
            _order.AddItem(item1);
            _order.ConfirmOrder();
            _caretaker.SaveSnapshot(_order, "Snapshot1");

            // Modify original order
            _order.RemoveItem("P1");
            _order.AddItem(new OrderItem { ProductId = "P2", ProductName = "Item2", UnitPrice = 50m, Quantity = 1 });
            _order.VerifyPayment();

            // Retrieve snapshot and verify isolation
            var snapshot = _caretaker.GetSnapshot("Snapshot1");
            Assert.That(snapshot?.Items.Count, Is.EqualTo(1));
            Assert.That(snapshot?.Items[0].ProductId, Is.EqualTo("P1"));
            Assert.That(snapshot?.Status, Is.EqualTo(OrderStatus.Confirmed));

            // Current order should be different
            Assert.That(_order.Items.Count, Is.EqualTo(1));
            Assert.That(_order.Items[0].ProductId, Is.EqualTo("P2"));
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.PaymentVerified));
        }

        [Test]
        public void OrderSnapshotTimestamp()
        {
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Item", UnitPrice = 10m, Quantity = 1 });
            var beforeTime = DateTime.Now;
            _caretaker.SaveSnapshot(_order, "TimedSnapshot");
            var afterTime = DateTime.Now;

            var snapshot = _caretaker.GetSnapshot("TimedSnapshot");
            Assert.That(snapshot?.SnapshotTime, Is.GreaterThanOrEqualTo(beforeTime));
            Assert.That(snapshot?.SnapshotTime, Is.LessThanOrEqualTo(afterTime.AddSeconds(1)));
        }

        [Test]
        public void MultipleOrderRestores()
        {
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Item", UnitPrice = 10m, Quantity = 1 });
            _order.ConfirmOrder();
            _caretaker.SaveSnapshot(_order, "Stable");

            // First restore
            _order.VerifyPayment();
            _caretaker.RestoreSnapshot(_order, "Stable");
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.Confirmed));

            // Second restore
            _order.ShipOrder();
            _caretaker.RestoreSnapshot(_order, "Stable");
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.Confirmed));
        }

        [Test]
        public void OrderProcessingFailureRecovery()
        {
            // Initial order
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Laptop", UnitPrice = 999m, Quantity = 1 });
            _order.ConfirmOrder();
            _caretaker.SaveSnapshot(_order, "AfterConfirm");

            // Start processing
            _order.VerifyPayment();
            _order.ReserveInventory();
            _caretaker.SaveSnapshot(_order, "AfterReserve");

            // Simulate failure - accidentally shipped before picking
            _order.ShipOrder();
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.Shipped));

            // Recover from error
            _caretaker.RestoreSnapshot(_order, "AfterReserve");
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.InventoryReserved));
        }

        [Test]
        public void OrderShippingStrategyComparison()
        {
            // Build base order
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Box", UnitPrice = 100m, Quantity = 1 });
            
            // Strategy 1: Standard
            _order.SetShippingMethod("Standard");
            decimal standardTotal = _order.GetTotal();
            _caretaker.SaveSnapshot(_order, "Standard");

            // Strategy 2: Express
            _order.SetShippingMethod("Express");
            decimal expressTotal = _order.GetTotal();
            _caretaker.SaveSnapshot(_order, "Express");

            // Strategy 3: International
            _order.SetShippingMethod("International");
            decimal internationalTotal = _order.GetTotal();
            _caretaker.SaveSnapshot(_order, "International");

            // Compare
            decimal standardVsExpress = _caretaker.CompareOrderTotals("Standard", "Express");
            decimal expressVsIntl = _caretaker.CompareOrderTotals("Express", "International");

            Assert.That(standardVsExpress, Is.EqualTo(15m));  // $25 - $10
            Assert.That(expressVsIntl, Is.EqualTo(25m));     // $50 - $25
        }

        [Test]
        public void OrderCompleteWorkflowRollback()
        {
            // Create order
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Item1", UnitPrice = 100m, Quantity = 2 });
            _order.AddItem(new OrderItem { ProductId = "P2", ProductName = "Item2", UnitPrice = 50m, Quantity = 1 });

            // Step 1: Confirm
            _order.ConfirmOrder();
            _caretaker.SaveSnapshot(_order, "Step1-Confirmed");

            // Step 2: Payment
            _order.VerifyPayment();
            _caretaker.SaveSnapshot(_order, "Step2-PaymentVerified");

            // Step 3: Reserve
            _order.ReserveInventory();
            _caretaker.SaveSnapshot(_order, "Step3-Reserved");

            // Step 4: Pick
            _order.PickItems();
            _caretaker.SaveSnapshot(_order, "Step4-Picked");

            // Rollback to Step 2
            _caretaker.RestoreSnapshot(_order, "Step2-PaymentVerified");
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.PaymentVerified));
            Assert.That(_order.Items.Count, Is.EqualTo(2));

            // Can continue from there
            _order.ReserveInventory();
            Assert.That(_order.Status, Is.EqualTo(OrderStatus.InventoryReserved));
        }

        [Test]
        public void OrderAddressChangeSnapshot()
        {
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Gift", UnitPrice = 50m, Quantity = 1 });
            
            var address1 = new ShippingAddress { Street = "123 Main", City = "City1", Country = "US", PostalCode = "12345" };
            _order.SetShippingAddress(address1);
            _caretaker.SaveSnapshot(_order, "AddressSnapshot1");

            var address2 = new ShippingAddress { Street = "456 Oak", City = "City2", Country = "US", PostalCode = "67890" };
            _order.SetShippingAddress(address2);

            var snapshot = _caretaker.GetSnapshot("AddressSnapshot1");
            Assert.That(snapshot?.ShippingAddress.Street, Is.EqualTo("123 Main"));
            Assert.That(_order.ShippingAddress.Street, Is.EqualTo("456 Oak"));
        }

        [Test]
        public void FullOrderLifecycle()
        {
            // Create and build
            _order.AddItem(new OrderItem { ProductId = "P1", ProductName = "Product", UnitPrice = 199.99m, Quantity = 3 });
            
            // Process through all stages, saving at key points
            _order.ConfirmOrder();
            _caretaker.SaveSnapshot(_order, "Confirmed");

            _order.VerifyPayment();
            _caretaker.SaveSnapshot(_order, "PaymentVerified");

            _order.ReserveInventory();
            _caretaker.SaveSnapshot(_order, "InventoryReserved");

            _order.PickItems();
            _caretaker.SaveSnapshot(_order, "Picked");

            _order.PackageOrder();
            _caretaker.SaveSnapshot(_order, "Packaged");

            _order.ShipOrder();
            _caretaker.SaveSnapshot(_order, "Shipped");

            _order.DeliverOrder();
            _caretaker.SaveSnapshot(_order, "Delivered");

            // Verify history
            var history = _caretaker.GetHistory();
            Assert.That(history.Count, Is.EqualTo(7));
            Assert.That(history[0].Status, Is.EqualTo(OrderStatus.Confirmed));
            Assert.That(history[6].Status, Is.EqualTo(OrderStatus.Delivered));
        }
    }
}
