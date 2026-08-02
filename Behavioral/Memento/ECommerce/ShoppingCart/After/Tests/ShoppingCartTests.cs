using NUnit.Framework;
using ShoppingCart.After.Context;

namespace ShoppingCart.After.Tests
{
    [TestFixture]
    public class ShoppingCartMementoTests
    {
        private ShoppingCart _cart;
        private CartCaretaker _caretaker;

        [SetUp]
        public void Setup()
        {
            _cart = new ShoppingCart("CUST-001");
            _caretaker = new CartCaretaker();
        }

        [Test]
        public void CreateSnapshot_Success()
        {
            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Laptop", Price = 999m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "Before-Laptop");
            
            Assert.That(_caretaker.GetSnapshotCount(), Is.EqualTo(1));
        }

        [Test]
        public void SaveAndRestore_SimpleCart()
        {
            var item = new CartItem { ProductId = "P1", ProductName = "Mouse", Price = 25m, Quantity = 1 };
            _cart.AddItem(item);
            decimal initialTotal = _cart.GetTotal();

            _caretaker.SaveSnapshot(_cart, "Checkpoint-1");
            
            _cart.ClearCart();
            Assert.That(_cart.GetTotal(), Is.EqualTo(0m));

            _caretaker.RestoreSnapshot(_cart, "Checkpoint-1");
            Assert.That(_cart.GetTotal(), Is.EqualTo(initialTotal));
        }

        [Test]
        public void MultipleSnapshots()
        {
            // Snapshot 1: One item
            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Phone", Price = 500m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "Snap1");

            // Snapshot 2: Two items
            _cart.AddItem(new CartItem { ProductId = "P2", ProductName = "Charger", Price = 30m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "Snap2");

            // Snapshot 3: Three items
            _cart.AddItem(new CartItem { ProductId = "P3", ProductName = "Case", Price = 15m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "Snap3");

            Assert.That(_caretaker.GetSnapshotCount(), Is.EqualTo(3));
        }

        [Test]
        public void RestoreToSpecificCheckpoint()
        {
            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Item1", Price = 10m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "Checkpoint-1");

            _cart.AddItem(new CartItem { ProductId = "P2", ProductName = "Item2", Price = 20m, Quantity = 1 });
            _cart.AddItem(new CartItem { ProductId = "P3", ProductName = "Item3", Price = 30m, Quantity = 1 });

            _caretaker.RestoreSnapshot(_cart, "Checkpoint-1");
            Assert.That(_cart.Items.Count, Is.EqualTo(1));
            Assert.That(_cart.GetTotal(), Is.EqualTo(10m));
        }

        [Test]
        public void SnapshotQuantityUpdate()
        {
            var item = new CartItem { ProductId = "P1", ProductName = "Widget", Price = 50m, Quantity = 1 };
            _cart.AddItem(item);
            _caretaker.SaveSnapshot(_cart, "Original");

            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Widget", Price = 50m, Quantity = 2 });
            Assert.That(_cart.Items[0].Quantity, Is.EqualTo(3));

            _caretaker.RestoreSnapshot(_cart, "Original");
            Assert.That(_cart.Items[0].Quantity, Is.EqualTo(1));
        }

        [Test]
        public void GetAvailableSnapshots()
        {
            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Item", Price = 10m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "Snap1");
            _caretaker.SaveSnapshot(_cart, "Snap2");
            _caretaker.SaveSnapshot(_cart, "Snap3");

            var snapshots = _caretaker.GetAvailableSnapshots();
            Assert.That(snapshots.Count, Is.EqualTo(3));
            Assert.That(snapshots, Does.Contain("Snap1"));
            Assert.That(snapshots, Does.Contain("Snap2"));
            Assert.That(snapshots, Does.Contain("Snap3"));
        }

        [Test]
        public void DeleteSnapshot()
        {
            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Item", Price = 10m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "ToDelete");

            _caretaker.DeleteSnapshot("ToDelete");
            Assert.That(_caretaker.GetSnapshotCount(), Is.EqualTo(0));
        }

        [Test]
        public void RestoreNonExistentSnapshot_Fails()
        {
            _caretaker.RestoreSnapshot(_cart, "NonExistent");
            // Should not throw, just log
            Assert.That(_cart.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public void CartHistory_Tracking()
        {
            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Item1", Price = 10m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "Snap1");

            _cart.AddItem(new CartItem { ProductId = "P2", ProductName = "Item2", Price = 20m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "Snap2");

            var history = _caretaker.GetHistory();
            Assert.That(history.Count, Is.EqualTo(2));
            Assert.That(history[0].Items.Count, Is.EqualTo(1));
            Assert.That(history[1].Items.Count, Is.EqualTo(2));
        }

        [Test]
        public void ComplexCartScenario()
        {
            // Build initial cart
            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Laptop", Price = 999m, Quantity = 1 });
            _cart.AddItem(new CartItem { ProductId = "P2", ProductName = "Mouse", Price = 25m, Quantity = 2 });
            decimal checkpoint1Total = _cart.GetTotal();
            _caretaker.SaveSnapshot(_cart, "BeforeClearing");

            // Clear and verify
            _cart.ClearCart();
            Assert.That(_cart.GetTotal(), Is.EqualTo(0m));

            // Restore and verify
            _caretaker.RestoreSnapshot(_cart, "BeforeClearing");
            Assert.That(_cart.GetTotal(), Is.EqualTo(checkpoint1Total));
            Assert.That(_cart.Items.Count, Is.EqualTo(2));
        }

        [Test]
        public void SnapshotIsolation()
        {
            var item1 = new CartItem { ProductId = "P1", ProductName = "Item1", Price = 100m, Quantity = 1 };
            _cart.AddItem(item1);
            _caretaker.SaveSnapshot(_cart, "Snapshot1");

            // Modify original cart
            _cart.RemoveItem("P1");
            _cart.AddItem(new CartItem { ProductId = "P2", ProductName = "Item2", Price = 50m, Quantity = 1 });

            // Retrieve snapshot and verify isolation
            var snapshot = _caretaker.GetSnapshot("Snapshot1");
            Assert.That(snapshot?.Items.Count, Is.EqualTo(1));
            Assert.That(snapshot?.Items[0].ProductId, Is.EqualTo("P1"));

            // Current cart should be different
            Assert.That(_cart.Items.Count, Is.EqualTo(1));
            Assert.That(_cart.Items[0].ProductId, Is.EqualTo("P2"));
        }

        [Test]
        public void SnapshotTimestamp()
        {
            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Item", Price = 10m, Quantity = 1 });
            var beforeTime = DateTime.Now;
            _caretaker.SaveSnapshot(_cart, "TimedSnapshot");
            var afterTime = DateTime.Now;

            var snapshot = _caretaker.GetSnapshot("TimedSnapshot");
            Assert.That(snapshot?.SnapshotTime, Is.GreaterThanOrEqualTo(beforeTime));
            Assert.That(snapshot?.SnapshotTime, Is.LessThanOrEqualTo(afterTime.AddSeconds(1)));
        }

        [Test]
        public void MultipleRestores_SameSnapshot()
        {
            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Item", Price = 10m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "Stable");

            // First restore
            _cart.ClearCart();
            _caretaker.RestoreSnapshot(_cart, "Stable");
            Assert.That(_cart.Items.Count, Is.EqualTo(1));

            // Second restore
            _cart.ClearCart();
            _caretaker.RestoreSnapshot(_cart, "Stable");
            Assert.That(_cart.Items.Count, Is.EqualTo(1));
        }

        [Test]
        public void CheckoutScenario()
        {
            // Build cart
            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Laptop", Price = 999m, Quantity = 1 });
            _cart.AddItem(new CartItem { ProductId = "P2", ProductName = "Mouse", Price = 25m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "ReadyForCheckout");

            // Simulate checkout
            decimal total = _cart.GetTotal();
            Assert.That(total, Is.EqualTo(1024m));

            // Customer changes mind, restore
            _cart.ClearCart();
            _caretaker.RestoreSnapshot(_cart, "ReadyForCheckout");
            Assert.That(_cart.GetTotal(), Is.EqualTo(1024m));
        }

        [Test]
        public void BrowsingHistory()
        {
            // Browse and save checkpoints
            _cart.AddItem(new CartItem { ProductId = "P1", ProductName = "Category1-Item", Price = 50m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "AfterBrowsingCategory1");

            _cart.RemoveItem("P1");
            _cart.AddItem(new CartItem { ProductId = "P2", ProductName = "Category2-Item", Price = 75m, Quantity = 1 });
            _caretaker.SaveSnapshot(_cart, "AfterBrowsingCategory2");

            var history = _caretaker.GetHistory();
            Assert.That(history.Count, Is.EqualTo(2));
            Assert.That(history[0].SnapshotName, Is.EqualTo("AfterBrowsingCategory1"));
            Assert.That(history[1].SnapshotName, Is.EqualTo("AfterBrowsingCategory2"));
        }
    }
}
